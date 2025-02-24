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

        const int TEXTBOX_SIZE = 50;
        const int TEXTBOX_START_POSITION_Y = 20;

        const int WAYS = 8;
        readonly int[,] WAY = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 }, { 1, 1 } };
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 40, 16 } };

        int MineFlagCnt;

        int timer = 0;
        Random rand = new Random();
        MyButton[,] buttons;
        TextBox tb_MineFlag;
        TextBox tb_Timer;
        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GameStart();
        }

        private TextBox CreateTextBox(Color backColor, Point Location)
        {
            TextBox textBox = new TextBox();

            textBox.BackColor = backColor;
            textBox.ReadOnly = true;
            textBox.TextAlign = HorizontalAlignment.Center;
            textBox.ForeColor = Color.White;
            textBox.Size = new Size(TEXTBOX_SIZE, BUTTON_SIZE);
            textBox.Location = Location;
            return textBox;
        }
        void GameStart()
        {
            InitGamePan(0);
            RandomLayingMine(0);
            DetectorMine(0);

            MineFlagCnt = MINE_FLAG_COUNT[0];
            timer_PlayTime.Start();
        }

        void InitGamePan(int Level)
        {
            int ny = GAMEPAN[Level, 1], nx = GAMEPAN[Level, 0];
            buttons = new MyButton[ny, nx];
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    buttons[y, x] = MakeButton(x, y);
                    Controls.Add(buttons[y, x]);
                }
            }

            int tb_x = BUTTON_START_POSITION_X + BUTTON_SIZE * GAMEPAN[Level, 0];
            tb_MineFlag = CreateTextBox(Color.Black, new Point(tb_x - TEXTBOX_SIZE * 2, TEXTBOX_START_POSITION_Y));
            tb_Timer = CreateTextBox(Color.OrangeRed, new Point(tb_x - TEXTBOX_SIZE, TEXTBOX_START_POSITION_Y));

            Controls.Add(tb_MineFlag);
            Controls.Add(tb_Timer);
        }


        void RandomLayingMine(int level)
        {
            for (int i = 0; i < MINE_COUNT[level]; i++)
            {
                int n = rand.Next(buttons.Length);

                int y = n / GAMEPAN[level, 0];
                int x = n % GAMEPAN[level, 0];

                buttons[y, x].LayingMine();
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
                    if (nx < 0 || nx >= 9 || ny < 0 || ny >= 9) continue;
                    if (buttons[ny, nx].isMineHave()) buttons[y, x].AroundMineCountPlus();
                }
            }
        }

        void DetectorZeroButton(Point point)
        {
            Point pt = point;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= 9 || ny < 0 || ny >= 9) continue;
                if (buttons[ny, nx].isMineHave()) continue;
                buttons[ny, nx].PerformClick();
            }
        }



        MyButton MakeButton(int x, int y)
        {
            MyButton button = new MyButton(new Point(x, y));
            button.AutoSize = false;
            button.Size = new Size(BUTTON_SIZE, BUTTON_SIZE);
            button.Location = new Point(BUTTON_START_POSITION_X + BUTTON_SIZE * x, BUTTON_START_POSITION_Y + BUTTON_SIZE * y);
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
                Open(button.GetButtonPoint());
                return;
            }
            else if (button.isMineHave()) return;
            else
            {
                button.ButtonClick();
                if (button.isZeroCount())
                {
                    DetectorZeroButton(button.GetButtonPoint());
                }
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
                    MessageBox.Show("gameover");
                    return;
                }
                else
                {

                    button.ButtonClick();
                    if (button.isZeroCount())
                    {
                        DetectorZeroButton(button.GetButtonPoint());
                    }
                }
            }

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
            tb_MineFlag.Text = (MineFlagCnt - cnt).ToString();
        }

        private void timer_PlayTime_Tick(object sender, EventArgs e)
        {
            timer++;
            tb_Timer.Text = timer.ToString();

        }

        private void Open(Point point)
        {
            Point pt = point;
            int cnt = 0;
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= 9 || ny < 0 || ny >= 9) continue;
                if (buttons[ny, nx].isMineHave() &&
                    buttons[ny, nx].GetButtonText().Contains(MINE_FLAG[1]))
                    cnt++;
            }
            if (cnt != buttons[point.Y, point.X].GetArroundMineCount()) return;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= 9 || ny < 0 || ny >= 9) continue;
                if (buttons[ny, nx].isMineHave()) continue;
                buttons[ny, nx].PerformClick();
            }
        }

        private void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
            Open(e.Location);
        }
    }
}

/*
 * 가로 세로 9 X 9
 * 1. 지뢰라면 gameover
 * 2. 지뢰에 탐지깃발이 꽂혀있고, 현재 disabled 상태의 버튼 주위에 지뢰가 없다면 근처 오픈
 *  
 */