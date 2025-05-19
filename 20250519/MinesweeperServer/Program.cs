using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Text;
using System.Reflection.Metadata.Ecma335;


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
            int p1 = IsGamePlay(player1);
            int p2 = IsGamePlay(player2);
            int level = -1;
            if (p1 == p2 && (p1 != -1 && p2 != -1)) level = p1;
            if (level == -1) return;

            List<Point> mine = RandomLayingMine(level);
            string strMine = MineConvertString(mine);

            Send(COMMAND[0], player1, player2, strMine);
            Send(COMMAND[0], player2, player1, strMine);

            Point point = new();
            while(true)
            {
                point.X = rand.Next(GAMEPAN[level, 0]);
                point.Y = rand.Next(GAMEPAN[level, 1]);

                if (!mine.Contains(point)) break;
            }

            Send(COMMAND[4], serverNo, player1, $"{point.X}*{point.Y}");
            Send(COMMAND[4], serverNo, player2, $"{point.X}*{point.Y}");

            Console.WriteLine($"[LOG] player1 : {player1} player2 : {player2} Mine : {strMine}");
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

                        if (command.Equals(COMMAND[1]))
                        {
                            search4correctwaitingroom(data, count);
                            LogSocketCount();
                        }
                        else if (command.Equals(COMMAND[2]))
                        {
                            int result = IsGamePlay(sendNo);
                            if ( result == -1 ) Send(COMMAND[6], serverNo, sendNo, "error");
                            socket_play_level_list[result].Remove(sendNo);
                            socket_wait_level_list[result].Add(sendNo);
                        }
                        else if (command.Equals(COMMAND[3]))
                        {
                            LogSocketCount();

                            int result = IsGamePlay(sendNo);

                            if (result == -1) break;

                            while (!RemovePlayListItem(sendNo, result)) ;

                            LogSocketCount();


                            socket_list[count].Shutdown(SocketShutdown.Both);
                            socket_list[count].Close();
                            socket_list[count].Dispose();

                            socket_list.Remove(count);

                            Console.WriteLine($"[LOG] socket_list : {socket_list.Count}");
                            break;
                        }
                        else if (command.Equals(COMMAND[4]))
                        {
                            Send(COMMAND[4], sendNo, recvNo, data);
                        }
                        else if (command.Equals(COMMAND[5]))
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
                        else if (command.Equals(COMMAND[6]))
                        {
                            Send(COMMAND[6], serverNo, count, count.ToString());
                        }
                    }
                    processedMessages.Clear();
                }
                catch (SocketException e)
                {
                    
                }
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
            string recvData = Encoding.UTF8.GetString(recvBytes,0,recvLen);
            return recvData;
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



        List<Point> RandomLayingMine(int level)
        {
            List <Point> mine = new List<Point>();

            for (int i = 0; i < MINE_COUNT[level];)
            {
                int n = rand.Next(GAMEPAN[level,0] * GAMEPAN[level, 1]);

                int y = n / GAMEPAN[level, 0];
                int x = n % GAMEPAN[level, 0];
                
                Point pt = new Point(x, y);

                if (mine.Contains(pt)) continue;
                mine.Add(pt);
                i++;
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
            string data = "";

            foreach (Point p in mine)
            {
                data += $"{p.X.ToString()}*{p.Y.ToString()} "; 
            }

            return data;
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
            int i = 0;
            foreach ( var li in socket_play_level_list)
            {
                if (li.Contains(socketNo)) return i;
                i++;
            }
            
            return -1;
        }

        bool RemovePlayListItem(int socketNo, int idx)
        {
            bool flag = false;
            socket_play_level_list[idx].Remove(socketNo);
            if (!socket_play_level_list[idx].Contains(socketNo)) flag = true;

            return flag;
        }
    }

}


/*
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 */