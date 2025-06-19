using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{

    public class GameManager
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



        Mine[,] buttons;
        int solo_level = 0; 
        int multi_level = 0;

        Random rand = new Random();

        public GameManager()
        {
            
        }

        void SetButtons(Mine[,] buttons)
        {
            this.buttons = buttons;
        }


        void RandomLayingMine(int level)
        {
            for (int i = 0; i < gameManager.MineCount[level];)
            {
                int n = rand.Next(buttons.Length);

                var (rows, cols) = gameManager.GetGamePanSize(level);
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
    }
}
