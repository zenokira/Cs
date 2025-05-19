using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperServer
{
    internal class Room
    {
        int player1, player2;
        Dictionary<int, Socket> dic;
        public Room(KeyValuePair<int, Socket> p1, KeyValuePair<int, Socket> p2)
        {
            dic[p1.Key] = p1.Value;
            dic[p2.Key] = p2.Value;
            player1 = p1.Key;
            player2 = p2.Key;
        }
    }
}
