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

        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";
        
        public Network()
        {

        }
        public void Dispose()
        {
            if (socketClient.Connected) {  socketClient.Close(); }
            socketClient.Dispose();
            socketClient = null;
        }

        public bool IsSocketNULL()
        {
            return socketClient == null;
        }

        public bool IsConnected()
        {
            return socketClient.IsBound;
        }

        public bool Connect()
        {
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect(SERVER_IP, SERVER_PORT);
                Thread thread = new Thread(() => ReceiveMessage());
                thread.Start();
                return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
        }

        public void SendMessage(string command, int sendNo, int recvNo, string data)
        {
            string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {data}{END_WORD}";
            byte[] sendMsg = Encoding.UTF8.GetBytes(sendData);
            socketClient.Send(sendMsg);
        }

        public void Disconnect()
        {
            socketClient.Close();
            socketClient.Dispose();
            socketClient = null;
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
