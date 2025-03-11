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
        readonly string[] COMMAND = { "start", "click", "gameover" };
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        readonly int SERVERPORT = 9900;
        readonly int BUFSIZE = 512;
        Dictionary<int, Socket> socket_list = new();
        List<int> socket_play_list = new();
        int cnt = 0;

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


                Thread thread = new Thread(() => ProccessReceive(cnt++));
                thread.Start();
                
            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        void MatchingGame()
        {
            if( socket_list.Count - socket_play_list.Count < 2) return;
            int playerCnt = 0;
            List<int> player = new();

            foreach ( int i in socket_list.Keys)
            {
                if (socket_play_list.Contains(i)) continue;

                player.Add(i);
                playerCnt++;
                if (playerCnt == 2)        break;
            }

            if (playerCnt != 2) return;

            Thread thread = new Thread(() => PlayGame(player));
            thread.Start();

            foreach (int i in player)
               socket_play_list.Add(i);
        }

        void PlayGame(List<int> player)
        {
            List<Point> mine = RandomLayingMine(0);
            string strMine = MineConvertString(mine);
            
            Send(COMMAND[0], player[0], player[1], strMine);
            Send(COMMAND[0], player[1], player[0], strMine);


            int playerTime = 0;
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
                    string command, sendNo, recvNo, data;
                    foreach (int i in player)
                    {
                        recvData = Receive(socket_list[i]);
                        Console.WriteLine($"[recv] {recvData}");
                        if (recvData != "") break;
                    }
                    ParsingRecvData(recvData, out command, out sendNo, out recvNo, out data);

                    if (command.Equals(COMMAND[2]))
                    {

                    }
                    else if (command.Equals(COMMAND[1]))
                    {
                        int send_n = Convert.ToInt32(sendNo);
                        int recv_n = Convert.ToInt32(recvNo);
                        if (sendNo.Equals(recvNo))
                        {
                            Send(COMMAND[1], send_n, send_n, data);
                            Send(COMMAND[1], send_n, recv_n, data);
                        }
                    }

                }
                catch (SocketException e)
                {

                }
            }
        }
        string Receive(Socket socket)
        {
            byte[] recvBytes = new byte[BUFSIZE];
            int recvLen = socket.Receive(recvBytes);
            string recvData = Encoding.UTF8.GetString(recvBytes,0,recvLen);
            return recvData;
        }

        bool ParsingRecvData(string recvData, out string command, out string sendNo, out string recvNo, out string data)
        {
            string[] s = recvData.Split(" ");
            command = s[0];
            sendNo = s[1];
            recvNo = s[2];
            data = s[3];

            return true;
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
            string sendData = $"{command} {sendNo} {recvNo} {data}";
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