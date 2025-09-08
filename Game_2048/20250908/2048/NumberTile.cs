using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{

    class NumberTile
    {
        const int CLEAR_NUMBER = 2048;

        public const int TILESIZE = 90;
        private Size SIZE { get; } = new Size(90, 90);
        private int Number { get; set; }

        public int X { get; set; }  // X 좌표  0 <= X <= 3
        public int Y { get; set ; } // Y 좌표  0 <= Y <= 3

        public bool IsNewTile;

        public Bitmap Tile_Image;
        public bool Changed { get; set; }

        public NumberTile (int y, int x, bool newTile)
        {
            X = x; Y = y;
            Number = 2;
            IsNewTile = newTile;

            string path;
            if (IsNewTile)
                path = $"res\\{Number}_highlight.png"; // 강조용 이미지
            else
                path = $"res\\{Number}.png";

            Tile_Image = new Bitmap(path);
        }
        public int NextLevel()
        {
            return Number * 2;
        }

        public void UpdateNumber(int newNumber)
        {
            Number = newNumber;

            // 색칠된 새 타일이면 다른 이미지 사용
            string path;
            if (IsNewTile && Number == 2)
                path = $"res\\{Number}_highlight.png"; // 강조용 이미지
            else
                path = $"res\\{Number}.png";

            Tile_Image = new Bitmap(path);
        }
        public void SetAsMoved()
        {
            if (IsNewTile)
            {
                IsNewTile = false;
                UpdateNumber(Number); // 강조 해제, 일반 이미지로 변경
            }
        }
        public int GetNumber()
        {
            return Number;
        }

        public bool IsGameClear()
        {
            return Number == CLEAR_NUMBER;
        }
    }
}
