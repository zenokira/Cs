using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServer
{
    class Program
    {
        readonly int SERVERPORT = 9000;
        readonly int BUFSIZE = 512;

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

            Console.WriteLine($"[TCP 서버] {SERVERPORT}번 포트에서 대기 중...");

            while (true)
            {
                // 클라이언트 소켓 생성 및 스레드 생성
                Socket clientSock = listenSock.Accept();
                Console.WriteLine($"[TCP 서버] 클라이언트 접속: IP 주소={clientSock.RemoteEndPoint}");

                Thread thread = new Thread(() => ProcessClient(clientSock));
                thread.Start();
            }

            // 소켓 종료 및 리소스 반환
            listenSock.Close();
        }

        void ProcessClient(Socket clientSock)
        {
            try
            {
                // 클라이언트 소켓으로부터 데이터를 받아 처리하는 부분
                while (true)
                {
                    byte[] recvBytes = new byte[BUFSIZE];
                    int recvLen = clientSock.Receive(recvBytes);
                    if (recvLen == 0)
                    {
                        Console.WriteLine("[TCP 서버] 클라이언트 종료: IP 주소={0}", clientSock.RemoteEndPoint);
                        break;
                    }

                    string recvData = Encoding.UTF8.GetString(recvBytes, 0, recvLen);
                    Console.WriteLine("[TCP 서버] 받은 데이터: IP 주소={0}, 데이터={1}", clientSock.RemoteEndPoint, recvData);

                    byte[] sendData = Encoding.UTF8.GetBytes(recvData);
                    clientSock.Send(sendData);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"[TCP 서버] 소켓 예외 발생: {ex.Message}");
            }
            finally
            {
                clientSock.Close();
            }
        }
    }
}
