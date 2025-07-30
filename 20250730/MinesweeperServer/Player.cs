using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperServer
{
    public class Player
    {
        public int Id { get; private set; }
        public Socket Player_Socket { get; private set; }
        public PlayerState State { get; set; }
        public bool HasClicked { get; set; }
        public int ClearTime { get; set; } = -1;

        public bool ExitReserved { get; set; } = false;
        public bool LeaveReserved { get; set; } = false;
        public Player(int id, Socket sock)
        {
            Id = id;
            Player_Socket = sock;
            State = PlayerState.Alive;
            HasClicked = false;
        }

        public void Reset()
        {
            State = PlayerState.Alive;
            HasClicked = false;
            LeaveReserved = false;
        }

        public int IsReservation()
        {
            if (LeaveReserved) { return 1; }
            else if (ExitReserved) { return 2; }
            else { return 0; }
        }

        public bool DisconnectPlayer()
        {
            if (Player_Socket != null)
            {
                try
                {
                    Player_Socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException) { }  // 이미 끊겼으면 무시
                catch (ObjectDisposedException) { }

                try { Player_Socket.Close(); } catch { return false; }
                try { Player_Socket.Dispose(); } catch { return false; }

                Console.WriteLine($"플레이어 {Id}의 소켓 연결을 서버에서 정리했습니다.");
                return true;
            }
            return false;
        }
    }
}
