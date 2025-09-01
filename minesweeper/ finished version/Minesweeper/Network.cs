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
    public class Network
    {
        const string SERVER_IP = "127.0.0.1";
        const int SERVER_PORT = 9900;
        readonly int BUFSIZE = 1024;

        Socket socketClient;

        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";


        bool isReceiving = false;
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
                socketClient.ReceiveTimeout = 500;
                isReceiving = true;

                Thread thread = new Thread(() => ReceiveMessage()) { IsBackground = true };
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
            isReceiving = false;

            if (socketClient == null) return;

            try { socketClient.Shutdown(SocketShutdown.Both); } catch { }
            try { socketClient.Close(); } catch { }
            try { socketClient.Dispose(); } catch { }

            socketClient = null;
        }

        void ReceiveMessage()
        {
            지뢰찾기 form = Application.OpenForms["지뢰찾기"] as 지뢰찾기;
            if (form == null) return;


            while (isReceiving)
            {
                if (socketClient == null || !socketClient.Connected)
                {
                    // 소켓이 끊기면 루프 종료
                    break;
                }

                byte[] recvBytes = new byte[BUFSIZE];
                int recvLen = 0;
                try
                {
                    recvLen = socketClient.Receive(recvBytes);
                    if (recvLen <= 0) break;
                    string recvData = Encoding.Default.GetString(recvBytes);
                    recvData = recvData.Trim('\0');
                    form.net2MainThread(recvData);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.TimedOut) continue;
                }
                catch (ObjectDisposedException ex)
                {
                    // 소켓이 이미 닫혔다면 그냥 종료
                    break;
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"ReceiveMessage 예외: {ex}");
                    isReceiving = false;
                    break;
                }
            }
        }
    }
}
