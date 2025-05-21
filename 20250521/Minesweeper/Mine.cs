using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Mine : Button
    {
        const int BUTTON_SIZE = 30;
        bool mine;
        Point xyPoint;
        Rectangle rectangle;

        int ArroundMineCount;
        int ArroundFlagCount;


        readonly string[] MINE_FLAG = { " ", "¶", "?" };
        int Mine_Flag_Idx;
        public Mine(Point xy, Point point)
        {
            mine = false;
            ArroundMineCount = 0;
            ArroundFlagCount = 0;
            Mine_Flag_Idx = 0;
            Text = " ";
            this.xyPoint = xy;
            Location = point;
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
            return mine;
        }

        public void LayingMine()
        {
            mine = true;
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
        public int GetArroundFlagCount()
        {
            return ArroundFlagCount;
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
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate { SetButtonClick(); }));
            }
            else
            {
                SetButtonClick();
            }
        }

        void SetButtonClick()
        {
            Enabled = false;
            BackColor = Color.White;
            Text = GetArroundMineCount().ToString();
            if (isZeroCount()) Text = "";
            else if (ArroundMineCount == -1) Text = "";
            else if (isMineHave())
            {
                Text = "♨";
                BackColor = Color.Red;
            }
        }

        public void MineFlagChange()
        {
            Text = MINE_FLAG[(++Mine_Flag_Idx) % 3];
        }

        public bool isCorrectMineFlag()
        {
            return isMineHave() && isMineFlagOn();
        }
    }
}