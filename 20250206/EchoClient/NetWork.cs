using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    internal class NetWork
    {
        const string SERVER_IP = "127.0.0.1";
        const int SERVER_PORT = 9000;
        const int BUFSIZE = 1024;
        Socket socketClient;
        public NetWork()
        {
            
        }

        public void Connect()
        {
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect(SERVER_IP, SERVER_PORT);
               
                Thread thread = new Thread(() => ReceiveMessage());
                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[TCP 클라이언트] 예외 발생: {0}", ex.Message);
            }
        }
        // 송신
        public void SendMessage(string msg)
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(msg);
            socketClient.Send(sendBytes);
        }

        //수신
        public void ReceiveMessage()
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            if (form == null) return;
            while (true) { 
                byte[] recvBytes = new byte[BUFSIZE];
               
                int recvLen = socketClient.Receive(recvBytes);
                if(recvLen == 0) break;
                string data = Encoding.Default.GetString(recvBytes);

                form.send2MainThread(data);

                if (data.Equals("exit") || data.Equals("종료"))
                    break;
            }

            
        }
    }
}

