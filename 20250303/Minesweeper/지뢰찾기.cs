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

        bool gameoverFlag = false;
        bool clearFlag = false;
        Random rand = new Random();
        Mine[,] buttons;
        List<Mine> minebtnList;
       
        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Level = 0;
            minebtnList = new();
          
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

            gameoverFlag = false;
            MineFlagCnt = MINE_FLAG_COUNT[Level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }

        void InitGamePan(int level)
        {
            int ny = GAMEPAN[level, 1], nx = GAMEPAN[level, 0];
            buttons = new Mine[ny, nx];
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

        bool isMineHave(Mine button)
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

        void ThreadDetectorZeroButton(Mine[,] buttons, Point pt, int level)
        {
            List<Mine> list = new List<Mine>();
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                if (isMineHave(buttons[ny, nx])) continue;
                list.Add(buttons[ny, nx]);
            }

            if (list.Count <= 0) return;
            send2MainThread(list);
        }

        public void send2MainThread(object o)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => send2MainThread(o)));
                return;
            }
            
            foreach(Mine m in (List<Mine>)o)
                m.PerformClick();
        }

        Mine MakeButton(int x, int y)
        {
            Mine button = new Mine(new Point(x, y), ButtonLocationPoint(x, y));
            button.AutoSize = false;
            button.Size = new Size(BUTTON_SIZE, BUTTON_SIZE);
            button.Text = " ";
            button.Click += Button_Clicked_Event;
            button.MouseUp += Button_Right_Clicked_Event;


            return button;
        }

        Point ButtonLocationPoint(int x, int y)
        {
            return new Point(BUTTON_START_POSITION_X + BUTTON_SIZE * x, BUTTON_START_POSITION_Y + BUTTON_SIZE * y);
        }

        private void Button_Clicked_Event(object? sender, EventArgs e)
        {
            Mine button = (Mine)sender;

           

            if (!button.Enabled)
            {
            
            }
            else if (button.isMineHave())
            {
                if (gameoverFlag)
                {
                    ChooseLevel();
                }
                GameOver();
            }
            else
            {
                button.ButtonClick();
                if (button.isZeroCount())
                {
                    Thread thread = new Thread(() => ThreadDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level));
                    //DetectorZeroButton(button.GetButtonXYPoint(), Level);
                }
                if (isGameClear(Level)) {
                    GameClear();
                }
            }            
        }


        void Button_Right_Clicked_Event(Object sender, MouseEventArgs e)
        {
            Mine button = (Mine)sender;
            if (e.Button == MouseButtons.Right)
            {
                button.MineFlagChange();
            }

            int cnt = 0;
            foreach (Mine btn in buttons)
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
            int flag_cnt = 0;
            if (gameoverFlag) return;
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                Mine btn = buttons[ny, nx];
                if (btn.isMineFlagOn()) flag_cnt++;
            }

            if (buttons[pt.Y, pt.X].GetArroundMineCount() != flag_cnt) return;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                Mine btn = buttons[ny, nx];
                
                if (btn.isCorrectMineFlag()) continue;
                if (gameoverFlag) return;
                btn.PerformClick();
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
            if(gameoverFlag)
            {
                ChooseLevel();
            }
        }

        private bool isGameClear(int level)
        {
            int open = 0;
            foreach (var button in buttons)
            {
                if (!button.isEnabled()) open++;
            }
            int openAmount = buttons.Length - MINE_COUNT[level];
            if (openAmount == open)
                return true;
            return false;
        }

        private void GameClear()
        {
            if(clearFlag) return;
            clearFlag = true;

            MessageBox.Show("GAME CLEAR");
            if (timer_PlayTime.Enabled)
            {
                timer_PlayTime.Stop();
                timer = 0;
            }

        }

        private void GameOver()
        {
            gameoverFlag = true;
            foreach (var button in minebtnList)
            {
                button.Text = "♨";
                button.BackColor = Color.Red;
                button.Enabled = false;
            }
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
            clearFlag = false;
            gameoverFlag = false;
            foreach (var button in buttons)
            {
                Controls.Remove(button);
                button.Dispose();
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
            ChooseLevel();
        }

        private void ChooseLevel()
        {
            Setting settingForm = new();
            if (settingForm.ShowDialog() == DialogResult.OK)
            {
                int level = settingForm.GetLevel();
                if (level != -1) Level = level;
            }

            PlayReGame();
        }
    }
}

/*
 */