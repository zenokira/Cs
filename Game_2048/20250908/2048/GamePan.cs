using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
        

        NumberTile[,] numberTiles = new NumberTile[4, 4]; 
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

        public void GameReset()
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    numberTiles[y, x] = null;
                }
            }
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
                    numberTiles[y, x] = new NumberTile(y, x, true);
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

            for (int y = 0; y < numberTiles.GetLength(0); y++)
            {
                for (int x = 0; x < numberTiles.GetLength(1); x++)
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

        public void MoveTile(KeyEventArgs e, out bool moved)
        {
            moved = false;

            (int dx, int dy, Action moveAction)? action = e.KeyCode switch
            {
                Keys.Left => (-1, 0, (Action)MoveTilesLeft),
                Keys.Right => (1, 0, (Action)MoveTilesRight),
                Keys.Up => (0, -1, (Action)MoveTilesUp),
                Keys.Down => (0, 1, (Action)MoveTilesDown),
                _ => null
            };

            if (action.HasValue && IsMovePossible(action.Value.dx, action.Value.dy))
            {
                action.Value.moveAction();
                moved = true;
            }
        }
        // 왼쪽 이동
        public void MoveTilesLeft()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                List<NumberTile> tileList = new List<NumberTile>();
                for (int x = 0; x < cols; x++)
                {
                    if (numberTiles[y, x] != null)
                    {
                        numberTiles[y, x].Changed = false;
                        tileList.Add(numberTiles[y, x]);
                    }
                }

                MoveTileLine(tileList, cols, false);

                for (int x = 0; x < cols; x++)
                    numberTiles[y, x] = tileList[x];
            }
        }

        // 오른쪽 이동
        public void MoveTilesRight()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                List<NumberTile> tileList = new List<NumberTile>();
                for (int x = 0; x < cols; x++)
                {
                    if (numberTiles[y, x] != null)
                    {
                        numberTiles[y, x].Changed = false;
                        tileList.Add(numberTiles[y, x]);
                    }
                }

                MoveTileLine(tileList, cols, true);

                for (int x = 0; x < cols; x++)
                    numberTiles[y, x] = tileList[x];
            }
        }

        // 위쪽 이동
        public void MoveTilesUp()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int x = 0; x < cols; x++)
            {
                List<NumberTile> tileList = new List<NumberTile>();
                for (int y = 0; y < rows; y++)
                {
                    if (numberTiles[y, x] != null)
                    {
                        numberTiles[y, x].Changed = false;
                        tileList.Add(numberTiles[y, x]);
                    }
                }

                MoveTileLine(tileList, rows, false);

                for (int y = 0; y < rows; y++)
                    numberTiles[y, x] = tileList[y];
            }
        }

        // 아래쪽 이동
        public void MoveTilesDown()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int x = 0; x < cols; x++)
            {
                List<NumberTile> tileList = new List<NumberTile>();
                for (int y = 0; y < rows; y++)
                {
                    if (numberTiles[y, x] != null)
                    {
                        numberTiles[y, x].Changed = false;
                        tileList.Add(numberTiles[y, x]);
                    }
                }

                MoveTileLine(tileList, rows, true);

                for (int y = 0; y < rows; y++)
                    numberTiles[y, x] = tileList[y];
            }
        }

        private void MoveTileLine(List<NumberTile> tileList, int lineLength, bool reverse)
        {
            if (reverse) tileList.Reverse();

            // 병합 처리
            for (int i = 0; i < tileList.Count - 1; i++)
            {
                if (tileList[i].GetNumber() == tileList[i + 1].GetNumber() &&
                    !tileList[i].Changed && !tileList[i + 1].Changed)
                {
                    int num = tileList[i].NextLevel();
                    tileList[i].UpdateNumber(num);
                    tileList[i].Changed = true;
                    tileList.RemoveAt(i + 1);
                }
            }

            // 길이 맞추기
            while (tileList.Count < lineLength)
                tileList.Add(null);

            if (reverse) tileList.Reverse();
        }

        public bool IsGameOver()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            // 1. 빈 칸이 있는지 확인
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (numberTiles[y, x] == null)
                        return false; // 빈 칸이 있으면 게임 오버 아님
                }
            }

            // 2. 병합 가능한 타일이 있는지 확인
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int current = numberTiles[y, x].GetNumber();

                    // 오른쪽 타일 비교
                    if (x < cols - 1 && numberTiles[y, x + 1].GetNumber() == current)
                        return false;

                    // 아래 타일 비교
                    if (y < rows - 1 && numberTiles[y + 1, x].GetNumber() == current)
                        return false;
                }
            }

            // 빈 칸도 없고 병합도 불가능 → 게임 오버
            return true;
        }
        
        public bool IsGameClear()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)  // <= -> <
                {
                    if (numberTiles[y, x] == null) continue;
                    if (numberTiles[y, x].IsGameClear())
                        return true;
                }
            }

            return false;
        }

        public void ResetNewTiles()
        {
            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (numberTiles[y, x] != null)
                        numberTiles[y, x].SetAsMoved();
                }
            }
        }

        public bool IsMovePossible(int xdel, int ydel)
        {
            if (xdel == 0 && ydel == 0) return false;


            int rows = numberTiles.GetLength(0);
            int cols = numberTiles.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var current = numberTiles[y, x];
                    if (current == null) continue;

                    int nx = x + xdel;
                    int ny = y + ydel;

                    // 보드 밖으로 나가는 경우는 패스
                    if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;

                    var next = numberTiles[ny, nx];

                    // 이동할 자리가 비어있거나 같은 숫자면 이동 가능
                    if (next == null || next.GetNumber() == current.GetNumber())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

