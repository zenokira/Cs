using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.ComponentModel;
using static System.Collections.Specialized.BitVector32;
using static MinesweeperServer.Program;
using System.ComponentModel.Design;
using System.Collections.Concurrent;
using System.Numerics;



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
        public enum ClientAction { Retry, Level, LeaveRoom, Reservation, Exit };
        public enum GameAction { Click, Result, FirstClick, MineMap, ReGame, Exit }
        public enum SystemAction { Connect, Exit, Error, Disconnect, CloseRoom, Wait };

        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";

        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 1024;

        private ConcurrentDictionary<int, Player> playerDict = new ConcurrentDictionary<int, Player>();
        private Dictionary<Player, Room> playerRoomDict = new();
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

            foreach (int recv in playerDict.Keys) // ToList()로 복사본 사용
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

            // 게임 매칭 스레드 시작
            Thread matchThread = new Thread(() => ThreadMatchingGame());
            matchThread.IsBackground = true;
            matchThread.Start();

            // 서버 소켓 생성 및 설정
            Socket listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, SERVERPORT);

            try
            {
                listenSock.Bind(serverEP);
                listenSock.Listen(5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[서버] 소켓 바인딩 또는 리스닝 실패: {ex.Message}");
                return;
            }

            Console.WriteLine($"[서버] {SERVERPORT}번 포트에서 대기 중...");

            while (true)
            {
                Socket clientSock;
                try
                {
                    clientSock = listenSock.Accept();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"[서버] 클라이언트 접속 대기 중 예외: {ex.Message}");
                    continue;
                }

                // 중복 접속 체크 (playerDict에 같은 소켓이 있는지 간단히 체크)
                if (playerDict.Values.Any(p => p.Player_Socket == clientSock))
                {
                    clientSock.Close();
                    continue;
                }

                int playerId = Interlocked.Increment(ref cnt) - 1; // 안전한 증가
                Player player = new Player(playerId, clientSock);
                player.Player_Socket.ReceiveTimeout = 500;  // 타임아웃 설정

                if (!playerDict.TryAdd(playerId, player))
                {
                    Console.WriteLine($"[서버] 플레이어 딕셔너리에 추가 실패: {playerId}");
                    player.Player_Socket.Close();
                    continue;
                }

                Console.WriteLine($"[서버] 클라이언트 접속: IP 주소={clientSock.RemoteEndPoint}");

                try
                {
                    string sendData = $"{START_WORD}{CommandType.System} {serverNo} {serverNo} {SystemAction.Connect} {playerId}{END_WORD}";
                    byte[] byteData = Encoding.UTF8.GetBytes(sendData);
                    player.Player_Socket.Send(byteData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[서버] 초기 연결 메시지 전송 실패: {ex.Message}");
                    player.Player_Socket.Close();
                    playerDict.TryRemove(playerId, out _);
                    continue;
                }

                Thread receiveThread = new Thread(() => ProccessReceive(player));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
        }


        bool isSocketContains(Socket socket)
        {
            foreach ( Player p in playerDict.Values)
            {
                if (p.Player_Socket == socket) return true;
            }
            return false;
        }

        void initialize()
        {
            for (int i = 0; i < 3; i++)
            {
                socket_wait_level_list.Add(new List<Player>());
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

            playerRoomDict[player1] = newRoom;
            playerRoomDict[player2] = newRoom;

            SetInitGame(playerRoomDict[player1]);
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
            if (playerRoomDict.TryGetValue(player, out Room room))
                return room;
            return null;
        }
        
        Player? SearchWaitGameSocket(int level)
        {
            if (socket_wait_level_list[level].Count == 0) return null;
            foreach (Player p in socket_wait_level_list[level])
            {
                if (!playerRoomDict.ContainsKey(p))
                { 
                    return p;
                }
            }
            return null;
        }

        void ProccessReceive(Player player)
        {
            
            if (player == null || player.Player_Socket == null)
                return;

            // ReceiveTimeout은 소켓 생성/연결 직후 한 번만 설정해 주세요.
            player.Player_Socket.ReceiveTimeout = 500;

            byte[] recvBytes = new byte[BUFSIZE];

            while (player != null && player.IsConnected)
            {
                try
                {
                    int recvLen = player.Player_Socket.Receive(recvBytes);
                    if (recvLen <= 0)
                    {
                        // 클라이언트가 연결을 끊음
                        Console.WriteLine($"[{player.Id}] 연결 종료 감지");
                        break;
                    }

                    string recvData = Encoding.UTF8.GetString(recvBytes, 0, recvLen);

                    if (string.IsNullOrEmpty(recvData))
                        continue;

                    if (!IsCorrectData(recvData))
                        continue;

                    List<string> list = GetRecvMultiLine(recvData);
                    HashSet<string> processedMessages = new HashSet<string>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        string message = list[i];

                        if (processedMessages.Contains(message)) continue;

                        processedMessages.Add(message);

                        ParsingRecvData(list[i], out string command, out int sendNo, out int recvNo, out string action, out string data);

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
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        // Receive 타임아웃: 데이터 없으니 루프 계속 돌면서 종료 플래그 체크
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"[{player.Id}] SocketException: {ex.Message}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{player.Id}] 예외 발생: {ex.Message}");
                    break;
                }
            }

            // 루프 종료 시 클린업 호출
            CleanupPlayerConnection(player);
        }

        void CleanupPlayerConnection(Player player)
        {
            if (player == null) return;

            try
            {
                // 플레이어 연결 상태 플래그 종료
                player.IsConnected = false;

                // 소켓 종료
                try
                {
                    player.Player_Socket?.Shutdown(SocketShutdown.Both);
                }
                catch { /* 이미 종료된 소켓 예외 무시 */ }
                finally
                {
                    player.Player_Socket?.Close();
                    player.Player_Socket?.Dispose();
                }

                // Dictionary에서 안전하게 제거
                if (playerDict.TryRemove(player.Id, out _))
                {
                    Console.WriteLine($"플레이어 {player.Id}의 소켓 연결을 서버에서 정리했습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cleanup] 예외 발생: {ex.Message}");
            }
        }

        void ExitPlayer(Player player)
        {
            if (playerDict.TryRemove(player.Id, out var removedPlayer))
            {
                // 제거 성공 시 처리할 내용 (필요하면)
            }
            player.DisconnectPlayer();
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
                case "Reservation":
                    HandleClientReservationAction(sendNo, data);
                    break;
                case "LeaveRoom":
                    HandleLeaveAction(sendNo);
                    break;
                case "Exit":
                    HandleClientExitAction(sendNo);
                    break;
            }
        }

        private void HandleLeaveAction(int sendNo)
        {
            if (!playerDict.TryGetValue(sendNo, out Player sendplayer))
            {
                Console.WriteLine($"[HandleLeaveAction] playerDict에 키 {sendNo}가 없습니다.");
                return;
            }

            if (!playerRoomDict.TryGetValue(sendplayer, out Room room))
            {
                Console.WriteLine($"[HandleLeaveAction] playerRoomDict에 플레이어 {sendplayer.Id}가 없습니다.");
                return;
            }

            Player oppPlayer = room.GetOppPlayer(sendplayer);
            int level = room.Level;

            RemoveRoomAndPlayer(room);

            Send(CommandType.System, serverNo, sendplayer.Id, SystemAction.Wait, "Exit");
            Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.Wait, "OppExit");
        }

        int RemoveRoomAndPlayer(Room room)
        {
            int level = room.Level;
            playerRoomDict.Remove(room.Player1);
            playerRoomDict.Remove(room.Player2);
            room.DestroyRoom();
            return level;
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
            if (playerDict.TryGetValue(no, out Player player))
            {
                return player;
            }
            else
            {
                // 키가 없으면 null 반환하거나 예외 대신 적절히 처리
                return null;
            }
        }
        void HandleGameCommand(int sendNo, int recvNo, string action, string data)
        {
            // GameAction { Click, Result,FirstClick, MineMap }

            if (!playerDict.TryGetValue(sendNo, out Player sendPlayer))
            {
                Console.WriteLine($"playerDict에 키 {sendNo}가 없습니다.");
                return; // 또는 예외 처리
            }

            if (!playerDict.TryGetValue(recvNo, out Player recvPlayer))
            {
                Console.WriteLine($"playerDict에 키 {recvNo}가 없습니다.");
                return; // 또는 예외 처리
            }

            if (!playerRoomDict.TryGetValue(sendPlayer, out Room room))
            {
                Console.WriteLine($"playerRoomDict에 플레이어 {sendPlayer.Id}가 없습니다.");
                return; // 또는 예외 처리
            }


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
                    HandleSystemExitAction(sendNo);
                    break;
                case "Error":
                    break;
                case "Disconnect":
                    break;
            }
        }

        
        void UpdateRoomState(MatchResult result   ,Room room)
        {
            if (!room.IsReservationPlayer())
            {
                SendGameResult(result, room);
                return;
            }
            
            int n = room.WhoIsReservation();

            if (n == 0) return;

            int Level = room.Level;
            Player player1 = room.Player1, player2 = room.Player2;
            int winner = room.Winner, loser = room.Loser;
            RemoveRoomAndPlayer(room);


            string result1 = GetResultText(player1.Id, winner, result);
            string result2 = GetResultText(player2.Id, winner, result);

            // 3. 둘 다 나가기 예약자인 경우
            if (player1.IsReservation() > 0 && player2.IsReservation() > 0)
            {
                SendReservePlayerMessage(player1, result1);
                SendReservePlayerMessage(player2, result2);

                return;
            }

            // 2. 한 명만 예약자인 경우
            Player reserver = (n == 1) ? player1 : player2;
            Player opponent = (n == 1) ? player2 : player1;

            string reserverResult = reserver.Id == player1.Id ? result1 : result2;
            string opponentResult = opponent.Id == player1.Id ? result1 : result2;


            SendReservePlayerMessage(reserver, reserverResult);
            Send(CommandType.System, serverNo, opponent.Id, SystemAction.CloseRoom, opponentResult);
        }

        string GetResultText(int playerId, int winner, MatchResult result)
        {
            if (result == MatchResult.Draw) return "Draw";
            return playerId == winner ? "Win" : "Lose";
        }

        void SendReservePlayerMessage(Player player, string result)
        {
            if (player.IsReservation() == 1)
                Send(CommandType.System, serverNo, player.Id, SystemAction.CloseRoom, result);
            else if (player.IsReservation() == 2)
            {
                Send(CommandType.Client, serverNo, player.Id, ClientAction.Exit, result);
                RemovePlayer(player);
            }
        }

        void RemovePlayer(Player player)
        {
            player?.DisconnectPlayer();

            if (player != null && playerDict.ContainsKey(player.Id))
            {
                if (playerDict.TryRemove(player.Id, out var removedPlayer))
                {
                    // 제거 성공 시 처리할 내용 (필요하면)
                }
            }
        }


        /// <summary>
        /// ActionHandler 액션핸들러
        /// </summary>
        void HandleSystemExitAction(int sendNo)
        {
            if (!playerDict.TryGetValue(sendNo, out Player sendPlayer)) return;
            if (!playerRoomDict.TryGetValue(sendPlayer, out Room room) || room == null) return;

            Player oppPlayer = room.GetOppPlayer(sendPlayer);
            int oppLevel = room.Level;

            RemoveRoomAndPlayer(room);

            ExitPlayer(sendPlayer);
           
            if (oppPlayer != null)
            {
                Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.Wait, "OppExit");
                socket_wait_level_list[oppLevel].Add(oppPlayer);
            }
        }

        // Click Action 처리
        void HandleClickAction(Player sendPlayer, Player recvPlayer, string data)
        {
            PlayerState playerState = Parsing_ClickData_PlayerState(data);
            Room room = playerRoomDict[sendPlayer];
            MatchResult result = room.UpdatePlayerState(sendPlayer, playerState);

            if (result == MatchResult.NotDecided)
                Send(CommandType.Game, sendPlayer.Id, recvPlayer.Id, GameAction.Click, data);
            else
            {
                UpdateRoomState(result , room);
            }
        }

        void HandleResultAction(Player sendPlayer, Player recvPlayer, string data)
        {
            PlayerState playerState = Parsing_ClickData_PlayerState(data);
            if (playerState == PlayerState.Clear) sendPlayer.ClearTime = Parsing_ClearTime(data);

            Room room = playerRoomDict[sendPlayer];

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

            if (playerRoomDict.ContainsKey(player))  // 게임 진행중에 바꾸려 할 때
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
            if (playerDict.TryGetValue(no, out Player player))
            {
                if (playerRoomDict.TryGetValue(player, out Room room))
                {
                    // 안전하게 room 사용
                    if (data.Equals("RequestDataResend"))
                    {
                        Send(CommandType.Game, serverNo, no, GameAction.FirstClick, $"{room.FirstClick}");
                    }
                }
                else
                {
                    Console.WriteLine($"playerRoomDict에 플레이어 {player.Id}가 없습니다.");
                    // 예외 처리 또는 리턴
                }
            }
            else
            {
                Console.WriteLine($"playerDict에 키 {no}가 없습니다.");
                // 예외 처리 또는 리턴
            }
        }

        private void HandleReGameAction(Player sendPlayer)
        {
            Room room = playerRoomDict[sendPlayer];
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

        private void HandleClientExitAction(int sendNo)
        {
            if (!playerDict.TryGetValue(sendNo, out Player sendplayer)) return;
            if (!playerRoomDict.TryGetValue(sendplayer, out Room room) || room == null) return;

            Player oppPlayer = room.GetOppPlayer(sendplayer);
            int level = room.Level;

            RemoveRoomAndPlayer(room);
          
            if (oppPlayer != null)
            {
                socket_wait_level_list[level].Add(oppPlayer);
                Send(CommandType.System, serverNo, oppPlayer.Id, SystemAction.Wait, "OppExit");
            }

            Send(CommandType.Client, serverNo, sendplayer.Id, SystemAction.Exit, "Exit");

            if (playerDict.TryRemove(sendplayer.Id, out var removedPlayer))
            {
                // 제거 성공 시 처리할 내용 (필요하면)
            }

        }

        private void HandleClientReservationAction(int sendNo, string data)
        {
            Player? sendPlayer = null;
            if (!playerDict.TryGetValue(sendNo, out sendPlayer))
            {
                // sendPlayer가 없을 때 처리
                sendPlayer = null;
            }
            if (sendPlayer == null) return;

            Room? playRoom = GetPlayerRoom(sendPlayer);
            if (playRoom == null) return;

            if ( data.Equals("Exit"))
            {
                sendPlayer.ExitReserved = true;
                Console.WriteLine($"플레이어 {sendPlayer.Id} 가 멀티게임 종료를 예약하셨습니다.");
            }
            else if ( data.Equals("LeaveRoom"))
            {
                sendPlayer.LeaveReserved = true;
                Console.WriteLine($"플레이어 {sendPlayer.Id} 가 방 나가기를 예약하셨습니다.");
            }
        }
        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Parsing_ClearTime(string data)
        {
            if (string.IsNullOrEmpty(data) || !data.StartsWith("Clear"))
                return -1;

            string[] parts = data.Split(' ');
            if (parts.Length < 2) return -1;

            if (int.TryParse(parts[1], out int clearTime))
                return clearTime;
            return -1;
        }

        PlayerState Parsing_ClickData_PlayerState(string data)
        {
            if (string.IsNullOrEmpty(data)) return PlayerState.Alive;

            if (data.StartsWith("Clear"))
                return PlayerState.Clear;
            else if (data.StartsWith("Dead"))
                return PlayerState.Dead;

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
            if (string.IsNullOrEmpty(recvData)) return new List<string>();

            string[] parts = recvData.Split(END_WORD, StringSplitOptions.RemoveEmptyEntries);

            List<string> list = new();
            foreach (var part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    list.Add(trimmed);
            }
            return list;
        }

        string Receive(Player player)
        {
            if (player == null || !playerDict.ContainsKey(player.Id))
                return "";
            player.Player_Socket.ReceiveTimeout = 500;
            try
            {
                byte[] recvBytes = new byte[BUFSIZE];
                int recvLen = player.Player_Socket.Receive(recvBytes);
                if (recvLen <= 0) return "";

                return Encoding.UTF8.GetString(recvBytes, 0, recvLen);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Receive] SocketException from player {player?.Id}: {ex.Message}");
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Receive] Exception from player {player?.Id}: {ex.Message}");
                return "";
            }
        }

        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string action, out string data)
        {
            command = "";
            sendNo = -1;
            recvNo = -1;
            action = "";
            data = "";

            if (string.IsNullOrEmpty(recvData)) return false;

            string eData = ExtractionData(recvData);
            if (string.IsNullOrEmpty(eData)) return false;

            string[] s = eData.Split(' ', 5, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length < 4) return false;

            command = s[0];

            if (!int.TryParse(s[1], out sendNo)) return false;
            if (!int.TryParse(s[2], out recvNo)) return false;

            action = s[3];

            if (s.Length == 5)
            {
                data = s[4];
            }
            else
            {
                data = "";
            }
            return true;
        }

        string ConcatData(string[] data)
        {
            if (data == null || data.Length <= 4) return "";

            StringBuilder sb = new();
            for (int i = 4; i < data.Length; i++)
            {
                sb.Append(data[i]).Append(' ');
            }
            return sb.ToString().TrimEnd();
        }

        bool IsCorrectData(string data)
        {
            return !string.IsNullOrEmpty(data) && data.StartsWith(START_WORD) && data.EndsWith(END_WORD);
        }

        string ExtractionData(string data)
        {
            if (string.IsNullOrEmpty(data)) return "";

            string eData = data;
            if (eData.StartsWith(START_WORD))
                eData = eData[START_WORD.Length..];

            if (eData.EndsWith(END_WORD))
                eData = eData[..^END_WORD.Length];

            return eData.Trim();
        }

        void Send<T>(CommandType command, int sendNo, int recvNo, T action, string data) where T : Enum
        {
            if (!playerDict.TryGetValue(recvNo, out Player? player))
            {
                Console.WriteLine($"[Send] Warning: recvNo {recvNo} not found in playerDict.");
                return;
            }

            try
            {
                string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {action} {data}{END_WORD}";
                byte[] byteData = Encoding.UTF8.GetBytes(sendData);
                player.Player_Socket.Send(byteData);
                Send_Log(command.ToString(), sendNo, recvNo, action.ToString(), data);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[Send] SocketException while sending to player {recvNo}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Send] Exception while sending to player {recvNo}: {ex.Message}");
            }
        }

    }
}


/*
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 */