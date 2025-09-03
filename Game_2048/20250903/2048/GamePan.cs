using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace _2048
{
    
    class GamePan
    {
        const int MAX_TILE_CNT = 16;
        Point GamePan_StartPoint;
        Rectangle GamePan_Rect;
        Size GamePan_Size;
        Point[,] Tile_Point = new Point[4, 4];

        public Rectangle col_Rect;
        public Rectangle row_Rect;
        

        NumberTile[,]? numberTiles = new NumberTile[4, 4]; 
        public int GAMEPAN_GAP { get; } = 10;
        public Size SIZE { get; set; }
        public Point STARTPOINT { get; set; }
        public Rectangle RECT { get; set; }

        public int[] row = new int[5];
        public int[] col = new int[5];
        Random random = new Random();

        Bitmap backBmp;

        public GamePan ()
        {
            Initialize();
        }

        private void Initialize()
        {
            GamePan_StartPoint = new Point(42, 141);
            GamePan_Size = new Size(410, 410);
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

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Tile_Point[y, x] = GetTileStartPoint(y, x);
                }
            }


            ReadyToBackImage();
        }

        private Point GetTileStartPoint(int y, int x)
        {
            return new Point(
                        GAMEPAN_GAP * (1 + x) + NumberTile.TILESIZE * x,
                        GAMEPAN_GAP * (1 + y) + NumberTile.TILESIZE * y
                        );
        }

        private void ReadyToBackImage()
        {
            backBmp = new Bitmap(GamePan_Size.Width, GamePan_Size.Height);
            using (Graphics g = Graphics.FromImage(backBmp))
            using (SolidBrush bgBrush = new SolidBrush(Color.Ivory))
            using (SolidBrush lineBrush = new SolidBrush(Color.BlueViolet))
            {
                g.FillRectangle(bgBrush, 0, 0, SIZE.Width, SIZE.Height);

                for (int i = 0; i < 5; i++)
                {
                    g.FillRectangle(lineBrush, 0, row[i], row_Rect.Width, row_Rect.Height);
                    g.FillRectangle(lineBrush, col[i], 0, col_Rect.Width, col_Rect.Height);
                }
            }
        }

        public void NewTile()
        {
            if (GetTilesCount() == MAX_TILE_CNT) return;

            bool flag = true;
            while (flag)
            {
                int rand = random.Next(16);
            
                int x, y;

                if (rand == 0) { y = 0; x = 0; }
                else
                {
                    y = rand / 4;
                    x = rand % 4;
                }

                if ( numberTiles[y,x] == null )
                {
                    numberTiles[y, x] = new NumberTile(y, x);
                    flag = false;
                }
            }
        }

        private int GetTilesCount()
        {
            int Count = 0;
            for (int i = 0; i < numberTiles.GetLength(0); i++)
            {
                for (int j = 0; j < numberTiles.GetLength(1); j++)
                {
                    if (numberTiles[i, j] != null)
                        Count++;
                }
            }

            return Count;
        }

        public void DrawTile(Graphics g)
        {
            if (numberTiles == null ) return;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    NumberTile tile = numberTiles[y, x];
                    if (tile != null && tile.Tile_Image != null)
                    {
                        // 타일 좌표 계산 (게임판 시작점 + 간격 + 타일 크기)
                        int drawX = Tile_Point[y,x].X;
                        int drawY = Tile_Point[y, x].Y;

                        g.DrawImage(tile.Tile_Image, drawX, drawY, NumberTile.TILESIZE, NumberTile.TILESIZE);
                    }
                }
            }
        }

        public Bitmap DrawBoard()
        {
            Bitmap bmp = new Bitmap(SIZE.Width, SIZE.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(backBmp, 0, 0); // 배경 먼저 복사
                DrawTile(g); // 기존 DrawTile 로직 그대로 사용
            }
            return bmp;
        }
    }
}
