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

        int ArroundMineCount = 0;
        int NO;

        public MyButton (int n)
        {
            ArroundMineCount = 0;
            NO = n;
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

        public int GetButtonNO()
        {
            return NO;
        }

        public Point GetButtonPoint()
        {
            return new Point(NO % 9, NO / 9);
        }
    }
}
