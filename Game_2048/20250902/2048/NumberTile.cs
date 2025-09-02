using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{

    class NumberTile
    {
        const int END_NUMBER = 2048;

        public const int TILESIZE = 75;
        private int Number { get; set; }

        private int X { get; set; }  // X 좌표  0 <= X <= 3
        private int Y { get; set ; } // Y 좌표  0 <= Y <= 3

        private Size SIZE { get; } = new Size(75,75);

        public void NextLevel()
        {
            Number *= 2;
        }
    }
}
