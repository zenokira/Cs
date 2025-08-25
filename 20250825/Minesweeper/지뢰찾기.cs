using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Label = System.Windows.Forms.Label;

namespace Minesweeper
{
    public enum GAMESTATE { 진행전, 게임중, 게임오버, 클리어, 보류 };
    public enum PLAYERORDER { 선공, 후공 }

    public enum CommandType { Server, Client, Game, System };
    /*
    client: 클라이언트 간 상호작용 (level 선택, retry 요청 등)
    game: 게임 내 동작(click, result ) [ click - alive, clear, dead   result - win, lose, draw ]
    server: 서버 메시지 ( recv 만 가능하게 절대 이걸로 send 하기 없기 )
    system: 연결/종료 관련 등
    */
    public enum ServerAction { OpponentInfo, TurnInfo, Matching }
    public enum ClientAction { Retry, Level, LeaveRoom, Reservation, Exit };
    public enum GameAction { Click, Result, FirstClick, MineMap, ReGame }
    public enum SystemAction { Connect, Exit, Error, Disconnect, CloseRoom, Wait };

    

    public partial class 지뢰찾기 : Form, IGameUI
    {
        int timer = 0;
        int serverNo = -9999;


        Network? network;
        GameManager gameManager;
        int mySocketNo = -1;
        int opponentNo = -1;
        bool MyTurnFlag = false;
        bool ServerFlag;
        bool SoloFlag;
        bool gamemode_multi = false;
        bool gameinprogress = false;




        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";

        Mine[,] buttons;

        Alert alert;





        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            UpdateState();
            SoloGameInit();
            SetToolStripMenuVisible(false);
        }

        private void Initialize()
        {
            gameManager = new();
            InitHandlers();
        }

