using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Text;

namespace MinesweeperServer
{
    internal class Program
    {
        readonly string[] COMMAND = { "start", "level", "retry", "exit", "click", "gameover", "server" };
        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";

        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 512;

        Dictionary<int, Socket> socket_list = new();

        List<List<int>> socket_play_level_list = new();
        List<List<int>> socket_wait_level_list = new();

        int cnt = 0;
        int serverNo = -9999;
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

            foreach (int recv in socket_list.Keys.ToList())
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
                socket_play_level_list.Add(new List<int>());
                socket_wait_level_list.Add(new List<int>());
            }
        }

        void ThreadMatchingGame()
        {
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (socket_wait_level_list[i].Count % 2 == 0 && socket_wait_level_list[i].Count != 0)
                    {
                        int player1 = SearchWaitGameSocket(i);
                        if (player1 < 0) continue;

                        socket_play_level_list[i].Add(player1);
                        int player2 = SearchWaitGameSocket(i);
                        if (player2 < 0)
                        {
                            socket_play_level_list[i].Remove(player1);
                            continue;
                        }

                        socket_play_level_list[i].Add(player2);

                        socket_wait_level_list[i].Remove(player1);
                        socket_wait_level_list[i].Remove(player2);

                        MatchingGame(player1, player2);
                    }
                }
                Thread.Sleep(100);
            }
        }

        int SearchWaitGameSocket(int listidx)
        {
            if (socket_wait_level_list[listidx].Count == 0) return -1;
            foreach (int idx in socket_wait_level_list[listidx])
            {
                if (!socket_play_level_list[listidx].Contains(idx))
                {
                    return idx;
                }
            }
            return -1;
        }

        void MatchingGame(int player1, int player2)
        {
            int level = GetGameLevel(player1, player2);
            if (level == -1) return;

            List<Point> mine = RandomLayingMine(level);
            string strMine = MineConvertString(mine);

            Send(COMMAND[0], player1, player2, strMine);
            Send(COMMAND[0], player2, player1, strMine);

            Point point = GenerateRandomPoint(level, mine);

            Send(COMMAND[4], serverNo, player1, $"{point.X}*{point.Y}");
            Send(COMMAND[4], serverNo, player2, $"{point.X}*{point.Y}");

            Console.WriteLine($"[LOG] player1 : {player1} player2 : {player2} Mine : {strMine}");
        }

        int GetGameLevel(int player1, int player2)
        {
            int p1 = IsGamePlay(player1);
            int p2 = IsGamePlay(player2);
            if (p1 == p2 && p1 != -1) return p1;
            return -1;
        }

        void ProccessReceive(int count)
        {
            while (true)
            {
                try
                {
                    string recvData = Receive(count);
                    if (recvData.Length == 0) return;

                    if (!IsCorrectData(recvData)) return;

                    List<string> list = GetRecvMultiLine(recvData);
                    HashSet<string> processedMessages = new HashSet<string>();

                    foreach (string message in list)
                    {
                        if (processedMessages.Contains(message)) continue;
                        processedMessages.Add(message);

                        string command, data;
                        int sendNo, recvNo;
                        ParsingRecvData(message, out command, out sendNo, out recvNo, out data);

                        Recv_Log(command, sendNo, recvNo, data);

                        ProcessCommand(command, sendNo, recvNo, data);
                    }

                    processedMessages.Clear();
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"[Error] SocketException: {e.Message}");
                }
            }
        }

        void ProcessCommand(string command, int sendNo, int recvNo, string data)
        {
            switch (command)
            {
                case "level":
                    search4correctwaitingroom(data, sendNo);
                    LogSocketCount();
                    break;

                case "retry":
                    HandleRetry(sendNo);
                    break;

                case "exit":
                    HandleExit(sendNo);
                    break;

                case "click":
                    HandleClick(sendNo, recvNo, data);
                    break;

                case "gameover":
                    HandleGameOver(sendNo, recvNo, data);
                    break;

                case "server":
                    Send(COMMAND[6], serverNo, sendNo, sendNo.ToString());
                    break;
            }
        }

        void HandleRetry(int sendNo)
        {
            int result = IsGamePlay(sendNo);
            if (result == -1)
            {
                Send(COMMAND[6], serverNo, sendNo, "error");
                return;
            }
            ResetGameForRetry(result, sendNo);
        }

        void HandleExit(int sendNo)
        {
            int result = IsGamePlay(sendNo);
            if (result == -1) return;

            while (!RemovePlayListItem(sendNo, result)) ;
            LogSocketCount();

            socket_list[sendNo].Shutdown(SocketShutdown.Both);
            socket_list[sendNo].Close();
            socket_list[sendNo].Dispose();

            socket_list.Remove(sendNo);

            Console.WriteLine($"[LOG] socket_list : {socket_list.Count}");
        }

        void HandleClick(int sendNo, int recvNo, string data)
        {
            Send(COMMAND[4], sendNo, recvNo, data);
        }

        void HandleGameOver(int sendNo, int recvNo, string data)
        {
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
            {
                Send(COMMAND[5], sendNo, recvNo, data);
            }
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
            return Encoding.UTF8.GetString(recvBytes, 0, recvLen);
        }

        void LogSocketCount()
        {
            Console.WriteLine($"[LOG] Play to lowlevel : {socket_play_level_list[0].Count}  " +
                                $"Play to midlevel : {socket_play_level_list[1].Count}  " +
                                $"Play to highlevel : {socket_play_level_list[2].Count}");
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

                data = s.Length > 4 ? ConcatData(s) : s[3];
            }
            catch (Exception)
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
            return string.Join(" ", data.Skip(3));
        }

        bool IsCorrectData(string data)
        {
            return data.StartsWith(START_WORD) && data.EndsWith(END_WORD);
        }

        string ExtractionData(string data)
        {
            return data.Replace(START_WORD, "").Replace(END_WORD, "");
        }

        List<Point> RandomLayingMine(int level)
        {
            List<Point> mine = new();

            while (mine.Count < MINE_COUNT[level])
            {
                Point pt = GenerateRandomPoint(level, mine);
                mine.Add(pt);
            }

            return mine;
        }

        void Send(string command, int sendNo, int recvNo, string data)
        {
            string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {data}{END_WORD}";
            byte[] byteData = Encoding.UTF8.GetBytes(sendData);
            socket_list[recvNo].Send(byteData);
            Send_Log(command, sendNo, recvNo, data);
        }

        string MineConvertString(List<Point> mine)
        {
            return string.Join(" ", mine.Select(p => $"{p.X}*{p.Y}"));
        }

        void search4correctwaitingroom(string data, int socketNo)
        {
            int playlevel = IsGamePlay(socketNo);
            int waitlevel = Convert.ToInt32(data);
            if (playlevel == -1)
            {
                socket_wait_level_list[waitlevel].Add(socketNo);
                return;
            }

            socket_play_level_list[playlevel].Remove(socketNo);
            socket_wait_level_list[waitlevel].Add(socketNo);
        }

        int IsGamePlay(int socketNo)
        {
            for (int i = 0; i < socket_play_level_list.Count; i++)
            {
                if (socket_play_level_list[i].Contains(socketNo)) return i;
            }

            return -1;
        }

        bool RemovePlayListItem(int socketNo, int idx)
        {
            socket_play_level_list[idx].Remove(socketNo);
            return !socket_play_level_list[idx].Contains(socketNo);
        }

        void ResetGameForRetry(int level, int playerNo)
        {
            List<Point> newMines = RandomLayingMine(level);
            string strMine = MineConvertString(newMines);

            Send(COMMAND[0], playerNo, GetOpponent(playerNo), strMine);
            Send(COMMAND[0], GetOpponent(playerNo), playerNo, strMine);

            Point newPoint = GenerateRandomPoint(level, newMines);
            Send(COMMAND[4], serverNo, playerNo, $"{newPoint.X}*{newPoint.Y}");
            Send(COMMAND[4], serverNo, GetOpponent(playerNo), $"{newPoint.X}*{newPoint.Y}");

            Console.WriteLine($"[LOG] Game reset for player {playerNo} with new mines and board.");
        }

        int GetOpponent(int playerNo)
        {
            foreach (var levelList in socket_play_level_list)
            {
                if (levelList.Contains(playerNo))
                {
                    return levelList.FirstOrDefault(p => p != playerNo);
                }
            }
            return -1;
        }

        Point GenerateRandomPoint(int level, List<Point> existingMines)
        {
            Point point = new();
            Random rand = new Random();

            while (true)
            {
                point.X = rand.Next(GAMEPAN[level, 0]);
                point.Y = rand.Next(GAMEPAN[level, 1]);

                if (!existingMines.Contains(point)) break;
            }

            return point;
        }
    }
}
