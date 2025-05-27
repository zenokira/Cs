using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperServer
{
    class Room
    {
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };
        public int RoomId { get; set; }
        public int Player1_Id { get; set; }
        public int Player2_Id { get; set; }
        
        public int First_Player { get; set; }

        public bool Player1_RetryFlag = false;
        public bool Player2_RetryFlag = false;
        public int Level { get; set; }
        public List<Point> MineMap { get; set; }
        public Point FirstClick { get; set; }

        Random rand = new Random();
        public Room(int roomId, int p1, int p2, int level)
        {
            RoomId = roomId;
            Player1_Id = p1;
            Player2_Id = p2;
            Level = level;
            
            MineMap = new List<Point>();
        }

        public void InitGame()
        {
            SetFirstClickPoint();
            SetRandomMinePosition();
        }

        private void SetRandomMinePosition()
        {
            MineMap.Clear();
            for (int i = 0; i < MINE_COUNT[Level];)
            {
                int n = rand.Next(GAMEPAN[Level, 0] * GAMEPAN[Level, 1]);

                int y = n / GAMEPAN[Level, 0];
                int x = n % GAMEPAN[Level, 0];

                Point pt = new Point(x, y);

                if (FirstClick.Equals(pt)) continue;
                if (MineMap.Contains(pt)) continue;

                MineMap.Add(pt);
                i++;
            }  
        }

        private void SetFirstClickPoint()
        {
            int n = rand.Next(GAMEPAN[Level, 0] * GAMEPAN[Level, 1]);

            int y = n / GAMEPAN[Level, 0];
            int x = n % GAMEPAN[Level, 0];

            FirstClick = new Point(x, y);
        }

        public int GetOppPlayer(int player)
        {
            return player == Player1_Id? Player2_Id : Player1_Id;
        }

        public bool IsGameRetry()
        {
            return Player1_RetryFlag && Player2_RetryFlag;
        }

        public void SetRetryFlag(int player)
        {
            if (Player1_Id == player) Player1_RetryFlag = true;
            else if (Player2_Id == player) Player2_RetryFlag = true;
        }

        public string MineConvertString()
        {
            string data = "";

            foreach (Point p in MineMap)
            {
                data += $"{p.X.ToString()}*{p.Y.ToString()} ";
            }

            return data;
        }
    }
}
