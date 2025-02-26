using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class MyButton : Button
    {
        const int BUTTON_SIZE = 30;
        Mine? mine;
        Point xyPoint;
        Rectangle rectangle;

        int ArroundMineCount = 0;


        readonly string[] MINE_FLAG = { " " , "¶", "?" };
        int Mine_Cnt = 0;
        public MyButton (Point xy)
        {
            ArroundMineCount = 0;
            Text = " ";
            this.xyPoint = xy;
        }

        public void SetRectangle(Point point)
        {
            rectangle = new(point, new Size(30, 30));
        }
        public bool PtInRect(Point point)
        {
            return rectangle.Contains(point);
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
        public Point GetButtonXYPoint()
        {
            return xyPoint;
        }

        public bool isMineFlagOn()
        {
            return Text.Equals(MINE_FLAG[1]);
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
