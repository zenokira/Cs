using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        readonly string[] COMMAND = { "exit", "make" };
        readonly int SERVERPORT = 9000;
        readonly int BUFSIZE = 512;

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

            if (isWhatCommand(recvData) == 0)
            {
                Console.WriteLine("[서버] 받은 데이터: IP 주소={0}, 커맨드={1}, 소켓={2}, 데이터={3}",
                    socket.RemoteEndPoint, ParsingData(recvData, 0), cnt, "");
            }
            else
            {
                Console.WriteLine("[서버] 받은 데이터: IP 주소={0}, 커맨드={1}, 소켓={2}, 데이터={3}",
                    socket.RemoteEndPoint, ParsingData(recvData, 0), ParsingData(recvData, 1), ParsingData(recvData, 2));
            }
            return recvData;
        }

        void Send(string data, int cnt)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(data);
            socket_list[cnt].Send(sendData);
        }

        
        void ProcessClient(int cnt)
        {
            try
            {
                // 클라이언트 소켓으로부터 데이터를 받아 처리하는 부분
                while (true)
                {
                    string recvData = Receive(cnt);
                  
                    Send(recvData, cnt);
                    if (isWhatCommand(ParsingData(recvData,0)) == 0)
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
                socket_list.Remove(cnt);
            }
        }

        int isWhatCommand(string command)
        {
            for(int i = 0; i < COMMAND.Length; i++)
            {
                if (COMMAND[i].Equals(command)) return i;
            }

            return -1;
        }

        string ParsingData(string data, int n)
        {
            string[] splitString = data.Split(" ");

            string str = n switch
            {
                0 => ParsingCommand(splitString),
                1 => ParsingSockNo(splitString),
                2 => ParsingMessage(splitString)
            };

            return str;
        }

        string ParsingCommand(string[] data)
        {
            return data[0];
        }

        string ParsingSockNo(string[] data)
        {
            return data[1];
        }

        string ParsingMessage(string[] data)
        {
            return ConcatString(data);
        }

        string ConcatString(string[] data)
        {
            string str = "";
            for (int i = 2; i < data.Length; i++)
            {
                str += data[i];
                if (i != data.Length - 1) str += " ";
            }
            return str;
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