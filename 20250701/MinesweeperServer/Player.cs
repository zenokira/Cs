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
        }
    }
}