        void InitHandlers()
        {
            gameHandler = new GameCommandHandler(this);
            systemHandler = new SystemCommandHandler(this);
            clientHandler = new ClientCommandHandler(this);
            serverHandler = new ServerCommandHandler(this);
        }
        private void SafeInvoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
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
                    if (flag.Equals(gameManager.GetMineFlag(1)) || flag.Equals(gameManager.GetMineFlag(2))) cnt++;
                }
                toolStripTextBox_MineFlag.Text = (gameManager.GetMineFlagCnt(!gamemode_multi) - cnt).ToString();
            }
            else
            {

            }
        }
        void FormResize(int level)
        {
            Size size = this.Size - this.ClientSize; ;
            var (rows, cols) = gameManager.GetGamePanSize(level);

            size.Width += gameManager.ButtonStartX * 2 + gameManager.ButtonSize * rows;
            size.Height += gameManager.ButtonStartY + gameManager.ButtonSize * cols + 메뉴ToolStripMenuItem.Height;

            this.Size = size;
        }
        void SoloGameInit()
        {
            int level = gameManager.SOLO_LEVEL;
            FormResize(level);
            MakeGamePanButton(level);

            gameManager.SoloGameInit(buttons);

            SoloFlag = true;
            gameManager.MINE_FLAG_CNT = gameManager.MineCount[level];
            toolStripTextBox_MineFlag.Text = gameManager.MINE_FLAG_CNT.ToString();
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }
        void OpenZeroButton(Point clckpt, bool isSolo)
        {

            List<Point> toOpen = gameManager.GetButtonsToOpenByZeroDetector(buttons, clckpt, isSolo);
            foreach (Point pt in toOpen)
            {
                buttons[pt.Y, pt.X].ButtonClick(); // 이건 UI가 책임
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
            button.Size = new Size(gameManager.ButtonSize, gameManager.ButtonSize);
            button.Text = " ";
            button.Click += Button_Clicked_Event;
            button.MouseUp += Button_Right_Clicked_Event;
            button.Enabled = true;


            return button;
        }

        void MakeGamePanButton(int level)
        {
            var (nx, ny) = gameManager.GetGamePanSize(level);
            buttons = new Mine[ny, nx];
            for (int y = 0; y < ny; y++)
            {
                for (int x = 0; x < nx; x++)
                {
                    buttons[y, x] = MakeButton(x, y);
                    Controls.Add(buttons[y, x]);
                }
            }

            Console.WriteLine("새 버튼 생성 후 Controls 개수: " + Controls.Count);
        }



        Point ButtonLocationPoint(int x, int y)
        {
            return new Point(gameManager.ButtonStartX + gameManager.ButtonSize * x, gameManager.ButtonStartY + gameManager.ButtonSize * y);
        }
        private void timer_PlayTime_Tick(object sender, EventArgs e)
        {
            timer++;
            toolStripTextBox_Timer.Text = timer.ToString();
        }
        private GAMESTATE OpenSurroundingButtons(Mine button, bool isSolo)
        {
            List<Point> toOpen = gameManager.GetOpenableSurroundingPoints(buttons, button.GetButtonXYPoint(), isSolo);

            foreach (Point pt in toOpen)
            {
                Mine btn = buttons[pt.Y, pt.X];

                if (btn.isMineHave())
                {
                    return GAMESTATE.게임오버;
                }

                if (btn.isZeroCount())
                    OpenZeroButton(pt, isSolo);
                else if (btn.isEnabled())
                {
                    btn.ButtonClick();
                }
            }

            if (gameManager.IsGameClear(buttons, true))
            {
                return GAMESTATE.클리어;
            }

            return GAMESTATE.게임중;
        }

        private void GameClear()
        {
            try
            {
                if (timer_PlayTime.Enabled)
                {
                    timer_PlayTime.Stop();
                    timer = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception GameClear: " + ex.Message);
            }
        }

        private void GameOver()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine("Exception GameOver: " + ex.Message);
            }
        }

        private void ButtonReset()
        {
            try
            {
                timer_PlayTime.Stop();
                timer = 0;

                if (buttons == null) return;

                foreach (var ctrl in Controls.OfType<Mine>().ToList())
                {
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                buttons = null;

                Console.WriteLine("Controls 개수: " + Controls.Count);
                foreach (Control ctrl in Controls)
                {
                    Console.WriteLine($"남은 컨트롤: {ctrl.GetType()} {ctrl.Location}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception ButtonReset: " + ex.Message);
            }
        }
        private void 새로시작ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServerFlag) return;
            ButtonReset();
            SoloGameInit();
        }

        private void 난이도설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ServerFlag) // 싱글모드
            {
                // 난이도 선택 가능
                ChooseLevel();
                ButtonReset();
                SoloGameInit();
            }
            else // 멀티모드
            {
                if (!SoloFlag && GameInProgress) // 멀티 게임 진행 중
                {
                    using (Alert alert = new Alert(
                        "대전중에는 난이도 변경을 할 수 없습니다.",
                        "알림",
                        "OK"))
                    {
                        alert.ShowDialog();
                    }
                    return;
                }
                else // 멀티 매칭 대기중
                {
                    ChooseAndSendLevel();
                    ButtonReset();
                    SoloGameInit();
                    SetToolStripMenuVisible(true);
                    UpdateState();
                }
            }
        }
        private void ChooseLevel()
        {
            Setting settingForm = new();
            if (settingForm.ShowDialog() == DialogResult.OK)
            {
                int level = settingForm.GetLevel();
                if (level != -1)
                {
                    if (ServerFlag && !SoloFlag)
                        gameManager.MULTI_LEVEL = level;
                    gameManager.SOLO_LEVEL = level;
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
            try
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
                            serverHandler.HandleServerCommand(sendNo, recvNo, action, data);
                            break;
                        case CommandType.Client:
                            clientHandler.HandleClientCommand(sendNo, recvNo, action, data);
                            break;
                        case CommandType.Game:
                            gameHandler.HandleGameCommand(sendNo, recvNo, action, data);
                            break;
                        case CommandType.System:
                            systemHandler.HandleSystemCommand(sendNo, recvNo, action, data);
                            break;
                    }
                }
                processedMessages.Clear();
            }
            catch ( Exception ex )
            {
                Console.WriteLine($"RecvDataUse 예외: {ex}");
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

        void NetWorkGameInit()
        {

            toolStripTextBox_MineFlag.Text = gameManager.MINE_FLAG_CNT.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();

            toolStripTextBox_MyNum.Text = $"Player{mySocketNo + 1}";

            UpdateState();


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
            if (network == null || !network.IsConnected())
            {
                // 기존 연결이 있다면 안전하게 종료
                if (network != null)
                {
                    try
                    {
                        network.Disconnect();
                        network.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"기존 연결 종료 중 오류: {ex.Message}");
                    }
                    finally
                    {
                        network = null;
                    }
                }

                // 새로운 연결 시도
                network = new Network();
                network.Connect();

                if (!network.IsConnected())
                {
                    network.Disconnect();
                    network.Dispose();
                    network = null;

                    Alert alert = new Alert("네트워크 연결에 실패했습니다");
                    alert.ShowDialog();
                }
                else
                {
                    Console.WriteLine("서버 연결 성공");
                }
            }
        }

        private void 나가기예약ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null) return;
            if (!network.IsConnected()) return;
            if (opponentNo == -1) return;

            Alert alert = new Alert("나가기를 예약합니다.\n멀티모드를 완전히 종료하고 싶으시면 Multy_Exit\n매칭상태로 돌아가고 싶다면 Lobby\n예약없이 게임으로 돌아가시려면 Cancel 를 눌러주세요", "알림", "Reservation");
            DialogResult result = alert.ShowDialog();
            if (result == DialogResult.Yes)
            {
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Reservation, "Exit", "기권");
            }
            else if (result == DialogResult.No)
            {
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Reservation, "LeaveRoom", "기권");
            }
            else
            {
                return;
            }
        }

        private async void 시작하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameinprogress) return;

            NetworkConnection();
            if (network == null || !network.IsReallyConnected())
            {
                Alert alert = new Alert("게임을 강제종료하려면 다음 게임을 준비하세요. 이전 게임을 하시겠습니까", "오류","OK");
                alert.ShowDialog();
                return;
            }
            ServerFlag = true;
            if (ServerFlag)
            {
                await ChooseAndSendLevel();
                ButtonReset();
                
                SoloGameInit();
                SetToolStripMenuVisible(true);
                UpdateState();
            }
        }

        async Task ChooseAndSendLevel()
        {
            await ChooseLevelasync();
            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level, gameManager.MULTI_LEVEL.ToString(), "Choose & Send");
        }

        async Task ChooseLevelasync()
        {
            Setting settingForm = new();
            if (settingForm.ShowDialog() == DialogResult.OK)
            {
                int level = settingForm.GetLevel();
                if (level != -1)
                {
                    if (ServerFlag)
                        gameManager.MULTI_LEVEL = level;
                    gameManager.SOLO_LEVEL = level;
                }
            }
        }

        void UpdateState()
        {
            string level = gameManager.MULTI_LEVEL switch
            {
                0 => "초급 난이도",
                1 => "중급 난이도",
                2 => "고급 난이도"
            };

            SafeInvoke(() => lb_Turn.ForeColor = Color.Black );

           string text = "";
            if (ServerFlag)
            {
                if (SoloFlag)
                {
                    text = $"{level} 매칭 대기중";
                    gamemode_multi = false;
                }
                else
                {
                    gamemode_multi = true;
                    if (MyTurnFlag)
                    {
                        SafeInvoke(() => lb_Turn.ForeColor = Color.DarkCyan);
                        text = "내 차례";
                    }
                    else text = "상대 차례";
                }
            }
            else
            {
                gamemode_multi = false;
                text = "싱글 모드";
            }

            SafeInvoke(() => lb_Turn.Text = text);
            lb_Turn.Location = new Point(this.Size.Width / 2 - lb_Turn.Width / 2, menuStrip1.Height);
        }

        void SetToolStripMenuVisible(bool visible)
        {
            나가기ToolStripMenuItem.Visible = visible;
            나가기예약ToolStripMenuItem.Visible = visible;
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



            Mine? button = Find_Button_Coordinates(e.Location);

            if (button == null) return;

            if (SoloFlag)
            {
                GAMESTATE gamestate = OpenSurroundingButtons(button, true);


                Alert alert;
                switch (gamestate)
                {
                    case GAMESTATE.게임오버:
                        GameOver();
                        alert = new Alert("지뢰를 밟으셨습니다");
                        alert.ShowDialog();
                        break;
                    case GAMESTATE.클리어:
                        GameClear();
                        alert = new Alert("클리어 하셨습니다");
                        alert.ShowDialog();
                        break;
                    case GAMESTATE.게임중:
                        return;
                }

                ChooseLevel();
                ButtonReset();
                SoloGameInit();
            }
            else
            {

                if (!MyTurnFlag) return;
                Console.WriteLine("[MouseClick] MyTurnFlag = false");
                GAMESTATE gamestate = OpenSurroundingButtons(button, false);

                switch (gamestate)
                {
                    case GAMESTATE.게임오버:
                        network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "자동열기");
                        GameOver();
                        break;
                    case GAMESTATE.클리어:
                        network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer}", "자동열기");
                        GameClear();
                        break;
                    case GAMESTATE.게임중:
                        return;
                }

                if (timer_PlayTime.Enabled) timer_PlayTime.Stop();
                MyTurnFlag = false;
                UpdateState();
            }
        }
        private void Button_Clicked_Event(object? sender, EventArgs e)
        {
            Mine mine = sender as Mine;
            if (mine == null || !mine.isEnabled())
                return;

            if (!Controls.Contains(mine))
            {
                Console.WriteLine("⚠ 클릭된 버튼은 Controls에 없음 (옛날 버튼)");
            }

            if (SoloFlag)
            {
                HandleSoloMode(mine);
            }

            else
            {
                if (!MyTurnFlag) return;

                Console.WriteLine($"클릭: {mine.GetButtonXYPoint()} {mine.GetHashCode()}");
                chckbutton(mine);
                Console.WriteLine($"mine : {mine.Enabled} same : {buttons[0, 0].Enabled}");
                HandleMultiClick(mine);
            }
        }

        void chckbutton(Mine mine)
        {
            foreach (var b in buttons)
            {
                Console.WriteLine($"배열: {b.GetButtonXYPoint()} {b.GetHashCode()}");
                if (b.Equals(mine))
                {
                    Console.WriteLine($"일치 {b.GetButtonXYPoint()}");
                }
            }
        }
        private void HandleSoloMode(Mine mine)
        {
            GAMESTATE gamestate = GAMESTATE.게임중;
            if (mine.isMineHave())
            {
                GameOver();
                Alert alert = new Alert("지뢰를 밟으셨습니다");
                alert.ShowDialog();
                gamestate = GAMESTATE.게임오버;
            }
            else if (mine.isZeroCount())
            {
                OpenZeroButton(mine.GetButtonXYPoint(), true);
            }
            else
            {
                mine.ButtonClick();
                mine.Refresh();
            }

            if (gameManager.IsGameClear(buttons, true))
            {
                GameClear();
                Alert alert = new Alert("클리어 하셨습니다");
                alert.ShowDialog();
                gamestate = GAMESTATE.클리어;
            }

            if (gamestate == GAMESTATE.게임오버 || gamestate == GAMESTATE.클리어)
            {
                ChooseLevel();
                ButtonReset();
                SoloGameInit();
            }
        }

        private async void 나가기ToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            if (network == null || network.IsSocketNULL()) return;

            // 게임 중
            if (opponentNo != -1 && GameInProgress)
            {
                var alert = new Alert("현재 방에서 나가 대기방으로 가시겠습니까?", "방 나가기", "YesOrNo");
                if (alert.ShowDialog() == DialogResult.Yes)
                {
                    network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.LeaveRoom, "Leave", "방나가기");
                    opponentNo = -1;
                    ServerFlag = true;
                    SoloFlag = true;
                    UpdateState();
                    GameInProgress = false;
                }
            }

            else if ( !GameInProgress && opponentNo == -1)
            {
                var alert = new Alert("멀티플레이 모드를 종료하고 싱글 모드로 돌아가시겠습니까?", "멀티플레이 종료", "YesOrNo");
                if (alert.ShowDialog() == DialogResult.Yes)
                {
                    network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Exit, "Exit", "멀티종료");
                    opponentNo = -1;
                    ServerFlag = false;
                    SoloFlag = true;
                    UpdateState();
                    SetToolStripMenuVisible(false);
                }
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

        void HandleMultiClick(Mine mine)
        {
            if (timer_PlayTime.Enabled) timer_PlayTime.Stop();

            if (mine.isMineHave())
            {
                GameOver();
                MyTurnFlag = false;
                UpdateState();
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "버튼 클릭 멀티");
               
                Application.DoEvents();
                return;
            }
            else if (mine.isZeroCount())
            {
                OpenZeroButton(mine.GetButtonXYPoint(), false);
            }
            else
            {
                mine.ButtonClick();
                Console.WriteLine($"mine : {mine.Enabled} same : {buttons[0, 0].Enabled}");
            }

            if (gameManager.IsGameClear(buttons, gamemode_multi))
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer}", "버튼 클릭 멀티");
            }
            else
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Click, "Alive", "버튼 클릭 멀티");
            }
            MyTurnFlag = false;
            UpdateState();
            Console.WriteLine($"[HandleMultiClick] MyTurnFlag = {MyTurnFlag}");
        }

        // 최초 자동 클릭 1번
        void HandleFirstClickCommand(string data)
        {
            string[] pt = data.Split("*");
            int x = Convert.ToInt32(pt[0]);
            int y = Convert.ToInt32(pt[1]);
            Point firstclick = new Point(x, y);

            Mine mine = buttons[firstclick.Y, firstclick.X];
            if (mine == null) return;

            if (mine.isZeroCount())
                OpenZeroButton(firstclick, true);
            else
                mine.ButtonClick();
        }


        // 기존 private 메서드는 그대로 두고,
        // public 래퍼 메서드만 추가합니다.
        public int Timer { get => timer; set => timer = value; }
        public int ServerNo { get => serverNo; set => serverNo = value; }
        public int MySocketNo { get => mySocketNo; set => mySocketNo = value; }
        public int OpponentNo { get => opponentNo; set => opponentNo = value; }
        public bool MyTurnFlagProp { get => MyTurnFlag; set => MyTurnFlag = value; }
        public bool ServerFlagProp { get => ServerFlag; set => ServerFlag = value; }
        public bool SoloFlagProp { get => SoloFlag; set => SoloFlag = value; }
        public bool GamemodeMulti { get => gamemode_multi; set => gamemode_multi = value; }

        public bool GameInProgress { get => gameinprogress; set => gameinprogress = value; }
        public Network NetworkInstance { get => network; set => network = value; }
        public GameManager GameManagerInstance { get => gameManager; set => gameManager = value; }
        public Alert AlertInstance { get => alert; set => alert = value; }
        public ToolStripTextBox ToolStripTextBox_MineFlag => toolStripTextBox_MineFlag;
        public Label Lb_Turn => lb_Turn;
        public MenuStrip MenuStrip1 => menuStrip1;
        public Size FormSize => this.Size;

        public Mine[,] GetButtons()
        {
            return buttons;
        }

        public void SetButtons(Mine[,] value)
        {
            buttons = value;
        }

        public System.Windows.Forms.Timer Timer_PlayTime => timer_PlayTime;

        private GameCommandHandler gameHandler;
        private SystemCommandHandler systemHandler;
        private ClientCommandHandler clientHandler;
        private ServerCommandHandler serverHandler;
        public void CallButtonReset()
        {
            ButtonReset();
        }

        public void CallUpdateState()
        {
            UpdateState();
        }

        public void CallGameOver()
        {
            GameOver();
        }

        public void CallMakeGamePanButton(int level)
        {
            MakeGamePanButton(level);
        }

        public void CallSoloGameInit()
        {
            SoloGameInit();
        }

        public void CallNetWorkGameInit()
        {
            NetWorkGameInit();
        }

        public void CallChooseLevel()
        {
            ChooseLevel();
        }

        public void CallFormResize(int level)
        {
            FormResize(level);
        }
        public List<Point> CallMineLaying(string data)
        {
            return MineLaying(data);
        }

        public void CallSetToolStripMenuVisible(bool visible)
        {
            SetToolStripMenuVisible(visible);
        }
    }
}