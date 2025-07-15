using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EchoClient
{
    internal class NetWork
    {
        readonly string[] COMMAND = { "exit", "make", "number", "remove" , "error" , "socket"};
        const string SERVER_IP = "127.0.0.1";
        const int SERVER_PORT = 9000;
        const int BUFSIZE = 512;

        const int COMMAND_START_IDX = 0;
        const int COMMAND_LENGTH = 10;

        const int SOCKET_NO_START_IDX = COMMAND_START_IDX + COMMAND_LENGTH;
        const int SOCKET_NO_LENGTH = 2;

        const int DATA_START_IDX = SOCKET_NO_START_IDX + SOCKET_NO_LENGTH;
        const int DATA_LENGTH = 512 - COMMAND_LENGTH - SOCKET_NO_LENGTH;

       
        Socket socketClient;
        int socketNo = -1;
        public NetWork()
        {
            
        }

        public void Connect()
        {
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect(SERVER_IP, SERVER_PORT);
                Form1 form = Application.OpenForms["Form1"] as Form1; 
                Thread thread = new Thread(() => ReceiveMessage());
                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[클라이언트] 예외 발생: {0}", ex.Message);
            }
        }
        // 송신
        public void SendMessage(string data, int No)
        {
            
            string strData = SetMessageFormat(data, No);
            byte[] msg = Encoding.UTF8.GetBytes(strData);
            socketClient.Send(msg);
        }

        string SetMessageFormat(string data , int No)
        {
            //"exit", "make", "number", "remove" , "error" , "socket"
            string[] split = data.Split(" ");

            if (isCommand(split[0])) return data;
            else
                return $"chat {No} {data} ";
            /*
            string sendData = split[0] switch
            {
                _ => $"chat {No} " + data
            };
            */
           
        }

        public void disConnect()
        {
            socketClient.Close();
        }
        public bool isConnect()
        {
            return socketClient.Connected;
        }
             //수신
        public void ReceiveMessage()
        {
            Form1 form = Application.OpenForms["Form1"] as Form1;
            if (form == null) return;
            while (true) { 
                byte[] recvBytes = new byte[BUFSIZE];
                int recvLen = 0;
                try
                {
                    recvLen = socketClient.Receive(recvBytes);
                }
                catch (SocketException ex)
                {
                    form.send2MainThread("서버가 비정상적으로 종료되었습니다.");
                    socketClient.Close();
                    break;
                }
                
                if(recvLen == 0)
                    break;

                string data = Encoding.Default.GetString(recvBytes);
                data = data.Trim('\0');

                form.send2MainThread(data);
               
                
                if (isExit(data))
                {
                    form.send2MainThread("정상적으로 서버와 연결이 종료되었습니다.");
                    break;
                }
            }
        }

 
        bool isExit(string data)
        {
            return data.Equals("exit");
        }

        bool isCommand(string msg)
        {
            foreach (string str in COMMAND)
            {
                if (msg.StartsWith(str)) return true;
            }
            return false;
        }

        public int GetSockNo()
        {
            return socketNo;
        }

        public void SetSockNo(int n)
        {
            socketNo = n;
        }
      
    }
}

