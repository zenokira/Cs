using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Minesweeper
{
    public partial class 지뢰찾기 : Form
    {
        readonly string[] MINE_FLAG = { " ", "¶", "?" };
        const int BUTTON_SIZE = 30;
        const int BUTTON_START_POSITION_X = 20;
        const int BUTTON_START_POSITION_Y = 60;

      
        const int WAYS = 8;
        readonly int[,] WAY = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 }, { 1, 1 } };
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        int MineFlagCnt;
        int Level;
        int timer = 0;
        Random rand = new Random();
        MyButton[,] buttons;
        List<MyButton> minebtnList;
        List<MyButton> openbtnList;
       
        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Level = 0;
            minebtnList = new();
            openbtnList = new();
            GameStart();
        }

        void FormResize(int level)
        {
            Size size = this.Size - this.ClientSize; ;
            size.Width += BUTTON_START_POSITION_X * 2 + BUTTON_SIZE * GAMEPAN[level, 0];
            size.Height += BUTTON_START_POSITION_Y + BUTTON_SIZE * GAMEPAN[level, 1] + 메뉴ToolStripMenuItem.Height;

            this.Size = size;
        }


        void GameStart()
        {
            FormResize(Level);
            InitGamePan(Level);
            RandomLayingMine(Level);
            DetectorMine(Level);
            

            MineFlagCnt = MINE_FLAG_COUNT[Level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }

        void InitGamePan(int level)
        {
            int ny = GAMEPAN[level, 1], nx = GAMEPAN[level, 0];
            buttons = new MyButton[ny, nx];
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    buttons[y, x] = MakeButton(x, y);
                    Controls.Add(buttons[y, x]);
                }
            }
        }


        void RandomLayingMine(int level)
        {
            for (int i = 0; i < MINE_COUNT[level];)
            {
                int n = rand.Next(buttons.Length);

                int y = n / GAMEPAN[level, 0];
                int x = n % GAMEPAN[level, 0];

                if (buttons[y, x].isMineHave()) continue;
                buttons[y, x].LayingMine();
                minebtnList.Add(buttons[y, x]);
                i++;
            }
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
                    if (isMineHave(buttons[ny, nx])) buttons[y, x].AroundMineCountPlus();
                }
            }
        }

        bool isMineHave(MyButton button)
        {
            return minebtnList.Contains(button);
        }
        void DetectorZeroButton(Point point, int level)
        {
            Point pt = point;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level,0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                if (isMineHave(buttons[ny, nx])) continue;
                
                buttons[ny, nx].PerformClick();
            }
        }



        MyButton MakeButton(int x, int y)
        {
            MyButton button = new MyButton(new Point(x, y));
            button.AutoSize = false;
            button.Size = new Size(BUTTON_SIZE, BUTTON_SIZE);
            button.Location = new Point(BUTTON_START_POSITION_X + BUTTON_SIZE * x, BUTTON_START_POSITION_Y + BUTTON_SIZE * y);
            button.SetRectangle(button.Location);
            button.Text = " ";
            button.Click += Button_Clicked_Event;
            button.MouseClick += Button_Left_Clicked_Event;
            button.MouseUp += Button_Right_Clicked_Event;


            return button;
        }

        private void Button_Clicked_Event(object? sender, EventArgs e)
        {
            MyButton button = (MyButton)sender;

            if (!button.Enabled)
            {
                Open(button.GetButtonXYPoint(), Level);
                if (isGameClear(Level)) GameClear();
                return;
            }
            else if (button.isMineHave()) return;
            else
            {
                openbtnList.Add(button);
                button.ButtonClick();
                if (button.isZeroCount())
                {
                    DetectorZeroButton(button.GetButtonXYPoint(),Level);
                    
                }
                if (isGameClear(Level)) GameClear();
            }
            
        }

        void Button_Left_Clicked_Event(object sender, MouseEventArgs e)
        {
            MyButton button = (MyButton)sender;


            if (!button.Enabled)
                return;


            if (e.Button == MouseButtons.Left)
            {
                if (button.isMineHave())
                {
                    GameOver();
                    return;
                }
                else
                {
                    button.ButtonClick();
                    if (button.isZeroCount())
                    {
                        DetectorZeroButton(button.GetButtonXYPoint(), Level);
                    }
                }
            }
            //if (isGameClear(Level)) GameClear();

        }

        void Button_Right_Clicked_Event(Object sender, MouseEventArgs e)
        {
            MyButton button = (MyButton)sender;
            if (e.Button == MouseButtons.Right)
            {
                button.MineFlagChange();
            }

            int cnt = 0;
            foreach (MyButton btn in buttons)
            {
                string flag = btn.GetButtonText();
                if (flag.Equals(MINE_FLAG[1]) || flag.Equals(MINE_FLAG[2])) cnt++;
            }
            toolStripTextBox_MineFlag.Text = (MineFlagCnt - cnt).ToString();
        }

        private void timer_PlayTime_Tick(object sender, EventArgs e)
        {
            timer++;
            toolStripTextBox_Timer.Text = timer.ToString();

        }

        private void Open(Point point, int level)
        {
            Point pt = point;
            int cnt = 0;
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                if (isMineHave(buttons[ny, nx]) &&
                    buttons[ny, nx].GetButtonText().Contains(MINE_FLAG[1]))
                    cnt++;
            }
            if (cnt != buttons[point.Y, point.X].GetArroundMineCount()) return;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                if (buttons[ny, nx].isMineFlagOn()) continue;
                buttons[ny, nx].PerformClick();
            }
        }

        private void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (var button in buttons)
            {
                if (button.PtInRect(e.Location))
                {
                    Open(button.GetButtonXYPoint(), Level);
                    break;
                }
            }
            if (isGameClear(Level)) GameClear();
        }

        private bool isGameClear(int level)
        {
            int openAmount = buttons.Length - MINE_COUNT[level];
            if (openAmount == openbtnList.Count)
                return true;
            return false;
        }

        private void GameClear()
        {
            MessageBox.Show("GAME CLEAR");
            if (timer_PlayTime.Enabled)
            {
                timer_PlayTime.Stop();
                timer = 0;
            }

        }

        private void GameOver()
        {
            MessageBox.Show("Game Over");
            if (timer_PlayTime.Enabled)
            {
                timer_PlayTime.Stop();
                timer = 0;
            }
            
        }

        private void GameReset()
        {
            timer_PlayTime.Stop();
            minebtnList.Clear();
            foreach (var button in buttons)
            {
                Controls.Remove(button);
            }
            buttons = null;
        }
        private void 새로시작ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayReGame();
        }

        void PlayReGame()
        {
            GameReset();
            GameStart();
        }
        private void 환경설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting settingForm = new();
            if (settingForm.ShowDialog() == DialogResult.OK)
            {
                int level = settingForm.GetLevel();
                if ( level != -1) Level = level;
            }

            PlayReGame();
        }
    }
}

/*
 */