using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCPClient
{
    class Program
    {
        // 서버 IP와 포트번호
        readonly string serverIp = "127.0.0.1";
        readonly int serverPort = 9000;
        static void Main(string[] args)
        {
            Program app = new Program();
            app.Start();
        }

        void Start()
        { 
            try
            {
                // 서버 연결
                TcpClient client = new TcpClient();
                client.Connect(serverIp, serverPort);

                NetworkStream stream = client.GetStream();
                Thread thread = new Thread(() => Receive(stream));
                thread.Start();

                // 서버와 데이터 통신
                while (true)
                {
                    SendMessage(stream);
                }

                // 연결 종료
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[TCP 클라이언트] 예외 발생: {0}", ex.Message);
            }
        }

        void SendMessage(NetworkStream stream)
        {
            int byteCount;
            Console.Write("\n[보낼 데이터] ");
            string data = Console.ReadLine();

            // '\n' 문자 제거
            data = data.TrimEnd('\n', '\r');
            if (string.IsNullOrEmpty(data))
                return;

            // 데이터 보내기
            byte[] sendBytes = Encoding.UTF8.GetBytes(data);
            stream.Write(sendBytes, 0, sendBytes.Length);
            Console.WriteLine("[TCP 클라이언트] {0}바이트를 보냈습니다.", sendBytes.Length);
        }

        void Receive(NetworkStream stream)
        {
            byte[] recvBytes = new byte[512];
            while (true) 
            {
                int byteCount = stream.Read(recvBytes, 0, recvBytes.Length);
                string data = Encoding.UTF8.GetString(recvBytes, 0, byteCount);
                Console.WriteLine("[TCP 클라이언트] {0}바이트를 받았습니다.", byteCount);
                Console.WriteLine("[받은 데이터] {0}", data);
            }
        }
    }
}
