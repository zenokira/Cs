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

        public const int TILESIZE = 90;
        private int Number { get; set; }

        private int X { get; set; }  // X 좌표  0 <= X <= 3
        private int Y { get; set ; } // Y 좌표  0 <= Y <= 3

        private Size SIZE { get; } = new Size(90,90);

        public Bitmap Tile_Image;

        public NumberTile (int y, int x)
        {
            X = x; Y = y;
            Number = 2;
            string path = $"res\\{Number}.png";
            Tile_Image = new Bitmap(path);
        }
        public void NextLevel()
        {
            Number *= 2;
        }

        public void UpdateNumber(int newNumber)
        {
            Number = newNumber;
            string path = $"res\\{Number}.png";
            Tile_Image = new Bitmap(path);
        }


    }
}
