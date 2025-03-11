using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Minesweeper
{
    public partial class 지뢰찾기 : Form
    {
        readonly string[] MINE_FLAG = { " ", "¶", "?" };
        readonly string[] COMMAND = { "start", "gameover", "click" };
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

        Network network;
        int mySocketNo = -1;
        int turnSocketNo = -1;
        int opponentNo = -1;
        bool MyTurnFlag = false;
        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Level = 0;
            GameStart();
        }
        private async void Button_Clicked_Event(object? sender, EventArgs e)
        {
            Mine button = (Mine)sender;
            if (!MyTurnFlag) return;

            if (button.Enabled)
            {
                network.SendMessage($"click {mySocketNo} {opponentNo} {button.GetButtonXYPoint().X}*{button.GetButtonXYPoint().Y}");
                return;
            }
            if (!button.Enabled)
            {
                Thread thread = new Thread(() => ThreadOpen(buttons, button.GetButtonXYPoint(), Level));
                thread.Start();
            }
            else if (button.isMineHave())
            {

                GameOver();
                await Task.Run(() => MessageBox.Show("GAME OVER"));

                ChooseLevel();
            }
            else
            {
                
                if (button.isZeroCount())
                {
                    button.ButtonClick();

                    Thread thread = new Thread(() => ThreadDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level));
                    thread.Start();
                }
                else
                {
                    button.ButtonClick();
                }
                if (isGameClear(Level))
                {
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
                    Mine btn = buttons[ny, nx];
                    if (btn.isMineHave()) buttons[y, x].AroundMineCountPlus();
                }
            }
        }

        void ThreadDetectorZeroButton(Mine[,] buttons, Point pt, int level)
        {
            List<Mine> list = new();
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                Mine btn = buttons[ny, nx];
                if (btn.isMineHave()) continue;
                list.Add(btn);
            }
            send2MainThread(list);
        }

        void send2MainThread(object o)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => send2MainThread(o)));
                return;
            }
            foreach (Mine m in (List<Mine>)o)
            {
                m.PerformClick();
            }
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
        private void ThreadOpen(Mine[,] buttons, Point pt, int level)
        {
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

            List<Mine> list = new();
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                Mine btn = buttons[ny, nx];

                if (btn.isCorrectMineFlag()) continue;
                if (gameoverFlag) return;
                list.Add(btn);
            }
            foreach (Mine m in list)
            {
                m.PerformClick();
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
            if (clearFlag) return;
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

            foreach (var button in buttons)
            {
                if (!button.isMineHave()) continue;
                button.Text = "♨";
                button.BackColor = Color.Red;
                button.Enabled = false;
            }

            if (timer_PlayTime.Enabled)
            {
                timer_PlayTime.Stop();
                timer = 0;
            }

        }

        private void GameReset()
        {
            timer_PlayTime.Stop();
            timer = 0;
            clearFlag = false;
            gameoverFlag = false;

            for (int y = 0; y < buttons.GetLength(0); y++)
            {
                for (int x = 0; x < buttons.GetLength(1); x++)
                {
                    Controls.Remove(buttons[y, x]);
                    buttons[y, x].Dispose();
                    buttons[y, x] = null;
                }
            }
            buttons = null;
        }
        private void 새로시작ToolStripMenuItem_Click(object sender, EventArgs e)
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

            GameReset();
            GameStart();

        }

        private void asdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if( network == null)
            {
                network = new Network();
                network.Connect();
            }
        }

        public void net2MainThread(object o)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => net2MainThread(o)));
                return;
            }
            RecvDataUse((string)o);
        }

        void RecvDataUse(string o)
        {
            string command, data;
            int sendNo, recvNo;

            ParsingRecvData(o,out command, out sendNo, out recvNo, out data);

            if ( command.Equals("start"))
            {
                GameReset();
                FormResize(Level);
                InitGamePan(Level);
                mySocketNo = recvNo;
                opponentNo = sendNo;

                if (mySocketNo < opponentNo) MyTurnFlag = false;
                else
                {
                    MyTurnFlag = true;
                }

                List<Point> mine = MineLaying(data);

                foreach (Point pt in mine)
                {
                    buttons[pt.Y, pt.X].LayingMine();
                }
                NetWorkGameInit();
            }
            else if (command.Equals("click"))
            {
                string[] pt = data.Split("*");
                int x = Convert.ToInt32(pt[0]), y = Convert.ToInt32(pt[1]);
                buttons[y, x].PerformClick();

                if ( sendNo == opponentNo && recvNo == mySocketNo )
                {
                    MyTurnFlag = true;
                }
            }
        }

        void NetWorkGameInit()
        {
            DetectorMine(Level);

            gameoverFlag = false;
            MineFlagCnt = MINE_FLAG_COUNT[Level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }
        List<Point> MineLaying(string data)
        {
            string[]s = data.Split(' ');
            List<Point> points = new List<Point>();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].Equals("")) continue;
                string[] pt = s[i].Split("*");
                points.Add(new Point(Convert.ToInt32(pt[0]), Convert.ToInt32(pt[1])));
            }
            return points;
        }

        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string data)
        {
            string[] s = recvData.Split(" ");
            command = s[0];
            sendNo = Convert.ToInt32(s[1]);
            recvNo = Convert.ToInt32(s[2]);
            
            if (s.Length > 4)
                data = ConcatData(s);
            else
                data = s[3];
            return true;
        }

        string ConcatData(string[] data)
        {
            string s = "";

            for (int i = 3; i < data.Length; i++)
            {
                s += $"{data[i]} ";
            }
            return s;
        }
    }
}

/*
 * 턴제 게임판은 1개 사람은 2명
 * 네트워크 연결 -> 대기 -> 매치 
 * 1. 오픈이 되면 턴을 넘긴다.
 * 2. 승부판정 ( 끝까지 가서 클리어 하면 무승부 , 지뢰를 터트리면 패배 )
 * 
 * 데이터 구조  Command SendSocketNo RecvSocketNo Data
 * 커맨드 [ start, gameover, click ]
 */



