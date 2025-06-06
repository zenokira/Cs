using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        readonly int SERVERPORT = 9000;
        readonly int BUFSIZE = 512;

        const int COMMAND_START_IDX = 0;
        const int COMMAND_LENGTH = 10;

        const int SOCKET_NO_START_IDX = COMMAND_START_IDX + COMMAND_LENGTH + 1;
        const int SOCKET_NO_LENGTH = 2;

        const int DATA_START_IDX = SOCKET_NO_START_IDX + SOCKET_NO_LENGTH + 1;
        const int DATA_LENGTH = 512 - COMMAND_LENGTH - SOCKET_NO_LENGTH;


        Dictionary<int, Socket> socket_list = new ();
        static int cnt = 0;
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

                Thread thread = new Thread(() => ProcessClient(cnt++));
                thread.Start();
            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        string Receive(int cnt)
        {
            Socket socket = socket_list[cnt];
            byte[] recvBytes = new byte[BUFSIZE];

            int recvLen = socket.Receive(recvBytes);
            string recvData = Encoding.UTF8.GetString(recvBytes, 0, recvLen);
            Console.WriteLine("[서버] 받은 데이터: IP 주소={0}, 데이터={1}", socket.RemoteEndPoint, recvData);
            return recvData;
        }

        void Send(int cnt, string msg)
        {
            string recvData = msg;
            byte[] sendData = Encoding.UTF8.GetBytes(recvData);
            socket_list[cnt].Send(sendData);
        }
        void ProcessClient(int cnt)
        {
            try
            {
                // 클라이언트 소켓으로부터 데이터를 받아 처리하는 부분
                while (true)
                {
                    string msg = Receive(cnt);
                    ParsingCommand(msg);
                    Send(cnt, msg);
                    if (msg.Equals("exit") || msg.Equals("종료"))
                    {
                        Console.WriteLine($"{socket_list[cnt].RemoteEndPoint} 연결 종료");
                        
                        break;
                    }
                    
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[서버] 소켓 예외 발생: {ex.Message}");
            }
            finally
            {
                socket_list[cnt].Close();
            }
        }

        void ParsingCommand(string _data)
        {
            string command = _data.Substring(0, COMMAND_LENGTH);
            string socketNo = _data.Substring(SOCKET_NO_START_IDX, SOCKET_NO_LENGTH);
            string data = _data.Substring(DATA_START_IDX, DATA_LENGTH);
        }
    }
}

/* Command NO 
 * exit 0 
 * make 0 button 333
 * 
 */ 




/*
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer
{
    internal class MainApp
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("사용법 : {0} <Bind IP>", Process.GetCurrentProcess().ProcessName);
                return;
            }

            string bindIp = args[0];
            const int bindPort = 5425;
            TcpListener server = null;

            try
            {
                IPEndPoint localAddress =
                    new IPEndPoint(IPAddress.Parse(bindIp), bindPort);

                server = new TcpListener(localAddress);

                server.Start();

                Console.WriteLine("메어리 서버 시작...");

                while(true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("클라이언트 접속 : {0} ", ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

                    NetworkStream stream = client.GetStream();

                    int length;
                    string data = null;
                    byte[] bytes = new byte[256];

                    while((length = stream.Read(bytes, 0, bytes.Length))!= 0)
                    {
                        data = Encoding.Default.GetString(bytes, 0, length);
                        Console.WriteLine(string.Format("수신: {0}", data));

                        byte[] msg = Encoding.Default.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine(String.Format("송신: {0}", data));
                    }
                    stream.Close();
                    client.Close();
                }
            }
            catch(SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("서버를 종료합니다.");
        }
    }
}
*/
