using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.ComponentModel;
using static System.Collections.Specialized.BitVector32;
using static MinesweeperServer.Program;



namespace MinesweeperServer
{
    internal class Program
    {
        public enum GAMESTATE { 진행전, 게임중, 게임오버, 클리어, 보류 };
        public enum GAMERESULT { 진행중 = -1, 무승부 = 0, 승리 = 1, 패배 = 2, 보류 = 3 }
        public enum PLAYERORDER { 선공, 후공 }

        public enum CommandType { Server, Client, Game, System };
        /*
        client: 클라이언트 간 상호작용 (level 선택, retry 요청 등)
        game: 게임 내 동작(click, result ) [ click - alive, clear, dead   result - win, lose, draw ]
        server: 서버 메시지 ( recv 만 가능하게 절대 이걸로 send 하기 없기 )
        system: 연결/종료 관련 등
        */
        public enum ServerAction { OpponentInfo, TurnInfo, Matching }
        public enum ClientAction { Retry, Level, LeaveRoom, Exit };
        public enum GameAction { Click, Result, FirstClick, MineMap, ReGame, Exit }
        public enum SystemAction { Connect, Exit, Error, Disconnect, CloseRoom, Wait };

        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";

        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 1024;

        private Dictionary<int, Player> playerMap = new();
        private Dictionary<Player, Room> playerRoomMap = new();
        private List<List<Player>> socket_wait_level_list = new();
        private List<Player> LeaveRoomPlayer = new();

        int cnt = 0;
        int serverNo = -9999;
        private int nextRoomId = 1;
        
        Random rand = new Random();


        static void Main(string[] args)
        {
            Program p = new();
            AppDomain.CurrentDomain.ProcessExit += (_, __) => p.end();
            p.start();
        }

        void end()
        {
            Console.WriteLine("[서버 종료] 클라이언트에게 서버 종료 명령 전송");

            foreach (int recv in playerMap.Keys) // ToList()로 복사본 사용
            {
                try
                {
                    Send(CommandType.System, serverNo, recv, SystemAction.Disconnect , "서버종료");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[경고] 클라이언트 {recv} 종료 전송 실패: {ex.Message}");
                }
            }
        }
        void start()
        {
            initialize();

            // 게임 매칭 스레드
            Thread matchThread = new Thread(() => ThreadMatchingGame());
            matchThread.Start();

            // 서버 소켓 생성 및 설정
            Socket listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, SERVERPORT);
            listenSock.Bind(serverEP);
            listenSock.Listen(5);

            Console.WriteLine($"[서버] {SERVERPORT}번 포트에서 대기 중...");

