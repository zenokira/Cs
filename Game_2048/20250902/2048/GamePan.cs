using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
    
    class GamePan
    {
        Point GamePan_StartPoint;
        Rectangle GamePan_Rect;
        Size GamePan_Size;
        Point[,] GamePan_Point = new Point[4, 4];

        public Rectangle col_Rect;
        public Rectangle row_Rect;
        

        NumberTile[,] numberTiles = new NumberTile[4, 4]; 
        public int GAMEPAN_GAP { get; } = 20;
        public Size SIZE { get; set; }
        public Point STARTPOINT { get; set; }
        public Rectangle RECT { get; set; }

        public int[] row = new int[5];
        public int[] col = new int[5];


        public GamePan ()
        {
            Initialize();
        }

        private void Initialize()
        {
            GamePan_StartPoint = new Point(42, 141);
            GamePan_Size = new Size(400, 400);
            GamePan_Rect = new Rectangle(new Point(0,0), GamePan_Size);

            SIZE = GamePan_Size;
            STARTPOINT = GamePan_StartPoint;
            RECT = GamePan_Rect;

            for (int i = 0; i < 5; i++)
            {
                row[i] = GAMEPAN_GAP * i + NumberTile.TILESIZE * i;
                col[i] = GAMEPAN_GAP * i + NumberTile.TILESIZE * i;
            }

            row_Rect = new Rectangle(0, 0, SIZE.Width, GAMEPAN_GAP);
            col_Rect = new Rectangle(0, 0, GAMEPAN_GAP, SIZE.Height);
        }

    }
}
