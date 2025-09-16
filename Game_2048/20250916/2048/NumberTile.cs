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
        
        private int Number;


        public int X { get; set; }  // X 좌표  0 <= X <= 3
        public int Y { get; set ; } // Y 좌표  0 <= Y <= 3

        public bool IsNewTile;
        public bool Changed { get; set; }

        public NumberTile (int y, int x, bool newTile)
        {
            X = x; 
            Y = y;
            Number = 2;
            IsNewTile = newTile;
        }
        public int NextLevel() => Number * 2;
    
        public void SetAsMoved()
        {
            if (IsNewTile)
            {
                IsNewTile = false;
                Number = NextLevel(); 
            }
        }
        public int GetNumber() => Number;
        public void SetNumber(int num) => Number = num;

        public bool IsGameClear() => Number == CLEAR_NUMBER;

        public Bitmap DrawTile()
        {
            Bitmap bmp = new Bitmap(TILESIZE, TILESIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // 1. 배경색 결정
                Color bgColor = GetTileBackColor(IsNewTile,Number);

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, 0, 0, TILESIZE, TILESIZE);
                }

                // 2. 숫자 색상
                Color textColor = GetTileTextColor(Number);
                using (Brush textBrush = new SolidBrush(textColor))
                using (Font font = new Font("맑은 고딕", 24, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    // 중앙 정렬
                    StringFormat sf = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(Number.ToString(), font, textBrush,
                                 new RectangleF(0, 0, TILESIZE, TILESIZE), sf);
                }
            }
            return bmp;
        }
        private Color GetTileTextColor(int number)
        {
            return number switch
            {
                2 => Color.Black,
                4 => Color.Green,
                8 => Color.Orange,
                16 => Color.OrangeRed,
                32 => Color.Red,
                64 => Color.DarkRed,
                128 => Color.Purple,
                256 => Color.Tomato,
                512 => Color.Blue,
                1024 => Color.DodgerBlue,
                2048 => Color.SteelBlue,
                _ => Color.Gray,
            };
        }



        private Color GetTileBackColor(bool newTile, int numbuer)
        {
            if (newTile) return Color.Gold;
            else return Color.Wheat;
            
        }
    }
}