            while (true)
            {
                // 클라이언트 소켓 생성 및 스레드 생성
                Socket clientSock = listenSock.Accept();

                if (isSocketContains(clientSock)) continue;

                int count = cnt++;
                Player player = new Player(count, clientSock);
                playerMap[count] = player;
                Console.WriteLine($"[서버] 클라이언트 접속: IP 주소={clientSock.RemoteEndPoint}");

                string sendData = $"{START_WORD}{CommandType.System} {serverNo} {serverNo} {SystemAction.Connect} {count}{END_WORD}";
                byte[] byteData = Encoding.UTF8.GetBytes(sendData);
                player.Player_Socket.Send(byteData);

                Thread thread = new Thread(() => ProccessReceive(player));
                thread.Start();
            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        bool isSocketContains(Socket socket)
        {
            foreach ( Player p in playerMap.Values)
            {
                if (p.Player_Socket == socket) return true;
            }
            return false;
        }

        void initialize()
        {
            for (int i = 0; i < 3; i++)
            {
                socket_wait_level_list.Add( []);
            }
        }
        void ThreadMatchingGame()
        {
            while (true)
            {
                for (int level = 0; level < 3; level++)
                {
                    // 대기 인원이 짝수 이상이고 0이 아닐 때만 매칭 시도
                    while (socket_wait_level_list[level].Count >= 2)
                    {
                        Player player1 = SearchWaitGameSocket(level);
                        if (player1 == null) break;
                        socket_wait_level_list[level].Remove(player1);

                        Player player2 = SearchWaitGameSocket(level);
                        if (player2 == null || player2.Id == player1.Id)
                        {
                            socket_wait_level_list[level].Add(player1);
                            break;
                        }
                        socket_wait_level_list[level].Remove(player2);
                        MatchingGame(player1, player2, level);
                    }
                }
                Thread.Sleep(100);
            }
        }
        private void MatchingGame(Player player1, Player player2, int level)
        {
            // Room 생성
            Room newRoom = new Room(nextRoomId++, player1, player2, level);
            newRoom.InitGame();

            playerRoomMap[player1] = newRoom;
            playerRoomMap[player2] = newRoom;

            SetInitGame(playerRoomMap[player1]);
        }

        void SetInitGame(Room room)
        {
            string strMine = room.MineConvertString();

            Console.WriteLine($"room level : {room.Level} , room gamePan : {room.GetGamePanSize()} mineLength : {strMine.Length} ");

            int player1 = room.Player1.Id;
            int player2 = room.Player2.Id;

            // 0. 매칭이 됐음을 알림
            Send(CommandType.Server, serverNo, player1, ServerAction.Matching, "");
            Send(CommandType.Server, serverNo, player2, ServerAction.Matching, "");

            // 1. 상대방 정보 전송
            Send(CommandType.Server, player1, player2, ServerAction.OpponentInfo, "");
            Send(CommandType.Server, player2, player1, ServerAction.OpponentInfo, "");

            // 2. 턴 정보 전송
            Send(CommandType.Server, player1, player2, ServerAction.TurnInfo, $"{room.FirstTurnPlayer.Id}");
            Send(CommandType.Server, player2, player1, ServerAction.TurnInfo, $"{room.FirstTurnPlayer.Id}");
            // 3. 지뢰맵 전송
            Send(CommandType.Game, player1, player2, GameAction.MineMap, strMine);
            Send(CommandType.Game, player2, player1, GameAction.MineMap, strMine);
            // 4. 최초 클릭 위치 전송 (가장 마지막에)
            //Send(CommandType.Game, serverNo, player1, GameAction.FirstClick, $"{room.FirstClick.X}*{room.FirstClick.Y}");
            //Send(CommandType.Game, serverNo, player2, GameAction.FirstClick, $"{room.FirstClick.X}*{room.FirstClick.Y}");
        }


        public Room? GetPlayerRoom(Player player)
        {
            if (playerRoomMap.TryGetValue(player, out Room room))
                return room;
            return null;
        }
        
        Player? SearchWaitGameSocket(int level)
        {
            if (socket_wait_level_list[level].Count == 0) return null;
            foreach (Player p in socket_wait_level_list[level])
            {
                if (!playerRoomMap.ContainsKey(p))
                { 
                    return p;
                }
            }
            return null;
        }
 
        void ProccessReceive(Player player)
        {
            while (true)
            {
                byte[] recvBytes = new byte[BUFSIZE];
                int recvLen = 0;

                try
                {
                    string recvData = "";
                    string command, action, data;
                    int sendNo, recvNo;

                    recvData = Receive(player);

                    if (recvData.Length == 0) return;

                    if (!IsCorrectData(recvData)) return;

                    List<string> list = GetRecvMultiLine(recvData);
                    HashSet<string> processedMessages = new HashSet<string>();


                    for (int i = 0; i < list.Count; i++)
                    {
                        string message = list[i];

                        if (processedMessages.Contains(message)) continue;

                        processedMessages.Add(message);

                        ParsingRecvData(list[i], out command, out sendNo, out recvNo, out action, out data);

                        Recv_Log(command, action, sendNo, recvNo, data);
                        
                        switch (command)
                        {
                            case "Server":
                                HandleServerCommand(sendNo, recvNo, action, data);
                                break;
                            case "Client":
                                HandleClientCommand(sendNo, recvNo, action, data);
                                break;
                            case "Game":
                                HandleGameCommand(sendNo, recvNo, action, data);
                                break;
                            case "System":
                                HandleSystemCommand(sendNo, recvNo, action, data);
                                break;
                        }
                    }
                    processedMessages.Clear();
                }
                catch (SocketException e)
                {

                }
            }
        }

        /// <summary>
        /// CommandHandler 커맨드핸들러
        /// </summary>
        void HandleServerCommand(int sendNo, int recvNo, string action, string data)
        {
            // ServerAction { Click, Mine };
       
        }
        void HandleClientCommand(int sendNo, int recvNo, string action, string data)
        {
            // ClientAction { Retry, Level, LeaveRoom };
            switch (action)
            {
                case "Retry":
                    break;
                case "Level":
                    HandleLevelAction(sendNo, recvNo, action, data);
                    break;
                case "Exit":
                    HandleExitAction(sendNo);
                    break;
                case "LeaveRoom":
                    HandleLeaveAction(sendNo);
                    break;
            }
        }
        
        void AddPlayer(Room room)
        {
            socket_wait_level_list[room.Level].Add(room.Player1);
            socket_wait_level_list[room.Level].Add(room.Player2);
        }

        void RemoveRoom(Room room)
        {
            playerRoomMap.Remove(room.Player1);
            playerRoomMap.Remove(room.Player2);
        }
        

        void RemoveWaitlist(Player player)
        {
            foreach (var v in socket_wait_level_list)
            {
                if (v.Contains(player)) v.Remove(player);
            }
        }

        Player GetPlayerToPlayerMap(int no)
        {
            return playerMap[no]; 
        }
        void HandleGameCommand(int sendNo, int recvNo, string action, string data)
        {
            // GameAction { Click, Result,FirstClick, MineMap }

            Player sendPlayer = playerMap[sendNo], recvPlayer = playerMap[recvNo];
            Room room = playerRoomMap[sendPlayer];


            switch (action)
            {
                case "Click":
                    HandleClickAction(sendPlayer, recvPlayer, data);
                    break;
                case "Result":
                    HandleResultAction(sendPlayer, recvPlayer, data);
                    break;
                case "FirstClick":
                    HandleFirstClickAction(sendNo, data);
                    break;
                case "MineMap":
                    break;
                case "ReGame":
                    HandleReGameAction(sendPlayer);
                    break;
                case "Exit":
                    break;
            }
        }

       
        void HandleSystemCommand(int sendNo, int recvNo, string action, string data)
        {
            // SystemAction { Connect, Exit, Error, Disconnect };
            switch (action)
            {
                case "Connect":
                    break;
                case "Exit":
                    break;
                case "Error":
                    break;
                case "Disconnect":
                    break;
            }
        }

        /// <summary>
        /// ActionHandler 액션핸들러
        /// </summary>


        // Click Action 처리
        void HandleClickAction(Player sendPlayer, Player recvPlayer, string data)
        {
            PlayerState playerState = Parsing_ClickData_PlayerState(data);
            Room room = playerRoomMap[sendPlayer];
            MatchResult result = room.UpdatePlayerState(sendPlayer, playerState);

            if (result == MatchResult.NotDecided)
                Send(CommandType.Game, sendPlayer.Id, recvPlayer.Id, GameAction.Click, data);
            else
                SendGameResult(result, room);
        }

        void HandleResultAction(Player sendPlayer, Player recvPlayer, string data)
        {
            PlayerState playerState = Parsing_ClickData_PlayerState(data);
            if (playerState == PlayerState.Clear) sendPlayer.ClearTime = Parsing_ClearTime(data);

            Room room = playerRoomMap[sendPlayer];

            MatchResult result = room.UpdatePlayerState(sendPlayer, playerState);

            if (result == MatchResult.NotDecided)
                Send(CommandType.Game, sendPlayer.Id, recvPlayer.Id, GameAction.Click, data);
            else
                SendGameResult(result, room);
        }
        private void HandleLevelAction(int sendNo, int recvNo, string action, string data)
        {
            int level = Convert.ToInt32(data);

            if (sendNo == -1)
            {

                return;
            }

            Player player = GetPlayerToPlayerMap(sendNo);

            if (playerRoomMap.ContainsKey(player))  // 게임 진행중에 바꾸려 할 때
            {
                // 이후 코드 추가
                return; 
            }

            if (LeaveRoomPlayer.Contains(player))
            {
                LeaveRoomPlayer.Remove(player);
            }
            else
            {
                RemoveWaitlist(player);
            }
            socket_wait_level_list[level].Add(player);
        }
        void SendGameResult(MatchResult result, Room room)
        {
            if ( result == MatchResult.Draw)
            {
                Send(CommandType.Game, serverNo, room.Player1.Id, GameAction.Result, "Draw");
                Send(CommandType.Game, serverNo, room.Player2.Id, GameAction.Result, "Draw");
                return;
            }
   
            Send(CommandType.Game, serverNo, room.Winner, GameAction.Result, "Win");
            Send(CommandType.Game, serverNo, room.Loser, GameAction.Result, "Lose");
        }

        void HandleFirstClickAction(int no, string data)
        {
            Room room = playerRoomMap[playerMap[no]];
            if (data.Equals("RequestDataResend"))
            {
                Send(CommandType.Game, serverNo, no, GameAction.FirstClick, $"{room.FirstClick}");
            }
        }

        private void HandleReGameAction(Player sendPlayer)
        {
            Room room = playerRoomMap[sendPlayer];
            Player player1 = room.Player1, player2 = room.Player2;
            room.SetResetRequest(sendPlayer);

            if (room.CanResetGame())
            {
                room.InitGame();

                string strMine = room.MineConvertString();

                // 0. Regame 진행된다는 정보 알림
                Send(CommandType.Game, serverNo, player1.Id, GameAction.ReGame, "");
                Send(CommandType.Game, serverNo, player2.Id, GameAction.ReGame, "");

                // 1. 상대 소켓 정보 전송
                Send(CommandType.Server, player1.Id, player2.Id, ServerAction.OpponentInfo, "");
                Send(CommandType.Server, player2.Id, player1.Id, ServerAction.OpponentInfo, "");
                // 2. 턴 정보 전송
                Send(CommandType.Server, player1.Id, player2.Id, ServerAction.TurnInfo, $"{room.FirstTurnPlayer.Id}");
                Send(CommandType.Server, player2.Id, player1.Id, ServerAction.TurnInfo, $"{room.FirstTurnPlayer.Id}");

                // 3. 마인 위치 전송
                Send(CommandType.Game, player1.Id, player2.Id, GameAction.MineMap, strMine);
                Send(CommandType.Game, player2.Id, player1.Id, GameAction.MineMap, strMine);
                // 4. 최초 클릭 위치 전송 (가장 마지막에)
                //Send(CommandType.Game, serverNo, player1.Id, GameAction.FirstClick, $"{room.FirstClick.X}*{room.FirstClick.Y}");
                //Send(CommandType.Game, serverNo, player2.Id, GameAction.FirstClick, $"{room.FirstClick.X}*{room.FirstClick.Y}");
            }
        }

        private void HandleExitAction(int sendNo)
        {
            Player sendplayer = playerMap[sendNo];
            Room room = playerRoomMap[sendplayer];
            Player oppPlayer = room.GetOppPlayer(sendplayer);
            int level = room.Level;
            Send(CommandType.System, serverNo, sendplayer.Id, SystemAction.CloseRoom, "Close");
            Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.CloseRoom, "Close");

            RemoveRoom(room);
            room.DestroyRoom();

            socket_wait_level_list[level].Add(oppPlayer);
            Send(CommandType.Client, serverNo, sendplayer.Id, SystemAction.Exit, "Exit");
            Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.Wait, "Wait");
            playerMap.Remove(sendplayer.Id);      
        }
        private void HandleLeaveAction(int sendNo)
        {
            Player sendplayer = playerMap[sendNo];
            Room room = playerRoomMap[sendplayer];
            Player oppPlayer = room.GetOppPlayer(sendplayer);
            int level = room.Level;

            Send(CommandType.System, serverNo, sendplayer.Id, SystemAction.CloseRoom, "Close");
            Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.CloseRoom, "Close");

