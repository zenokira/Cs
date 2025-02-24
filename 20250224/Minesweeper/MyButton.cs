using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class MyButton : Button
    {
        Mine? mine;
        Point point;

        int ArroundMineCount = 0;


        readonly string[] MINE_FLAG = { " " , "¶", "?" };
        int Mine_Cnt = 0;
        public MyButton (Point point)
        {
            ArroundMineCount = 0;
            Text = " ";
            this.point = point;
        }

        public string GetButtonText()
        {
            return Text;
        }
        public bool isEnabled()
        {
            return Enabled == true;
        }
        public bool isMineHave()
        {
            return mine != null;
        }

        public void LayingMine()
        {
            mine = new ();
            ArroundMineCount = -1;
        }

        public void AroundMineCountPlus()
        {
            ArroundMineCount++;
        }

        public int GetArroundMineCount()
        {
            return ArroundMineCount;
        }

        public bool isZeroCount()
        {
            return ArroundMineCount == 0;
        }

      

        public Point GetButtonPoint()
        {
            return point;
        }

        public void ButtonClick()
        {
            Enabled = false;
            BackColor = Color.White;
            Text = GetArroundMineCount().ToString();
            if (isZeroCount())Text = "";
        }

        public void MineFlagChange()
        {
            Text = MINE_FLAG[(++Mine_Cnt) % 3];
        }
    }
}
