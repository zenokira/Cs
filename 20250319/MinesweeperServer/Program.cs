using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Emit;
using System.Drawing;
using System.Text;
using System.Numerics;

namespace MinesweeperServer
{   
    internal class Program
    {
        readonly string[] COMMAND = { "start", "wait", "retry", "exit", "click", "gameover" };
        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";
        
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 512;

        Dictionary<int, Socket> socket_list = new();
        List<int> socket_play_list = new();
        int cnt = 0;
        int serverCnt = -9999;
        int level = 0;
        Random rand = new Random();
        
        static void Main(string[] args)
        {
            (new Program()).start();
        }

        void start()
        {
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
                socket_list.Add(cnt, clientSock);
                Console.WriteLine($"[서버] 클라이언트 접속: IP 주소={clientSock.RemoteEndPoint}");

                socket_play_list.Add(cnt);

                Thread thread = new Thread(() => ProccessReceive(cnt++));
                thread.Start();

                if (socket_play_list.Count % 2 == 0 && socket_play_list.Count != 0)
                {
                    int idx = socket_play_list.Count - 2;
                    MatchingGame(socket_play_list[idx], socket_play_list[idx + 1]);
                }              

            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        void MatchingGame(int player1, int player2)
        {
            List<Point> mine = RandomLayingMine(0);
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

            Send(COMMAND[4], serverCnt, player1, $"{point.X}*{point.Y}");
            Send(COMMAND[4], serverCnt, player2, $"{point.X}*{point.Y}");

            Console.WriteLine($"player1 : {socket_list[player1]} player2 : {socket_list[player2]} Mine : {strMine}");
        }



        void ProccessReceive(int cnt)
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

                    recvData = Receive(cnt);

                    if (recvData.Length == 0) return;

                    if (!IsCorrectData(recvData)) return;
                    string eData = ExtractionData(recvData);
                    ParsingRecvData(eData, out command, out sendNo, out recvNo, out data);
                    
                    Console.WriteLine($"[recv] command : {command}, sendPlayer : {sendNo}, recvPlayer : {recvNo}, data : {data}");

                   
                    if (command.Equals(COMMAND[1]))
                    {
                        
                    }
                    else if (command.Equals(COMMAND[2]))
                    {
                       
                    }
                    else if (command.Equals(COMMAND[3]))
                    {
                        try
                        {
                            Send(COMMAND[3], sendNo, sendNo, data);
                        }
                        catch (SocketException e)
                        {
                            MatchDisconnect(cnt);
                        }

                        try
                        {
                            Send(COMMAND[3], sendNo, recvNo, data);
                        }
                        catch (SocketException e)
                        {
                            MatchDisconnect(cnt);
                        }
                    }
                    else if (command.Equals(COMMAND[4]))
                    {
                        Send(COMMAND[4], sendNo, recvNo, data);
                        Console.WriteLine($"[recv] sendPlayer : {sendNo}, click : {data}");
                    }
                    else if (command.Equals(COMMAND[5]))
                    {
                        Send(COMMAND[5], sendNo, recvNo, data);
                        Console.WriteLine($"[recv] gameoverPlayer : {sendNo}, result : {data}");
                    }
                }
                catch (SocketException e)
                {
                    
                }
            }
        }


        void MatchDisconnect(int cnt)
        {
            socket_play_list.Remove(cnt);
            
            socket_list[cnt].Close();

            socket_list.Remove(cnt);
          
        }
        string Receive(int cnt)
        {
            Socket socket = socket_list[cnt];
            byte[] recvBytes = new byte[BUFSIZE];
            int recvLen = socket.Receive(recvBytes);
            string recvData = Encoding.UTF8.GetString(recvBytes,0,recvLen);
            return recvData;
        }

        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string data)
        {
            try
            {
                string[] s = recvData.Split(" ");
                command = s[0];
                sendNo = Convert.ToInt32(s[1]);
                recvNo = Convert.ToInt32(s[2]);
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
    }

}


/*
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 */