            LeaveRoomPlayer.Add(room.Player1);
            LeaveRoomPlayer.Add(room.Player2);
            RemoveRoom(room);
            room.DestroyRoom();
        }

        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Parsing_ClearTime(string data)
        {
            if (!data.StartsWith("Clear")) return -1;

            return Convert.ToInt32(data.Split(" ")[1]);
        }
        PlayerState Parsing_ClickData_PlayerState(string data)
        {
            if (data.StartsWith("Clear"))
            {
                return PlayerState.Clear;
            }
            else if (data.StartsWith("Dead")) return PlayerState.Dead;

            return PlayerState.Alive;
        }

       
        void Recv_Log(string command, string action, int sendNo, int recvNo, string data)
        {
            Console.WriteLine($"[recv] command : {command}, action : {action}, sendPlayer : {sendNo}, recvPlayer : {recvNo}, data : {data}");
        }
        void Send_Log(string command, int sendNo, int recvNo, string action, string data)
        {
            Console.WriteLine($"[send] command : {command}, action : {action}, sendPlayer : {sendNo}, recvPlayer : {recvNo}, data : {data}");
        }
        List<string> GetRecvMultiLine(string recvData)
        {
            List<string> list = new();
            string[] str = recvData.Split(END_WORD);

            for (int i = 0; i < str.Length; i++)
            {
                if (!str[i].Equals("")) list.Add(str[i]);
            }

            return list;
        }
        string Receive(Player player)
        {
            if (!playerMap.ContainsValue(player)) return "";
            
            byte[] recvBytes = new byte[BUFSIZE];
            int recvLen = player.Player_Socket.Receive(recvBytes);
            string recvData = Encoding.UTF8.GetString(recvBytes, 0, recvLen);
            return recvData;
        }

        void LogSocketCount()
        {
            Console.WriteLine($"[LOG] Room Count : {playerMap.Count}  " +
                                $"Play to User : {playerRoomMap.Count * 2}  " +
                                $"Wait to User : {playerMap.Count - playerRoomMap.Count * 2}  "
                                );
        }
        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string action, out string data)
        {
            string eData = ExtractionData(recvData);
            try
            {
                string[] s = eData.Split(" ");
                command = s[0];
                sendNo = Convert.ToInt32(s[1]);
                recvNo = Convert.ToInt32(s[2]);
                action = s[3];

                if (s.Length > 5)
                    data = ConcatData(s);
                else
                    data = s[4];
            }
            catch (Exception e)
            {
                command = "";
                sendNo = -1;
                recvNo = -1;
                action = "";
                data = "";
                return false;
            }
            return true;
        }
        string ConcatData(string[] data)
        {
            string s = "";

            for (int i = 4; i < data.Length; i++)
            {
                s += $"{data[i]} ";
            }
            return s;
        }

        bool IsCorrectData(string data)
        {
            return data.StartsWith(START_WORD) && data.EndsWith(END_WORD);
        }

        string ExtractionData(string data)
        {
            string eData = data.Replace(START_WORD, "");
            eData = eData.Replace(END_WORD, "");
            return eData;
        }

        void Send<T>(CommandType command, int sendNo, int recvNo, T action, string data) where T : Enum
        {
            string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {action} {data}{END_WORD}";
            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            playerMap[recvNo].Player_Socket.Send(byteData);
            Send_Log(command.ToString(), sendNo, recvNo, action.ToString(), data);
        }
    }
}


/*
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 */