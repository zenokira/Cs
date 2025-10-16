using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _2048
{

    class NumberTile
    {
        static readonly int[] NUMBERING;
        const int CLEAR_NUMBER = 2048;

        public const int TILESIZE = 90;

        private int Numbering_idx;

        public int X { get; set; }  // X 좌표  0 <= X <= 3
        public int Y { get; set ; } // Y 좌표  0 <= Y <= 3

        public bool IsNewTile;
        public bool Changed { get; set; }

        static NumberTile()
        {
            NUMBERING = new int[] { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };
        }
        public NumberTile (int y, int x, bool newTile, int Numbering_idx = 0)
        {
            X = x; 
            Y = y;
            IsNewTile = newTile;
            this.Numbering_idx = Numbering_idx;
        }
        public void NextLevel() {
            Numbering_idx++;
        } 
    
        public void SetAsMoved()
        {
            if (IsNewTile)
            {
                IsNewTile = false;
            }
        }
        public bool IsGameClear() => NUMBERING[Numbering_idx] == CLEAR_NUMBER;
        public int GetNumber() => NUMBERING[Numbering_idx];

        public Bitmap DrawTile()
        {
            Bitmap bmp = new Bitmap(TILESIZE, TILESIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // 1. 배경색 결정
                Color bgColor = GetTileBackColor(IsNewTile, NUMBERING[Numbering_idx]);

                using (SolidBrush brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, 0, 0, TILESIZE, TILESIZE);
                }

                // 2. 숫자 색상
                Color textColor = GetTileTextColor(NUMBERING[Numbering_idx]);
                using (Brush textBrush = new SolidBrush(textColor))
                using (Font font = new Font("맑은 고딕", 24, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    // 중앙 정렬
                    StringFormat sf = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(NUMBERING[Numbering_idx].ToString(), font, textBrush,
                                 new RectangleF(0, 0, TILESIZE, TILESIZE), sf);
                }
            }
            return bmp;
        }
        private Color GetTileTextColor(int number)
        {
            return number switch
            {
                /*
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
                */
                _ => Color.Black,
            };
        }



        private Color GetTileBackColor(bool newTile, int number)
        {
            if (newTile) return Color.Goldenrod;

            return number switch
            {
                2 => Color.LightSkyBlue,    // 연한 파랑
                4 => Color.LightGreen,      // 연한 초록
                8 => Color.LightSalmon,     // 연한 주황/살몬
                16 => Color.Orange,      // 밝은 오렌지톤
                32 => Color.Khaki,          // 노랑 계열
                64 => Color.Tomato,         // 진한 빨강
                128 => Color.Plum,          // 보라
                256 => Color.CornflowerBlue,// 파랑
                512 => Color.MediumSeaGreen,// 초록
                1024 => Color.Gold,         // 금색
                2048 => Color.OrangeRed,    // 강한 주황/빨강
                _ => Color.Wheat,
            };

        }
    }
}
