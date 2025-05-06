using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Minesweeper
{
    public partial class 지뢰찾기 : Form
    {
        readonly string[] MINE_FLAG = { " ", "¶", "?" };
        readonly string[] COMMAND = { "start", "level", "retry", "exit", "click", "gameover", "server" }; // win draw lose
        const int BUTTON_SIZE = 30;
        const int BUTTON_START_POSITION_X = 20;
        const int BUTTON_START_POSITION_Y = 60;


        const int WAYS = 8;
        readonly int[,] WAY = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 }, { 1, 1 } };
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[] MINE_FLAG_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        int MineFlagCnt;
        int Level=0;
        int timer = 0;
        int opp_timer = 0;
        int serverNo = -9999;

        bool networkPlayFlag = false;
        bool gameoverFlag = false;
        bool clearFlag = false;
        Random rand = new Random();
        Mine[,] buttons;

        Network? network;
        int mySocketNo = -1;
        int opponentNo = -1;
        bool MyTurnFlag = false;
        bool OppTurnFlag = false;
        bool ServerFlag;
        bool SoloFlag;

        int opp_state = -1;  // 0 게임중  1 게임오버  2 클리어

        bool first = false;
        bool serverclickFlag;


        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";



        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            lb_Turn.Text = "싱글 모드";
            GameStart();
        }
        private async void Button_Clicked_Event(object? sender, EventArgs e)
        {
            Mine button = (Mine)sender;

            if (SoloFlag)
            {
                if (button.isEnabled())
                {
                    if (button.isMineHave())
                    {
                        GameOver();
                        await Task.Run(() => MessageBox.Show("GAME OVER"));
                        ChooseLevel();
                        GameReset();
                        GameStart();
                    }
                    else if (button.isZeroCount())
                    {

                        TestDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
                        /*button.ButtonClick();
                        //ThreadDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
                        Thread thread = new Thread(() => ThreadDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level));
                        thread.Start();
                        */
                    }
                    else
                    {
                        button.ButtonClick();
                    }
                }
                if (isGameClear(Level))
                {
                    GameClear();
                    if (MessageBox.Show("다시 하시겠습니까?", "YESORNO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ChooseLevel();
                        GameReset();
                        GameStart();
                    }
                }
            }

            else if (ServerFlag)
            {
                if (serverclickFlag)
                {
                    if (button.isZeroCount())
                    {
                        TestDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
                    }
                    else
                    {
                        button.ButtonClick();
                    }
                    serverclickFlag = false;
                }

                else if (MyTurnFlag)
                {
                    if (button.Enabled)
                    {
                        if (button.isMineHave())
                        {
                            network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");

                            if (first)
                            {
                                if (opp_state == 0)
                                {
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "gameover");
                                    MessageBox.Show("지뢰를 터트리셨습니다. 상대방 턴이 종료될 때 까지 기다려주세요.");
                                }
                            }
                            else
                            {
                                if (opp_state == 1)
                                {
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "draw");
                                }
                                else
                                {
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "lose");
                                }
                            }
                            GameOver();
                        }
                        else if (button.isZeroCount())
                        {
                            network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");
                            TestDetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
                        }
                        else
                        {
                            if (first)
                            {
                                network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");
                                button.ButtonClick();
                            }
                            else
                            {
                                if (opp_state == 0)
                                {
                                    network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");
                                    button.ButtonClick();
                                }
                                else if (opp_state == 1)
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "win");
                                else if (opp_state == 2)
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "lose");
                            }

                        }

                        if (isGameClear(Level))
                        {
                            if (first)
                            {
                                network.SendMessage(COMMAND[5], mySocketNo, opponentNo, $"clear {timer}");
                                timer_PlayTime.Stop();
                                MessageBox.Show("클리어 하셨습니다. 상대방 턴이 종료될 때 까지 기다려주세요.");
                            }
                            else
                            {
                                if (opp_state == 2)
                                {
                                    if (timer < opp_timer)
                                        network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "win");
                                    else if (timer > opp_timer)
                                        network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "lose");
                                    else
                                        network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "draw");
                                }
                                else
                                {
                                    network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "win");
                                }
                            }

                            GameClear();
                        }
                        lb_Turn.Text = "상대 차례";
                        MyTurnFlag = false;

                    }
                }
            }
        }
        void Button_Right_Clicked_Event(object? sender, MouseEventArgs e)
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
            SoloFlag = true;
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

        void TestDetectorZeroButton(Mine[,] buttons, Point pt, int level)
        {
            Queue<Point> queue = new();
            queue.Enqueue(pt);

            while (queue.Count != 0)
            {
                Point point = queue.Dequeue();
                if (!buttons[point.Y, point.X].isEnabled()) continue;
                buttons[point.Y, point.X].ButtonClick();
                for (int w = 0; w < WAYS; w++)
                {
                    int nx = point.X + WAY[w, 0], ny = point.Y + WAY[w, 1];
                    if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                    Mine btn = buttons[ny, nx];
                    if (btn.isMineHave()) continue;
                    if (!btn.isZeroCount())
                    {
                        buttons[ny, nx].ButtonClick();
                        continue;
                    }
                    queue.Enqueue(new Point(nx, ny));
                }
            }
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

        private int TestOpen(Mine[,] buttons, Point pt, int level)
        {
            if (gameoverFlag) return 0;

            int flag_cnt = 0;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                Mine btn = buttons[ny, nx];
                if (btn.isMineFlagOn()) flag_cnt++;
            }

            if (buttons[pt.Y, pt.X].GetArroundMineCount() != flag_cnt) return 0;

            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                Mine btn = buttons[ny, nx];

                if (btn.isCorrectMineFlag()) continue;

                if (btn.isZeroCount())
                {
                    TestDetectorZeroButton(buttons, new Point(nx, ny), level);
                }

                if (btn.isEnabled()) btn.ButtonClick();

                if (btn.isMineHave())
                {
                    return 1;
                }
                else if (isGameClear(Level))
                {
                    return 2;
                }
            }
            return 0;
        }
        private void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
            int result = -1;
            if (SoloFlag)
            {
                foreach (var button in buttons)
                {
                    if (button.PtInRect(e.Location))
                    {
                        result = TestOpen(buttons, button.GetButtonXYPoint(), Level);
                        break;
                    }
                }
                if (result == 1)
                {
                    GameOver();
                    ChooseLevel();
                    GameReset();
                    GameStart();
                }
                else if (result == 2)
                {
                    GameClear();
                    if (MessageBox.Show("다시 하시겠습니까?", "YESORNO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ChooseLevel();
                        GameReset();
                        GameStart();
                    }

                }
            }
            else
            {
                if (!MyTurnFlag) return;
                foreach (var button in buttons)
                {
                    if (button.PtInRect(e.Location))
                    {
                        result = TestOpen(buttons, button.GetButtonXYPoint(), Level);
                        break;
                    }
                }
                if (result == 1)
                {
                    network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");
                    if (first)
                    {
                        network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "gameover");
                        MessageBox.Show("지뢰를 터트리셨습니다. 상대방 턴이 종료될 때 까지 기다려주세요.");
                    }
                    else
                    {
                        if (opp_state == 1)
                            network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "draw");
                        else
                            network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "lose");
                    }
                    GameOver();
                }
                else if (result == 2)
                {
                    network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "click");

                    if (first)
                    {
                        network.SendMessage(COMMAND[5], mySocketNo, opponentNo, $"clear {timer}");
                        timer_PlayTime.Stop();
                        MessageBox.Show("클리어 하셨습니다. 상대방 턴이 종료될 때 까지 기다려주세요.");
                    }
                    else
                    {

                        if (opp_state == 2)
                        {
                            if (timer < opp_timer)
                                network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "win");
                            else if (timer > opp_timer)
                                network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "lose");
                            else
                                network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "draw");
                        }
                        else
                        {
                            network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "win");
                        }
                    }

                    GameClear();
                }
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
            if (clearFlag) return;
            clearFlag = true;

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
            if (ServerFlag) return;
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

        async void RecvDataUse(string o)
        {
            string command, data;
            int sendNo, recvNo;

            if (!IsCorrectData(o)) return;

            List<string> list = GetRecvMultiLine(o);

            for (int i = 0; i < list.Count; i++)
            {

                ParsingRecvData(list[i], out command, out sendNo, out recvNo, out data);
                LogPacket(o);
                if (command.Equals(COMMAND[0]))
                {
                    GameReset();
                    FormResize(Level);
                    InitGamePan(Level);
                    lb_Turn.Location = new Point(this.Size.Width / 2 - lb_Turn.Width / 2, menuStrip1.Height);

                    List<Point> mine = MineLaying(data);
                   
                    foreach (Point pt in mine)
                    {
                        buttons[pt.Y, pt.X].LayingMine();
                    }
                    NetWorkGameInit();
                    mySocketNo = recvNo;
                    opponentNo = sendNo;
                    opp_state = 0;

                    MyTurnFlag = mySocketNo < opponentNo;
                    first = MyTurnFlag;
                    ServerFlag = true;

                    toolStripTextBox_MyNum.Text = $"나 : {mySocketNo.ToString()}";
                    toolStripTextBox_OPNum.Text = $"상대 : {opponentNo.ToString()}";


                    if (MyTurnFlag || SoloFlag) lb_Turn.Text = "내 차례";
                    else lb_Turn.Text = "상대 차례";
                    networkPlayFlag = true;

                    if (!MyTurnFlag) timer_PlayTime.Stop();

                }
                else if (command.Equals(COMMAND[1]))
                {

                }
                else if (command.Equals(COMMAND[2]))
                {
                    GameReset();
                    GameStart();
                }
                else if (command.Equals(COMMAND[3]))
                {
                    opponentNo = -1;

                    if (MessageBox.Show("상대방이 접속을 종료하였습니다.\n다시 하시겠습니까? (YES or No)", "YESORNO", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        network.SendMessage(COMMAND[2], mySocketNo, serverNo, "retry");

                    }
                    else
                    {
                        network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString());
                    }
                }
                else if (command.Equals(COMMAND[4]))
                {
                    if (sendNo == serverNo)
                    {
                        string[] pt = data.Split("*");
                        int x = Convert.ToInt32(pt[0]), y = Convert.ToInt32(pt[1]);
                        serverclickFlag = true;
                        serverRecieveClicked(x, y);
                    }
                    if (sendNo == serverNo)
                    {
                        OppTurnFlag = false;
                        return;
                    }
                    if (sendNo == opponentNo && recvNo == mySocketNo && !MyTurnFlag)
                    {
                        MyTurnFlag = true;
                        OppTurnFlag = false;
                        lb_Turn.Text = "내 차례";
                        if (!timer_PlayTime.Enabled) timer_PlayTime.Start();
                    }
                }
                else if (command.Equals(COMMAND[5]))
                {
                    if (data.Equals("gameover"))
                    {
                        opp_state = 1;
                        return;
                    }
                    else if (data.StartsWith("clear"))
                    {
                        opp_state = 2;
                        string[] dataList = data.Split(" ");
                        opp_timer = Convert.ToInt32(dataList[1]);
                    }
                    else if (data.Equals("retire"))
                    {
                        GameOver();
                        MessageBox.Show("상대가 기권했습니다.\n대기모드로 전환합니다");
                        network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString());
                    }
                    else
                    {
                        GameOver();
                        string result = "";
                        if (data.Equals("win"))
                        {
                            result = "승리하셨습니다";
                        }
                        else if (data.Equals("lose"))
                        {
                            result = "패배하셨습니다";
                        }
                        else if (data.Equals("draw"))
                        {
                            result = "무승부입니다";
                        }

                        if (MessageBox.Show($"{result}\n다시 하시려면 Yes 를, 그만 하고 싶으시면 No 를 눌러주세요", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            network.SendMessage(COMMAND[2], mySocketNo, mySocketNo, "retry");
                        }
                        else
                        {
                            network.SendMessage(COMMAND[1], mySocketNo, mySocketNo, Level.ToString());
                        }
                    }
                }

                else if (command.Equals(COMMAND[6]))
                {
                    if (data.Equals("서버종료"))
                    {
                        server_off();
                        MessageBox.Show("서버가 종료되었습니다.\n모든 연결이 끊겨 재접속 전까지 혼자하기만 가능합니다", "알림");
                    }
                    else if ( data.Equals("error"))
                    {
                        MessageBox.Show("error");
                    }
                    else
                    {
                        if (mySocketNo > 0) return;
                        try
                        {                       
                            mySocketNo = Convert.ToInt32(data);
                        }
                        catch (FormatException e1)
                        {
                            
                        }
                        if (mySocketNo == null || mySocketNo == -1)
                            network.SendMessage(COMMAND[6], mySocketNo, serverNo, "소켓번호");
                    }
                }
            }
        }

        List<string> GetRecvMultiLine(string recvData)
        {
            List<string> list = new();
            string[] str = recvData.Split(END_WORD);

            for (int i = 0; i < str.Length; i++)
            {
                if (!str[i].Equals("")) list.Add(str[i]);
            }

            return list;
        }

        void serverRecieveClicked(int x, int y)
        {
            //await Task.Run(() => buttons[y, x].PerformClick());
            if (buttons[y, x].InvokeRequired)
            {
                buttons[y, x].Invoke(new MethodInvoker(delegate { buttons[y, x].PerformClick(); }));
            }
            else
            {
                buttons[y, x].PerformClick();
            }
        }

        void NetWorkGameInit()
        {
            DetectorMine(Level);
            gameoverFlag = false;
            SoloFlag = false;
            networkPlayFlag = true;
            MineFlagCnt = MINE_FLAG_COUNT[Level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }
        List<Point> MineLaying(string data)
        {
            string[] s = data.Split(' ');
            List<Point> points = new List<Point>();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].Equals("")) continue;
                string[] pt = s[i].Split("*");

                points.Add(new Point(Convert.ToInt32(pt[0]), Convert.ToInt32(pt[1])));
            }
            return points;
        }

        bool IsCorrectData(string data)
        {
            return data.StartsWith(START_WORD) && data.EndsWith(END_WORD);
        }
        bool ParsingRecvData(string recvData, out string command, out int sendNo, out int recvNo, out string data)
        {
            string eData = ExtractionData(recvData);

            string[] s = eData.Split(" ");
            command = s[0];
            sendNo = Convert.ToInt32(s[1]);
            recvNo = Convert.ToInt32(s[2]);

            if (s.Length > 4)
                data = ConcatData(s);
            else
                data = s[3];
            return true;
        }

        string ExtractionData(string data)
        {
            string eData = data.Replace(START_WORD, "");
            eData = eData.Replace(END_WORD, "");
            return eData;
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

        private void 지뢰찾기_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (network == null) return;

            if (network.IsSocketNULL()) return;

            if (network.IsConnected())
            {
                network.SendMessage(COMMAND[3], mySocketNo, opponentNo, "exit");
            }

            Application.Exit();
        }

        private void 접속하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (network == null)
            {
                network = new Network();

                network.Connect();
                timer_chckConnect.Start();

                if (!network.IsConnected())
                {
                    //network.Dispose();
                    network.Disconnect();
                    network = null;
                    MessageBox.Show("서버와 연결을 실패했습니다");
                }
                else
                {
                    timer_chckConnect.Start();
                    MessageBox.Show("서버와 연결되었습니다.");
                }
            }
            else if (network.IsConnected()) return;
        }

        private void 기권하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!network.IsConnected()) return;

            if (mySocketNo != -1 && opponentNo != -1)
                network.SendMessage(COMMAND[5], mySocketNo, opponentNo, "retire");

            if (network.IsReallyConnected())  // network.Socket은 내부의 Socket 객체라 가정
            {
                network.SendMessage(COMMAND[1], mySocketNo, opponentNo, Level.ToString());
                MessageBox.Show("대기모드로 전환합니다");
            }
            else
            {
                network.Dispose();
                MessageBox.Show("서버가 비정상적으로 종료되어 연결이 종료됩니다.");
            }
        }

        private void timer_chckConnect_Tick(object sender, EventArgs e)
        {
            /*
            if (network == null || !network.IsReallyConnected())
            {
                server_off();
                MessageBox.Show("서버가 비정상적으로 종료되어 솔로잉모드로 돌아갑니다."); 
            }
            */
        }

        void server_off()
        {
            timer_chckConnect.Stop();
            network.Dispose();
            network = null;
            lb_Turn.Text = "싱글 모드";


            GameOver();
            GameReset();
            GameStart();
        }

        private void 시작하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null || !network.IsReallyConnected() )
            {
                if ( MessageBox.Show("네트워크 연결이 안되어있습니다. 연결 후 다시 시도해주세요", "알림", MessageBoxButtons.OK) == DialogResult.OK)
                return;
            }

            ChooseLevel();
            network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString());
            GameReset();
            GameStart();
        }

        public void LogPacket(string message)
        {
            try
            {
                // 로그 파일이 존재하지 않으면 새로 생성
                using (StreamWriter writer = new StreamWriter($"network_log{mySocketNo}.txt", true))
                {
                    // 현재 시간과 함께 메시지 기록
                    writer.WriteLine($"{message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그 기록 중 오류 발생: {ex.Message}");
            }
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



