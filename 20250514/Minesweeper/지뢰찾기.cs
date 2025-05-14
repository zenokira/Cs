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
        int Level = 0;
        int timer = 0;
        int opp_timer = -1;
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

        int myState = -1;    // 0 게임중  1 게임오버  2 클리어
        int opp_state = -1;  // 0 게임중  1 게임오버  2 클리어

        bool first = false;
        bool serverclickFlag;
        bool isHandledByButtonClick = false;


        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";

        bool superClickFlag = false;


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
            if (superClickFlag) return;
            superClickFlag = true;

            Mine button = (Mine)sender;

            if ( !button.isEnabled() )
            {
                superClickFlag = false;
                return;
            }

            await Task.Run(async () =>
            {
                if (SoloFlag)
                {
                    if (button.isEnabled())
                        HandleSoloClick(button);

                }
                else if (ServerFlag)
                {
                    if (button.isEnabled())
                        await HandleServerClickAsync(button);
                }
            });

            await Task.Delay(100);
            superClickFlag = false;
        }

        private void HandleSoloClick(Mine button)
        {
            if (!button.isEnabled()) return;

            if (button.isMineHave())
            {
                SoloGameOver();
            }
            else if (button.isZeroCount())
            {
                DetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
            }
            else
            {
                button.ButtonClick();
            }

            if (isGameClear(Level))
            {
                SoloGameClear();
            }
        }

        private async Task HandleServerClickAsync(Mine button)
        {
            if (serverclickFlag)
            {
                if (button.isZeroCount())
                {
                    DetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
                }
                else
                {
                    button.ButtonClick();
                }
                serverclickFlag = false;
                return;
            }

            if (!MyTurnFlag || !button.Enabled)
                return;

            int gameResult = -1;

            SendClickCommand(mySocketNo, opponentNo);
            myState = 0;
            timer_PlayTime.Stop();
            
            if (button.isMineHave())
            {
                await MultiGameoverAsync();
                return;
            }
            else if ( button.isZeroCount())
            {
                DetectorZeroButton(buttons, button.GetButtonXYPoint(), Level);
            }
            else
            {
                button.ButtonClick();
            }

            if (isGameClear(Level))
            {
                await MultiGameClearAsync();
            }
            else
            {
                int result = JudgeGameResult(myState, opp_state);
                SendGameResult(result);
            }

        }

        private void SafeInvoke(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void SoloGameClear()
        {
            GameClear();
            if (MessageBox.Show("다시 하시겠습니까?", $"Player{mySocketNo}", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ChooseLevel();
                GameReset();
                GameStart();
            }
        }

        private void SoloGameOver()
        {
            GameOver();
            MessageBox.Show("GAME OVER");
            ChooseLevel();
            GameReset();
            GameStart();
        }

        private async Task MultiGameClearAsync()
        {
            myState = 2;
            if (first)
            {
                SendGameoverCommand( mySocketNo, opponentNo, $"clear {timer}", "MultiGameClear");
                timer_PlayTime.Stop();
                MessageBox.Show("클리어 하셨습니다. 상대방 턴이 종료될 때까지 기다려주세요.");
            }
            else
            {
                int result = JudgeGameResult(myState, opp_state);
                SendGameResult(result);
            }

            GameClear();
        }

        private async Task MultiGameoverAsync()
        {
            myState = 1;
            if (first)
            {
                SendGameoverCommand(mySocketNo, opponentNo, $"gameover", "MultiGameover");
                timer_PlayTime.Stop();
                MessageBox.Show("지뢰를 밟으셨습니다. 상대방 턴이 종료될 때까지 기다려주세요.");
            }
            else
            {
                int result = JudgeGameResult(myState, opp_state);
                SendGameResult(result);
            }

            GameOver();
        }

        private void SendGameResult(int result)
        {
            if (result == -1) { return; }
            // 게임 결과 (0: 무승부, 1: 승리, 2: 패배)
            switch (result)
            {
                case 0:
                    SendGameoverCommand(mySocketNo, opponentNo, "draw", "SendGameResult"); 
                    break;
                case 1:
                    SendGameoverCommand(mySocketNo, opponentNo, "win", "SendGameResult"); 
                    break;
                case 2:
                    SendGameoverCommand(mySocketNo, opponentNo,"lose","SendGameResult");
                    break;
            }
        }

        private void SendGameoverCommand(int mysock, int oppsock, string result, string func)
        {
            network.SendMessage(COMMAND[5], mysock, oppsock, result , func);
            MyTurnFlag = false;
        }

        private void SendClickCommand(int mysock, int oppsock)
        {
            network.SendMessage(COMMAND[4], mysock, oppsock, "click", "SendClickCommand");
            MyTurnFlag = false;
            SafeInvoke(lb_Turn, () => lb_Turn.Text = "상대 차례");
        }
        int JudgeTimerResult()
        {
            if (opp_timer == -1) return -1;         // 진행중
            else if (timer < opp_timer) return 1;  // 승리
            else if (timer > opp_timer) return 2;  // 패배
            return 0;                               // 무승부

        }
        int JudgeGameResult(int myState, int oppState) // 게임 결과 (0: 무승부, 1: 승리, 2: 패배, -1: 진행중)
        {

            if (oppState == 0)
            {
                if (myState == 1) return 2; // 패배
                else if (myState == 2) return 1; // 승리
            }
            else if (oppState == 1)
            {
                if (myState == 1) return 0; // 무승부
                return 1; // 승리
            }
            else if (oppState == 2)
            {
                int timeResult = JudgeTimerResult(); // (0: 무승부, 1: 승리, 2: 패배, -1: 진행중)

                if (myState == 2)
                {
                    return timeResult;
                }
                else
                {
                    return 2; // 패배
                }
            }

            return -1;
        }


        void Button_Right_Clicked_Event(object? sender, MouseEventArgs e)
        {
            Mine button = (Mine)sender;

            if (button.isEnabled())
            {
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
            else
            {

            }
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

        void DetectorZeroButton(Mine[,] buttons, Point pt, int level)
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

        private int OpenSurroundingButtons(Mine button, int level)
        {
            if (gameoverFlag) return 0;

            int flag_cnt = 0;
            Point pt = button.GetButtonXYPoint();

          
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                // 1. 지뢰 플래그가 있는 버튼 개수 셈
                if (buttons[ny, nx].isMineFlagOn()) flag_cnt++;
            }

            // 3. 표시된 지뢰 개수와 실제 주변 지뢰 개수 비교
            if (buttons[pt.Y, pt.X].GetArroundMineCount() != flag_cnt) return 0;

            // 4. 주변 버튼을 열기
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                Mine btn = buttons[ny, nx];

                // 5. 이미 지뢰 플래그가 맞으면 넘어감
                if (btn.isCorrectMineFlag()) continue;
                
                if (btn.isZeroCount())
                {
                    DetectorZeroButton(buttons, new Point(nx, ny), level);
                }

                if (btn.isMineHave())
                {
                    return 1;
                }

                if (btn.isEnabled()) btn.ButtonClick();

                if (isGameClear(Level))
                {
                    return 2;
                }
            }
            return 0;
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
            HashSet<string> processedMessages = new HashSet<string>();

            for (int i = 0; i < list.Count; i++)
            {
                string message = list[i];

                if (processedMessages.Contains(message)) continue;

                processedMessages.Add(message);

                ParsingRecvData(message, out command, out sendNo, out recvNo, out data);
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
                    opp_state = -1;

                    MyTurnFlag = mySocketNo < opponentNo;
                    first = MyTurnFlag;
                    ServerFlag = true;

                    toolStripTextBox_MyNum.Text = $"Player{mySocketNo.ToString()}";
                    
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

                    if (data.StartsWith("exit"))
                    {
                        string exitType = data.Substring("exit:".Length);

                        switch (exitType)
                        {
                            case "opponent_left":
                                if (MessageBox.Show("상대방이 접속을 종료하였습니다.\n다시 하시겠습니까? (YES or No)", $"Player{mySocketNo}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    network.SendMessage(COMMAND[2], mySocketNo, serverNo, "retry", "recv 에서 COMMAND[3] messagebox yes");
                                }
                                else
                                {
                                    network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString(), "recv 에서 COMMAND[3] messagebox no");
                                }
                                break;
                        }
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
                        OppTurnFlag = false;
                        return;
                    }
                  
                    if (sendNo == opponentNo && recvNo == mySocketNo && !MyTurnFlag)
                    {
                        MyTurnFlag = true;
                        OppTurnFlag = false;
                        if (opp_state == -1 ) opp_state = 0;
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
                        network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString() , "recv 에서 상대가 기권");
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

                        if (MessageBox.Show($"{result}\n다시 하시려면 Yes 를, 그만 하고 싶으시면 No 를 눌러주세요", $"Player{mySocketNo}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            network.SendMessage(COMMAND[2], mySocketNo, mySocketNo, "retry", "recv 에서 messagebox yes");
                        }
                        else
                        {
                            network.SendMessage(COMMAND[1], mySocketNo, mySocketNo, Level.ToString(), "recv 에서 messagebox no");
                        }
                    }
                }

                else if (command.Equals(COMMAND[6]))
                {
                    if (data.StartsWith("error:"))
                    {
                        string errorType = data.Substring("error:".Length);

                        switch (errorType)
                        {
                            case "already_in_game":
                                MessageBox.Show("이미 게임 중입니다.");
                                break;
                            case "opponent_left":
                                MessageBox.Show("상대방이 게임에서 나갔습니다.");
                                GameOver(); // 또는 적절한 게임 종료 처리
                                break;
                            case "서버종료":
                                server_off();
                                MessageBox.Show("서버가 종료되었습니다.\n모든 연결이 끊겨 재접속 전까지 혼자하기만 가능합니다", "알림");
                                break;
                            default:
                                MessageBox.Show("알 수 없는 오류가 발생했습니다.");
                                break;
                        }
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
                            network.SendMessage(COMMAND[6], mySocketNo, serverNo, "소켓번호", "recv 에서 마지막 else");
                    }
                }
            }
            processedMessages.Clear();
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
                network.SendMessage(COMMAND[3], mySocketNo, opponentNo, "exit", "종료하기");
            }

            Application.Exit();
        }

        private void NetworkConnection()
        {
            if (network == null)
            {
                network = new Network();

                network.Connect();

                if (!network.IsConnected())
                {
                    //network.Dispose();
                    network.Disconnect();
                    network = null;
                    MessageBox.Show("서버와 연결을 실패했습니다");
                }
                else
                {
                    //MessageBox.Show("서버와 연결되었습니다.");
                }
            }
            else if (network.IsConnected()) return;
        }

        private void 기권하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null) return;
            if (!network.IsConnected()) return;

            if (mySocketNo != -1 && opponentNo != -1)
                SendGameoverCommand( mySocketNo, opponentNo, "retire", "기권하기");

            if (network.IsReallyConnected())  // network.Socket은 내부의 Socket 객체라 가정
            {
                network.SendMessage(COMMAND[1], mySocketNo, opponentNo, Level.ToString(), "기권하기");
                MessageBox.Show("대기모드로 전환합니다");
            }
            else
            {
                network.Dispose();
                MessageBox.Show("서버가 비정상적으로 종료되어 연결이 종료됩니다.");
            }
        }

        void server_off()
        {
            network.Dispose();
            network = null;
            lb_Turn.Text = "싱글 모드";


            GameOver();
            GameReset();
            GameStart();
        }

        private void 시작하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkConnection();
            if (network == null || !network.IsReallyConnected())
            {
                if (MessageBox.Show("네트워크 연결이 안되어있습니다. 연결 후 다시 시도해주세요", "알림", MessageBoxButtons.OK) == DialogResult.OK)
                    return;
            }

            ChooseLevel();
            network.SendMessage(COMMAND[1], mySocketNo, serverNo, Level.ToString(), "시작하기");
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

        private async void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (Mine btn in buttons)
            {
                if (btn.Bounds.Contains(e.Location))
                {
                    if (btn.Enabled)
                    {
                        return; // Button_Clicked_Event에서만 처리되게 함
                    }
                    break;
                }
            }

            if (superClickFlag) return;

            Mine? button = Find_Button_Coordinates(e.Location);

            if (button == null) return;

            if ( SoloFlag)
            {                          
                int result = OpenSurroundingButtons(button, Level);
                if (result == 0) return;

                if (result == 1) GameOver();
                else if (result == 2) GameClear();

                GameReset();
                GameStart();
            }
            else
            {
                int result = OpenSurroundingButtons(button, Level);
                if (result == 0) return;

                SendClickCommand(mySocketNo, opponentNo);

                if (result == 1 )
                {
                    await MultiGameoverAsync();
                }
                else if (result == 2)
                {
                    await MultiGameClearAsync();
                }
            }
        }

        private Mine? Find_Button_Coordinates(Point pt)
        {
            foreach (Mine m in buttons)
            {
                if (m.PtInRect(pt)) return m;
            }
            return null;
        }
    }
}

