﻿using System;
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
        readonly int BUFSIZE = 1024;

        Socket socketClient;

        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";
        public Network()
        {

        }
        public void Dispose()
        {
            Disconnect();
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

        public bool IsReallyConnected()
        {
            if (socketClient == null)
                return false;
            try
            {
                return !(socketClient.Poll(1, SelectMode.SelectRead) && socketClient.Available == 0);
            }
            catch (ObjectDisposedException ode)
            {
                // 소켓이 이미 Dispose된 상태
                return false;
            }
            catch (SocketException se)
            {
                return false;
            }
        }

        public void SendMessage<T>(CommandType command, int sendNo, int recvNo, T action, string data, string func) where T : Enum
        {
            string str_action = action.ToString();
            Console.WriteLine($"{func} 에서 호출됨");
            string sendData = $"{START_WORD}{command} {sendNo} {recvNo} {str_action} {data}{END_WORD}";
            byte[] sendMsg = Encoding.UTF8.GetBytes(sendData);
            socketClient.Send(sendMsg);
        }

        public void Disconnect()
        {
            if ( IsConnected () ) socketClient.Close();
            if (IsReallyConnected() )socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Dispose();
            socketClient = null;
        }

        void ReceiveMessage()
        {
            지뢰찾기 form = Application.OpenForms["지뢰찾기"] as 지뢰찾기;
            if (form == null) return;

            while (true)
            {
                byte[] recvBytes = new byte[BUFSIZE];
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
