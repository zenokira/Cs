using System;
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
    public enum GAMESTATE { 진행전, 게임중, 게임오버, 클리어, 보류 };
    public enum GAMERESULT { 진행중 = -1, 무승부 = 0, 승리 = 1, 패배 = 2, 보류 = 3 }
    public enum PLAYERORDER { 선공, 후공 }

    public enum CommandType { Server, Client, Game, System };
    /*
    client: 클라이언트 간 상호작용 (level 선택, retry 요청 등)
    game: 게임 내 동작(click, result ) [ click - alive, clear, dead   result - win, lose, draw ]
    server: 서버 메시지 ( recv 만 가능하게 절대 이걸로 send 하기 없기 )
    system: 연결/종료 관련 등
    */
    public enum ServerAction { OpponentInfo, TurnInfo }
    public enum ClientAction { Retry, Level, LeaveRoom, Exit };
    public enum GameAction { Click, Result, FirstClick, MineMap, ReGame }
    public enum SystemAction { Connect, Exit, Error, Disconnect, CloseRoom, Wait };
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
        int solo_level = 0; int multi_level = 0;
        int timer = 0;
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
        bool ServerFlag;
        bool SoloFlag;


        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";

        bool superClickFlag = false;

        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetMystate();
            GameStart(solo_level);
            SetToolStripMenuVisible(false);
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

        void Button_Right_Clicked_Event(object? sender, MouseEventArgs e)
        {
            Mine button = (Mine)sender;
            if (button == null || buttons == null) return;

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
        void GameStart(int level)
        {
            FormResize(level);
            InitGamePan(level);
            RandomLayingMine(level);
            DetectorMine(level);

            gameoverFlag = false;
            SoloFlag = true;
            MineFlagCnt = MINE_FLAG_COUNT[level];
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

        void test(Mine[,] buttons)
        {
            for ( int y = 0; y < buttons.GetLength(0); y++)
            {
                for (int x = 0; x < buttons.GetLength(1); x++)
                {
                    Console.Write(buttons[y, x].isEnabled().ToString().PadRight(6));
                }
                Console.WriteLine();
            }
        }

        void DetectorZeroButton( Point pt, int level)
        {
            test(buttons);
            HashSet<Point> visited = new HashSet<Point>();
            Queue<Point> queue = new();
            queue.Enqueue(pt);
            visited.Add(pt);


            while (queue.Count != 0)
            {
                Point point = queue.Dequeue();
                if (!buttons[point.Y, point.X].isEnabled()) continue;
                buttons[point.Y, point.X].ButtonClick();
                for (int w = 0; w < WAYS; w++)
                {
                    int nx = point.X + WAY[w, 0], ny = point.Y + WAY[w, 1];
                    if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;
                    
                    Point neighbor = new Point(nx, ny);
                    if (visited.Contains(neighbor)) continue;  // 중복 추가 방지

                    Mine btn = buttons[ny, nx];
                    if (btn.isMineHave()) continue;
                    if (!btn.isZeroCount())
                    {
                        Console.WriteLine($"DetectZeroButton: 좌표({point.X},{point.Y}) 상태 Enabled={buttons[point.Y, point.X].isEnabled()} 열기 시도");
                        buttons[ny, nx].ButtonClick();
                        this.Refresh();
                        continue;
                    }
                    queue.Enqueue(new Point(nx, ny));
                }
            }
            test(buttons);
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
            button.Enabled = true;


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

                // 1. 클릭 후보 확인 후 클릭 후보 확인
                if (buttons[ny, nx].isMineFlagOn()) flag_cnt++;
            }

            // 3. 클릭 후보 확인 후 클릭 후보 확인
            if (buttons[pt.Y, pt.X].GetArroundMineCount() != flag_cnt) return 0;

            // 4. 클릭 후보 확인
            for (int w = 0; w < WAYS; w++)
            {
                int nx = pt.X + WAY[w, 0], ny = pt.Y + WAY[w, 1];
                if (nx < 0 || nx >= GAMEPAN[level, 0] || ny < 0 || ny >= GAMEPAN[level, 1]) continue;

                Mine btn = buttons[ny, nx];

                // 5. 클릭 후보 확인 후 클릭 후보 확인
                if (btn.isCorrectMineFlag()) continue;

                if (btn.isZeroCount())
                {
                    DetectorZeroButton(new Point(nx, ny), level);
                }

                if (btn.isMineHave())
                {
                    return 1;
                }

                if (btn.isEnabled()) btn.ButtonClick();

                if (isGameClear(level))
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

            if (buttons == null) return;

            for (int y = 0; y < buttons.GetLength(0); y++)
            {
                for (int x = 0; x < buttons.GetLength(1); x++)
                {
                    Controls.Remove(buttons[y, x]);
                    buttons[y, x].Enabled = true;
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
            GameStart(solo_level);
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
                if (level != -1)
                {
                    if (SoloFlag) solo_level = level;

                    if (SoloFlag && ServerFlag) solo_level = level;
                    else multi_level = level;
                }
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
            CommandType command;
            string data, action;
            int sendNo, recvNo;


            if (!IsCorrectData(o)) return;

            List<string> list = GetRecvMultiLine(o);
            HashSet<string> processedMessages = new HashSet<string>();

            for (int i = 0; i < list.Count; i++)
            {
                string message = list[i];

                if (processedMessages.Contains(message)) continue;

                processedMessages.Add(message);

                ParsingRecvData(message, out command, out sendNo, out recvNo, out action, out data);
                LogPacket(o);

                switch (command)
                {
                    case CommandType.Server:
                        HandleServerCommand(sendNo, recvNo, action, data);
                        break;
                    case CommandType.Client:
                        HandleClientCommand(sendNo, recvNo, action, data);
                        break;
                    case CommandType.Game:
                        HandleGameCommand(sendNo, recvNo, action, data);
                        break;
                    case CommandType.System:
                        HandleSystemCommand(sendNo, recvNo, action, data);
                        break;
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

        void NetWorkGameInit()
        {
            DetectorMine(multi_level);
            gameoverFlag = false;
            SoloFlag = false;
            networkPlayFlag = true;
            MineFlagCnt = MINE_FLAG_COUNT[multi_level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();

            toolStripTextBox_MyNum.Text = $"Player{mySocketNo + 1}";

            SetMystate();
            networkPlayFlag = true;

            if (!MyTurnFlag) timer_PlayTime.Stop();
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
        bool ParsingRecvData(string recvData, out CommandType command, out int sendNo, out int recvNo, out string action, out string data)
        {
            command = default;
            sendNo = 0;
            recvNo = 0;
            action = null;
            data = string.Empty;

            string eData = ExtractionData(recvData);
            if (string.IsNullOrEmpty(eData))
                return false;

            string[] s = eData.Split(' ');

            if (s.Length < 5)
                return false;

            if (!WhatCommandEnum(s[0], out command))
                return false;

            if (!int.TryParse(s[1], out sendNo))
                return false;

            if (!int.TryParse(s[2], out recvNo))
                return false;

            // action 변환
            action = s[3];

            // data 처리
            if (s.Length > 5)
                data = ConcatData(s, 4);  // s[4]부터 끝까지 이어붙이기
            else
                data = s[4];

            return true;
        }


        string ExtractionData(string data)
        {
            string eData = data.Replace(START_WORD, "");
            eData = eData.Replace(END_WORD, "");
            return eData;
        }

        string ConcatData(string[] data, int startIndex)
        {
            if (data.Length <= startIndex)
                return string.Empty;

            return string.Join(" ", data, startIndex, data.Length - startIndex);
        }


        private void 지뢰찾기_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (network == null) return;

            if (network.IsSocketNULL()) return;

            if (network.IsConnected())
            {
                network.SendMessage(CommandType.System, mySocketNo, opponentNo, SystemAction.Exit, "exit", "보류");
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
                    MessageBox.Show("네트워크 연결에 실패했습니다");
                }
                else
                {
                    //MessageBox.Show("보류 성공했습니다.");
                }
            }
            else if (network.IsConnected()) return;
        }

        private void 기권하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null) return;
            if (!network.IsConnected()) return;
            if (opponentNo == -1) return;
            
            if ( MessageBox.Show("기권 시 네트워크 연결이 종료됩니다. 싱글모드로 돌아가시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Exit, "연결종료", "기권");
                ServerFlag = false;
                SoloFlag = true;
                SetMystate();
            }
        }

        private void 시작하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkConnection();
            if (network == null || !network.IsReallyConnected())
            {
                if (MessageBox.Show("게임을 강제종료하려면 다음 게임을 준비하세요. 이전 게임을 하시겠습니까", "오류", MessageBoxButtons.OK) == DialogResult.OK)
                    return;
            }
            ServerFlag = true;

            if (ServerFlag )
            {
                SetToolStripMenuVisible(true);
                SetMystate();
            }
        }

        void SetMystate()
        {
            string text = "";
            if ( ServerFlag )
            {
                if (SoloFlag) text = "매칭 대기중";
                else {
                    if (MyTurnFlag) text = "내 차례";
                    else text = "상대 차례";
                }
            }
            else
            {
                text = "싱글 모드";
            }
            
            SafeInvoke(lb_Turn, () => lb_Turn.Text = text);
        }

        void SetToolStripMenuVisible(bool visible)
        {
            방나가기ToolStripMenuItem.Visible = visible;
            기권하기ToolStripMenuItem.Visible = visible;
        }
        public void LogPacket(string message)
        {
            try
            {
                // 로그 파일 생성 후 로그 작성
                using (StreamWriter writer = new StreamWriter($"network_log{mySocketNo}.txt", true))
                {
                    // 로그 작성 시간 추가
                    writer.WriteLine($"{message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그 작성 오류: {ex.Message}");
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

        private void ChooseMatchingLevel()
        {
            if (mySocketNo == -1) return;

            ChooseLevel();
            if (SoloFlag) multi_level = solo_level;

            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level, multi_level.ToString(), "난이도 선택");
            GameReset();
            GameStart(multi_level);
            SetMystate();
        }

        private async void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
            if (buttons == null) return;
            foreach (Mine btn in buttons)
            {
                if (btn.Bounds.Contains(e.Location))
                {
                    if (btn.Enabled)
                    {
                        return; // Button_Clicked_Event 호출 후 종료
                    }
                    break;
                }
            }

            if (superClickFlag) return;

            Mine? button = Find_Button_Coordinates(e.Location);

            if (button == null) return;

            if (SoloFlag)
            {
                int result = OpenSurroundingButtons(button, solo_level);
                if (result == 0) return;

                if (result == 1)
                {
                    GameOver();
                    MessageBox.Show("지뢰를 밟으셨습니다", "알림", MessageBoxButtons.OK);
                }

                else if (result == 2)
                {
                    GameClear();
                    MessageBox.Show("클리어 하셨습니다", "알림", MessageBoxButtons.OK);
                }

                GameReset();
                GameStart(solo_level);
            }
            else
            {

                if (!MyTurnFlag) return;
                Console.WriteLine("[MouseClick] MyTurnFlag = false");
                int result = OpenSurroundingButtons(button, multi_level);
                if (result == 0) return;

                MyTurnFlag = false;
                SetMystate();

                if (timer_PlayTime.Enabled) timer_PlayTime.Stop();

                if (result == 1)
                {
                    network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "자동열기");
                }
                else if (result == 2)
                {
                    network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer.ToString()}", "자동열기");
                }
            }
        }
        private void Button_Clicked_Event(object? sender, EventArgs e)
        {
            Mine mine = sender as Mine;
            if (mine == null || !mine.isEnabled())
                return;

            if (SoloFlag)
            {
                HandleSoloMode(mine);
            }

            else
            {
                if (!MyTurnFlag) return;
                HandleMultiClick(mine);
            }
        }

        private void HandleSoloMode(Mine mine)
        {
            if (mine.isMineHave())
            {
                GameOver();
                ChooseLevel();
                GameReset();
                GameStart(solo_level);
                return;
            }
            else if (mine.isZeroCount())
            {
                DetectorZeroButton(mine.GetButtonXYPoint(), solo_level);
            }
            else
            {
                mine.ButtonClick();
            }

            if (isGameClear(solo_level))
            {
                GameClear();
                ChooseLevel();
                GameReset();
                GameStart(solo_level);
            }
        }

        private void 방나가기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null || network.IsSocketNULL()) return; 
            if ( MessageBox.Show("현재 방에서 나가 대기방으로 가시겠습니까?\nYes or No", "방나가기", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                opponentNo = -1;
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.LeaveRoom, "Leave", "방나가기");
            }
            else
            {
                return;
            }
        }

        void HandleMultiClick(Mine mine)
        {
            if (timer_PlayTime.Enabled) timer_PlayTime.Stop();
            MyTurnFlag = false;
            SetMystate();
            Console.WriteLine($"[HandleMultiClick] MyTurnFlag = false {MyTurnFlag}");
            if (mine.isMineHave())
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "버튼 클릭 멀티");
                GameOver();
                Application.DoEvents();
                return;
            }
            else if (mine.isZeroCount())
            {
                DetectorZeroButton( mine.GetButtonXYPoint(), multi_level);
            }
            else
            {
                mine.ButtonClick();
            }

            if (isGameClear(multi_level))
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer.ToString()}", "버튼 클릭 멀티");
            }
            else
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Click, "Alive", "버튼 클릭 멀티");
            }
        }
        bool WhatCommandEnum(string str, out CommandType command)
        {
            switch (str)
            {
                case "Server":
                    command = CommandType.Server;
                    return true;
                case "Client":
                    command = CommandType.Client;
                    return true;
                case "Game":
                    command = CommandType.Game;
                    return true;
                case "System":
                    command = CommandType.System;
                    return true;
                default:
                    command = default;
                    return false;
            }
        }


        /// <summary>
        /// CommantType Handler 모음
        /// </summary>
        /// <param name="sendNo"></param>
        /// <param name="recvNo"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        void HandleServerCommand(int sendNo, int recvNo, string action, string data)
        {
            // ServerAction { OpponentInfo, TurnInfo };

            if (action == "OpponentInfo")
            {
                HandleOpponentInfoAction(sendNo);
            }
            else if (action == "TurnInfo")
            {
                HandleTurnInfoAction(data);
            }
        }
        private void HandleTurnInfoAction(string data)
        {
            int turnSock = Convert.ToInt32(data);

            MyTurnFlag = turnSock == mySocketNo;

            GameReset();
            FormResize(multi_level);
            InitGamePan(multi_level);
            lb_Turn.Location = new Point(this.Size.Width / 2 - lb_Turn.Width / 2, menuStrip1.Height);
        }

        private void HandleOpponentInfoAction(int no)
        {
            opponentNo = no;
        }

        void HandleClientCommand(int sendNo, int recvNo, string action, string data)
        {
            // ClientAction { Retry, Level, LeaveRoom, Exit };

            switch(action)
            {
                case "Exit":
                    HandleExitCommand();
                    break;

            }
        }
        void HandleExitCommand()
        {
            network.Dispose();
            networkPlayFlag = false;

            GameReset();
            GameStart(solo_level);
        }

        void HandleGameCommand(int sendNo, int recvNo, string action, string data)
        {
            // Click, Result, FirstClick, MineMap

            switch (action)
            {
                case "Click":
                    HandleClickCommand();
                    break;
                case "Result":
                    HandleResultCommand(sendNo, recvNo, data);
                    break;
                case "FirstClick":
                    HandleFirstClickCommand(data);
                    break;
                case "MineMap":
                    HandleMineMapCommand(data);
                    break;
                case "ReGame":
                    HandleReGameCommand();
                    break;
            }
        }

        private void HandleReGameCommand()
        {
            GameReset();
        }

        void HandleSystemCommand(int sendNo, int recvNo, string action, string data)
        {
            // SystemAction { Connect, Exit, Error, Disconnect };
            switch (action)
            {
                case "Connect":
                    HandleConnectAction(data);
                    break;
                case "Exit":
                    break;
                case "Error":
                    break;
                case "Disconnect":
                    break;
                case "CloseRoom":
                    HandleCloseRoomAction();
                    break;
                case "Wait":
                    HandleWaitAction();
                    break;
            }
        }

        private void HandleWaitAction()
        {
            SoloFlag = true;
            GameStart(solo_level);
            SetMystate();
        }

        void HandleCloseRoomAction()
        {
            opponentNo = -1; 
            GameOver();
            GameReset();
        }

        void HandleConnectAction(string data)
        {
            if (mySocketNo != -1) return;

            int no = Convert.ToInt32(data);
            mySocketNo = no;

            this.Invoke(new Action(() =>
            {
                ChooseMatchingLevel();
            }));
        }
        void HandleClickCommand()
        {
            Console.WriteLine("혹시 버튼 클릭했는데 받는건가?");
            MyTurnFlag = true;
            SetMystate();
            if (!timer_PlayTime.Enabled) timer_PlayTime.Start();
        }

        void HandleResultCommand(int sendNo, int recvNo, string data)
        {
            string result = "";
            switch (data)
            {
                case "Win":
                    result = "승리하셨습니다.";
                    break;
                case "Lose":
                    result = "패배하셨습니다.";
                    break;
                case "Draw":
                    result = "무승부입니다.";
                    break;
            }

            if (MessageBox.Show($"{result}\n다시 하시려면 Yes 대기실로 돌아가시려면 No 를 눌러주세요", $"Player{mySocketNo + 1} 결과", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.ReGame, "", "결과");
            }
            else
            {
                network.SendMessage(CommandType.Client, mySocketNo, opponentNo, ClientAction.LeaveRoom, "", "결과");
            }
        }
        void HandleFirstClickCommand(string data)
        {
            string[] pt = data.Split("*");
            int x = Convert.ToInt32(pt[0]);
            int y = Convert.ToInt32(pt[1]);
            Point firstclick = new Point(x, y);

            Mine mine = buttons[firstclick.Y, firstclick.X];
            if (mine == null) return;

            if (mine.isZeroCount())
                DetectorZeroButton( firstclick, multi_level);
            else
                mine.ButtonClick();
        }
        void HandleMineMapCommand(string data)
        {
            List<Point> mine = MineLaying(data);

            foreach (Point pt in mine)
            {
                buttons[pt.Y, pt.X].LayingMine();
            }

            NetWorkGameInit();
        }


    }
}

