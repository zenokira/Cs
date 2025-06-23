using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{

    internal class GameManager
    {

        private readonly int buttonSize = 30;
        private readonly int buttonStartX = 20;
        private readonly int buttonStartY = 60;

        public int ButtonSize => buttonSize;
        public int ButtonStartX => buttonStartX;
        public int ButtonStartY => buttonStartY;

        readonly string[] MINE_FLAG = { " ", "¶", "?" };
        const int WAYS = 8;
        readonly int[,] WAY = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 }, { 1, 1 } };
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        public IReadOnlyList<int> MineCount => MINE_COUNT;
        public IReadOnlyList<int> MineFlagCount => MINE_FLAG_COUNT;

        public int MINE_FLAG_CNT { get; set; }


        Mine[,] ?buttons;
        public int SOLO_LEVEL { get; set; } = 0;
        public int MULTI_LEVEL { get; set; } = 0;

        Random rand = new Random();

        public GameManager()
        {

        }

        public void SoloGameInit()
        {
            RandomLayingMine(SOLO_LEVEL);
            DetectorMine(SOLO_LEVEL);
        }
        public void MultiGameInit()
        {
            DetectorMine(MULTI_LEVEL);
        }
        public void SetButtons(Mine[,] buttons)
        {
            this.buttons = buttons;
        }

        public void ClearButtons()
        {
            this.buttons = null;
        }

        void RandomLayingMine(int level)
        {
            for (int i = 0; i < MineCount[level];)
            {
                int n = rand.Next(buttons.Length);

                var (rows, cols) = GetGamePanSize(level);
                int y = n / rows;
                int x = n % rows;

                if (buttons[y, x].isMineHave()) continue;
                buttons[y, x].LayingMine();
                i++;
            }
        }

        public (int Rows, int Cols) GetGamePanSize(int levelIndex)
        {
            return (GAMEPAN[levelIndex, 0], GAMEPAN[levelIndex, 1]);
        }



        void DetectorMine(int level)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int x = i % GAMEPAN[level, 0];
                int y = i / GAMEPAN[level, 0];
                for (int w = 0; w < WAYS; w++)
                {
                    int nx = x + WAY[w, 0], ny = y + WAY[w, 1];
                    if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                    Mine btn = buttons[ny, nx];
                    if (btn.isMineHave()) buttons[y, x].AroundMineCountPlus();
                }
            }
        }

        // GameManager 내부
        public List<Point> GetButtonsToOpenByZeroDetector(Point start, bool isSolo)
        {

            int level = -1;

            if (isSolo)
                level = SOLO_LEVEL;
            else
                level = MULTI_LEVEL;

            HashSet<Point> visited = new();
            Queue<Point> queue = new();
            List<Point> toOpen = new();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                toOpen.Add(p);

                for (int i = 0; i < WAYS; i++)
                {
                    int nx = p.X + WAY[i, 0], ny = p.Y + WAY[i, 1];
                    if (nx < 0 || ny < 0 || nx >= GAMEPAN[level, 0] || ny >= GAMEPAN[level, 1]) continue;

                    Point np = new(nx, ny);
                    if (visited.Contains(np)) continue;
                    visited.Add(np);

                    Mine btn = buttons[ny, nx];
                    if (btn.isMineHave()) continue;
                    if (btn.isZeroCount())
                    {
                        queue.Enqueue(np);
                    }
                    else
                    {
                        toOpen.Add(np);
                    }
                }
            }

            return toOpen;
        }

        public List<Point> GetOpenableSurroundingPoints(Mine[,] buttons, Point center, bool isSolo)
        {
            int level = -1;

            if (isSolo)
                level = SOLO_LEVEL;
            else
                level = MULTI_LEVEL;


            List<Point> openPoints = new();
            int flag_cnt = 0;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = center.X + WAY[w, 0], ny = center.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                if (buttons[ny, nx].isMineFlagOn()) flag_cnt++;
            }

            if (buttons[center.Y, center.X].GetArroundMineCount() != flag_cnt) return openPoints;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = center.X + WAY[w, 0], ny = center.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                Mine btn = buttons[ny, nx];
                if (!btn.isCorrectMineFlag()) openPoints.Add(new Point(nx, ny));
            }

            return openPoints;
        }

        public bool IsGameClear(bool solo)
        {
            int level = -1;
           
            if (solo)
                level = SOLO_LEVEL;
            else
                level = MULTI_LEVEL;
                     
            int open = 0;
            foreach (var button in buttons)
            {
                if (!button.isEnabled()) open++;
            }
            int openAmount = buttons.Length - MineCount[level];
            if (openAmount == open)
                return true;
            return false;
        }

        public int GetMineFlagCnt(bool solo)
        {
            if ( solo ) return MineFlagCount[SOLO_LEVEL];
            else
            {
                return MineFlagCount[MULTI_LEVEL];
            }
        }
        
        public string GetMineFlag(int n )
        {
            if (n < 0 || n >= 3) return "";
            else                 return MINE_FLAG[n];
        }
    }
}