using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    class Program
    {
        readonly string[] COMMAND = { "exit", "make", "number", "remove", "error" };
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
            listenSock.Listen(20);

            Console.WriteLine($"[서버] {SERVERPORT}번 포트에서 대기 중...");

            while (true)
            {
                // 클라이언트 소켓 생성 및 스레드 생성
                Socket clientSock = listenSock.Accept();
                socket_list.Add(cnt, clientSock);
                SendAllMakeButton(cnt);
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

            string[] sliceData = new string[3];
            ParsingData(recvData, out sliceData[0], out sliceData[1], out sliceData[2]);
           
            Console.WriteLine("[서버] 받은 데이터: IP 주소={0}, 커맨드={1}, 소켓={2}, 데이터={3}",
                    socket.RemoteEndPoint, sliceData[0], sliceData[1], sliceData[2]);

            return recvData;
        }

        void Send(string data, int cnt)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(data);
            socket_list[cnt].Send(sendData);
        }

        void errorSend(string data, int cnt)
        {
            string errorString = $"error {cnt} {data} ";
            byte[] sendData = Encoding.UTF8.GetBytes(errorString);
            socket_list[cnt].Send(sendData);
        }

        
        void ProcessClient(int cnt)
        {
            string numbering = $"number {cnt} ";
            Send(numbering, cnt);
            try
            {
                SendMakeButton(cnt);
                // 클라이언트 소켓으로부터 데이터를 받아 처리하는 부분
                while (true)
                {
                    string recvData = Receive(cnt);
                    string command = "";
                    string sock = "";
                    string msg = "";
                    if (ParsingData(recvData, out command, out sock, out msg))
                    {
                        int n = Convert.ToInt32(sock);
                        if (socket_list.ContainsKey(n))
                        {
                            Send(recvData, n);
                        }
                        else
                        {
                            errorSend("잘못된 경로로 메시지를 보내셨습니다", cnt);
                        }
                    }
                    
                   

                    if (isWhatCommand(command) == 0)
                    {
                        Send(recvData, cnt);
                        Console.WriteLine($"[서버] 클라이언트 종료: IP 주소={socket_list[cnt].RemoteEndPoint}");
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
        bool ParsingData(string input, out string command, out string dst, out string msg)
        {
            string[] splitString = input.Split(" ");
            dst = msg = "";
          
            command = splitString[0];
            if (splitString.Length < 2) return false;
            dst = splitString[1];

            if (splitString.Length < 3) return false;

            msg = ParsingMessage(input, splitString[2]);
          
            return true;
        }

        string ParsingMessage(string data, string start)
        {
            if (start.Equals("")) return "";
            string splitString = data.Substring(data.IndexOf(start));
            return splitString;
        }

 
        void SendMakeButton(int cnt)
        {
            foreach(int i in socket_list.Keys)
            { 
                if (cnt == i) continue;
                string sendData = $"make {cnt} button {i} ";
                Console.WriteLine($"MakeButton : {sendData}");
                Send(sendData, cnt);
            }
        }
        void SendAllMakeButton(int cnt)
        {
            foreach(var pair in socket_list)
            {
                if (pair.Key == cnt) continue;
                
                string sendData = $"make {pair.Key} button {cnt} ";
                Console.WriteLine($"AllMakeButton : {sendData}");
                Send(sendData, pair.Key);

            }
        }
    }
}

/* Command NO 
 * exit 0 
 * make 0 button 333
 * number 0 
 * error 0 msg
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