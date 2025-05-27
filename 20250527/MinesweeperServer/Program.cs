using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;



namespace MinesweeperServer
{
    internal class Program
    {
        readonly string[] COMMAND = { "start", "level", "retry", "exit", "click", "gameover", "server" };
        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";

        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 512;

        Dictionary<int, Socket> socket_list = new();

        List<List<int>> socket_wait_level_list = new();

        List<List<Room>> room_level_list = new();
        Dictionary<int, Room> playerRoomMap = new Dictionary<int, Room>();

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

            foreach (int recv in socket_list.Keys.ToList()) // ToList()로 복사본 사용
            {
                try
                {
                    Send(COMMAND[6], serverNo, recv, "서버종료");
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

                if (socket_list.ContainsValue(clientSock)) continue;

                int count = cnt++;
                socket_list.Add(count, clientSock);
                Console.WriteLine($"[서버] 클라이언트 접속: IP 주소={clientSock.RemoteEndPoint}");

                Thread thread = new Thread(() => ProccessReceive(count));
                thread.Start();
            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        void initialize()
        {
            for (int i = 0; i < 3; i++)
            {
                socket_wait_level_list.Add(new List<int>());
                room_level_list.Add(new List<Room>());
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
                        int player1 = SearchWaitGameSocket(level);
                        if (player1 < 0) break;
                        socket_wait_level_list[level].Remove(player1);

                        int player2 = SearchWaitGameSocket(level);
                        if (player2 < 0 || player2 == player1)
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
        private void MatchingGame(int player1, int player2, int level)
        {
            // Room 생성
            Room newRoom = new Room(nextRoomId++, player1, player2, level);
            newRoom.InitGame();

            // 플레이 중 방 리스트에 추가
            room_level_list[level].Add(newRoom);

            playerRoomMap[player1] = newRoom;
            playerRoomMap[player2] = newRoom;

            SetInitGame(newRoom);
        }

        void SetInitGame(Room room)
        {
            string strMine = room.MineConvertString();

            int player1 = room.Player1_Id;
            int player2 = room.Player2_Id;

            Point firstclick = room.FirstClick;

            Send(COMMAND[0], player1, player2, strMine);
            Send(COMMAND[0], player2, player1, strMine);

            Send(COMMAND[4], serverNo, player1, $"{firstclick.X}*{firstclick.Y}");
            Send(COMMAND[4], serverNo, player2, $"{firstclick.X}*{firstclick.Y}");

            Console.WriteLine($"[LOG] player1 : {player1} player2 : {player2} Mine : {strMine}");
        }

        public Room? GetPlayerRoom(int playerId)
        {
            if (playerRoomMap.TryGetValue(playerId, out Room room))
                return room;
            return null;
        }
        private void RemovePlayerFromRoom(int playerId)
        {
            if (playerRoomMap.TryGetValue(playerId, out Room room))
            {
                playerRoomMap.Remove(playerId);

                // 방에 있는 두 플레이어가 모두 나갔는지 확인 후 방 삭제
                bool p1Gone = !playerRoomMap.ContainsKey(room.Player1_Id);
                bool p2Gone = !playerRoomMap.ContainsKey(room.Player2_Id);

                if (p1Gone && p2Gone)
                {
                    room_level_list[room.Level].Remove(room);
                    Console.WriteLine($"[LOG] Room {room.RoomId} removed");
                }
            }
        }
        int SearchWaitGameSocket(int level)
        {
            if (socket_wait_level_list[level].Count == 0) return -1;
            foreach (int idx in socket_wait_level_list[level])
            {
                if (!playerRoomMap.ContainsKey(idx))
                { 
                    return idx;
                }
            }
            return -1;
        }
 
        void ProccessReceive(int count)
        {
            while (true)
            {
                byte[] recvBytes = new byte[BUFSIZE];
                int recvLen = 0;

                try
                {
                    string recvData = "";
                    string command, data;
                    int sendNo, recvNo;

                    recvData = Receive(count);

                    if (recvData.Length == 0) return;

                    if (!IsCorrectData(recvData)) return;

                    List<string> list = GetRecvMultiLine(recvData);
                    HashSet<string> processedMessages = new HashSet<string>();


                    for (int i = 0; i < list.Count; i++)
                    {
                        string message = list[i];

                        if (processedMessages.Contains(message)) continue;

                        processedMessages.Add(message);

                        ParsingRecvData(list[i], out command, out sendNo, out recvNo, out data);

                        Recv_Log(command, sendNo, recvNo, data);
                        
                        if (command == "level")
                        {
                            HandleLevelCommand(data, count);
                        }
                        else if (command == "retry")
                        {
                            HandleRetryCommand(sendNo);
                        }
                        else if (command == "exit")
                        {
                            HandleExitCommand(sendNo, count);
                        }
                        else if (command == "click")
                        {
                            HandleClickCommand(sendNo, recvNo, data);
                        }
                        else if (command == "gameover")
                        {
                            HandleGameOverCommand(sendNo, recvNo, data);
                        }
                        else if (command == "server")
                        {
                            HandleServerCommand(count);
                        }
                    }
                    processedMessages.Clear();
                }
                catch (SocketException e)
                {

                }
            }
        }

        // level 커맨드 처리
        void HandleLevelCommand(string data, int count)
        {
            if (playerRoomMap.ContainsKey(count)) return;
            int level = Convert.ToInt32(data);
            socket_wait_level_list[level].Add(count);
        }

        // retry 커맨드 처리
        void HandleRetryCommand(int sendNo)
        {
            Room? room = GetPlayerRoom(sendNo);
            if (room == null) return;

            room.SetRetryFlag(sendNo);

            if ( room.IsGameRetry() )
            {
                room.InitGame();
                SetInitGame(room);
            }
        }

        // exit 커맨드 처리
        void HandleExitCommand(int sendNo, int count)
        {
            LogSocketCount();

            socket_list[count].Shutdown(SocketShutdown.Both);
            socket_list[count].Close();
            socket_list[count].Dispose();

            socket_list.Remove(count);

            Console.WriteLine($"[LOG] socket_list : {socket_list.Count}");
            
        }

        // click 커맨드 처리
        void HandleClickCommand(int sendNo, int recvNo, string data)
        {
            Send(COMMAND[4], sendNo, recvNo, data);
        }

        // gameover 커맨드 처리
        void HandleGameOverCommand(int sendNo, int recvNo, string data)
        {
            Send(COMMAND[4], sendNo, recvNo, "click");

            if (data.Equals("win"))
            {
                Send(COMMAND[5], serverNo, sendNo, data);
                Send(COMMAND[5], serverNo, recvNo, "lose");
            }
            else if (data.Equals("lose"))
            {
                Send(COMMAND[5], serverNo, sendNo, data);
                Send(COMMAND[5], serverNo, recvNo, "win");
            }
            else if (data.Equals("draw"))
            {
                Send(COMMAND[5], serverNo, sendNo, data);
                Send(COMMAND[5], serverNo, recvNo, "draw");
            }
            else if (data.Equals("delay"))
            {
                Send(COMMAND[5], serverNo, sendNo, "solo");
                Send(COMMAND[5], serverNo, recvNo, "multi");
            }
            else
                Send(COMMAND[5], sendNo, recvNo, data);
        }

        // server 커맨드 처리
        void HandleServerCommand(int count)
        {
            Send(COMMAND[6], serverNo, count, count.ToString());
        }
        void Recv_Log(string command, int sendNo, int recvNo, string data)
        {
            Console.WriteLine($"[recv] command : {command}, sendPlayer : {sendNo}, recvPlayer : {recvNo}, data : {data}");
        }
        void Send_Log(string command, int sendNo, int recvNo, string data)
        {
            Console.WriteLine($"[send] command : {command}, sendPlayer : {sendNo}, recvPlayer : {recvNo}, data : {data}");
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
        string Receive(int count)
        {
            if (!socket_list.ContainsKey(count)) return "";
            Socket socket = socket_list[count];
            byte[] recvBytes = new byte[BUFSIZE];
            int recvLen = socket.Receive(recvBytes);
            string recvData = Encoding.UTF8.GetString(recvBytes, 0, recvLen);
            return recvData;
        }

        void LogSocketCount()
        {
            Console.WriteLine($"[LOG] Play to lowlevel : {room_level_list[0].Count * 2}  " +
                                $"Play to midlevel : {room_level_list[1].Count * 2}  " +
                                $"Play to highlevel : {room_level_list[2].Count * 2}  ");
            Console.WriteLine($"[LOG] Wait to lowlevel : {socket_wait_level_list[0].Count}  " +
                $"Wait to midlevel : {socket_wait_level_list[1].Count}  " +
                $"Wait to highlevel : {socket_wait_level_list[2].Count}");
        }
        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string data)
        {
            string eData = ExtractionData(recvData);
            try
            {
                string[] s = eData.Split(" ");
                command = s[0];
                sendNo = Convert.ToInt32(s[1]);
                recvNo = Convert.ToInt32(s[2]);

                if (s.Length > 4)
                    data = ConcatData(s);
                else
                    data = s[3];
            }
            catch (Exception e)
            {
                command = "";
                sendNo = -1;
                recvNo = -1;
                data = "";
                return false;
            }
            return true;
        }
        string ConcatData(string[] data)
        {
            string s = "";

            for (int i = 3; i < data.Length; i++)
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

        void Send(string command, int sendNo, int recvNo, string data)
        {
            string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {data}{END_WORD}";
            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            socket_list[recvNo].Send(byteData);
            Send_Log(command, sendNo, recvNo, data);
        }
    }
}


/*
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 */