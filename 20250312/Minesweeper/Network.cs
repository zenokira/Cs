using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace Minesweeper
{
    internal class Network
    {
        const string SERVER_IP = "127.0.0.1";
        const int SERVER_PORT = 9900;

        Socket socketClient;
        
        public Network()
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
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            byte[] sendMsg = Encoding.UTF8.GetBytes(message);
            socketClient.Send(sendMsg);
        }

        public void Disconnect()
        {
            socketClient.Close();
        }

        void ReceiveMessage()
        {
            지뢰찾기 form = Application.OpenForms["지뢰찾기"] as 지뢰찾기;
            if (form == null) return;

            while (true)
            {
                byte[] recvBytes = new byte[512];
                int recvLen = 0;
                try
                {
                    recvLen = socketClient.Receive(recvBytes);
                    string recvData = Encoding.Default.GetString(recvBytes);
                    recvData = recvData.Trim('\0');
                    form.net2MainThread(recvData);
                }
                catch (SocketException ex)
                {
                    form.net2MainThread("서버가 비정상적으로 종료되었습니다.");
                    socketClient.Close();
                    break;
                }
            }
        }
    }
}
