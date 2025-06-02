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
    public enum ServerAction { Mine, Click }
    public enum ClientAction { Retry, Level, LeaveRoom, Exit };
    public enum GameAction { Click, Result }
    public enum SystemAction { Connect, Exit, Error, Disconnect };
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

        bool serverclickFlag;
        bool isHandledByButtonClick = false;


        readonly string START_WORD = "@@";
        readonly string END_WORD = "##";

        bool superClickFlag = false;

        User Me_info, Opp_info;


        public 지뢰찾기()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lb_Turn.Text = "싱글 모드";
            GameStart(solo_level);
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
                DetectorZeroButton(buttons, button.GetButtonXYPoint(), solo_level);
            }
            else
            {
                button.ButtonClick();
            }

            if (isGameClear(solo_level))
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
                    DetectorZeroButton(buttons, button.GetButtonXYPoint(), multi_level);
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

            SafeInvoke(lb_Turn, () => lb_Turn.Text = "상대 차례");
            Me_info.State = GAMESTATE.게임중;
            timer_PlayTime.Stop();
            
            if (button.isMineHave())
            {
                await MultiGameoverAsync();
                return;
            }
            else if ( button.isZeroCount())
            {
                DetectorZeroButton(buttons, button.GetButtonXYPoint(), multi_level);
            }
            else
            {
                button.ButtonClick();
            }

            if (isGameClear(multi_level))
            {
                await MultiGameClearAsync();
            }
            else
            {
                GAMERESULT result = JudgeGameResult(Me_info, Opp_info);
                if ( result != GAMERESULT.보류) SendGameResult(result);
                else
                { 
                    if (MessageBox.Show("승리하셨습니다. 이어서 마무리 하시겠습니까? (YES or No)", $"Player{mySocketNo+1}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SendGameResult(GAMERESULT.보류);
                    }
                    else
                    {
                        SendGameResult(GAMERESULT.승리);
                    }
                }
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
            if (MessageBox.Show("이전 게임을 다시 하시겠습니까?", $"Player{mySocketNo+1}", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ChooseLevel();
                GameReset();
                GameStart(solo_level);
            }
        }

        private void SoloGameOver()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                GameOver();
            }));
            MessageBox.Show("GAME OVER");
            ChooseLevel();
            GameReset();
            GameStart(solo_level);
        }

        private async Task MultiGameClearAsync()
        {
            Me_info.State = GAMESTATE.클리어;
            if (Me_info.Order == PLAYERORDER.선공)
            {
                SendGameCommand( mySocketNo, opponentNo, GameAction.Click, $"clear {timer}", "MultiGameClear");
                timer_PlayTime.Stop();
                MessageBox.Show("게임을 클리어 하셨습니다. 상대를 기다려주세요");
            }
            else
            {
                GAMERESULT result = JudgeGameResult(Me_info, Opp_info);
                SendGameResult(result);
            }

            GameClear();
        }

        private async Task MultiGameoverAsync()
        {
            Me_info.State = GAMESTATE.게임오버;
            if (Me_info.Order == PLAYERORDER.선공)
            {
                SendGameCommand(mySocketNo, opponentNo, GameAction.Click, $"gameover", "MultiGameover");
                timer_PlayTime.Stop();
                MessageBox.Show("지뢰를 밟으셨습니다. 상대를 기다려주세요");
            }
            else
            {
                GAMERESULT result = JudgeGameResult(Me_info, Opp_info);
                SendGameResult(result);
            }

            GameOver();
        }

        private void SendGameResult(GAMERESULT result)
        {
            if (result == GAMERESULT.진행중) { return; }
            // 게임 결과 (0: 무승부, 1: 승리, 2: 패배)
            switch (result)
            {
                case GAMERESULT.무승부:
                    SendGameCommand(mySocketNo, opponentNo, GameAction.Result, "draw", "SendGameResult"); 
                    break;
                case GAMERESULT.승리:
                    SendGameCommand(mySocketNo, opponentNo, GameAction.Result, "win", "SendGameResult"); 
                    break;
                case GAMERESULT.패배:
                    SendGameCommand(mySocketNo, opponentNo, GameAction.Result, "lose","SendGameResult");
                    break;
                case GAMERESULT.보류:
                    SendGameCommand(mySocketNo, opponentNo, GameAction.Result, "delay", "SendGameResult");
                    break;
            }
        }

        private void SendGameCommand(int mysock, int oppsock, GameAction action, string result, string func)
        {
            network.SendMessage(CommandType.Game, mysock, oppsock, action, result, func);
            MyTurnFlag = false;
        }
        GAMERESULT JudgeTimerResult()
        {
            if (opp_timer == -1) return GAMERESULT.진행중;         // 진행전
            else if (timer < opp_timer) return GAMERESULT.승리;  // 승리
            else if (timer > opp_timer) return GAMERESULT.패배;  // 패배
            return GAMERESULT.무승부;                               // 무승부

        }
        public GAMERESULT JudgeGameResult(User me, User opp)
        {
            GAMESTATE myGameState = me.State;
            GAMESTATE oppGameState = opp.State;

            PLAYERORDER myOrder = me.Order;

            // 1. 상대가 게임 중
            if (oppGameState == GAMESTATE.게임중)
            {
                if (myGameState == GAMESTATE.게임오버) return GAMERESULT.패배;
                else if (myGameState == GAMESTATE.클리어) return GAMERESULT.승리;
            }
            // 2. 상대가 게임 오버
            else if (oppGameState == GAMESTATE.게임오버)
            {
                if (myGameState == GAMESTATE.게임오버) return GAMERESULT.무승부;
                else if (myGameState == GAMESTATE.클리어) return GAMERESULT.승리;
                else
                {
                    // 내가 아직 게임 중인데 상대가 먼저 죽음 -> 보류
                    // 단, 선공이라면 내가 유리하므로 승리 처리할 수도 있음
                    return myOrder == PLAYERORDER.선공 ? GAMERESULT.승리 : GAMERESULT.보류;
                }
            }
            // 3. 상대가 클리어
            else if (oppGameState == GAMESTATE.클리어)
            {
                if (myGameState == GAMESTATE.클리어)
                {
                    GAMERESULT timeResult = JudgeTimerResult();  // 0: 무승부, 1: 승리, 2: 패배, -1: 진행전

                    if (timeResult == GAMERESULT.진행중)
                    {
                        // 시간 비교 실패한 경우, 선공 우선
                        return myOrder == PLAYERORDER.선공 ? GAMERESULT.승리 : GAMERESULT.패배;
                    }

                    return timeResult;
                }
                else
                {
                    // 나는 클리어 못했는데 상대는 클리어 => 패배
                    return GAMERESULT.패배;
                }
            }

            return GAMERESULT.진행중;
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
                    DetectorZeroButton(buttons, new Point(nx, ny), level);
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
                    if (SoloFlag ) solo_level = level;
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
            string data , action;
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
            
        









                /*

                if (command.Equals(COMMAND[0]))
                {
                    GameReset();
                    FormResize(multi_level);
                    InitGamePan(multi_level);
                    lb_Turn.Location = new Point(this.Size.Width / 2 - lb_Turn.Width / 2, menuStrip1.Height);
                    

                    List<Point> mine = MineLaying(data);

                    foreach (Point pt in mine)
                    {
                        buttons[pt.Y, pt.X].LayingMine();
                    }
                    NetWorkGameInit();
                    mySocketNo = recvNo;
                    opponentNo = sendNo;

                    MyTurnFlag = mySocketNo < opponentNo;

                    Me_info.Order = MyTurnFlag ? PLAYERORDER.선공 : PLAYERORDER.후공;
                    Opp_info.Order = MyTurnFlag ? PLAYERORDER.후공 : PLAYERORDER.선공;

                    ServerFlag = true;

                    toolStripTextBox_MyNum.Text = $"Player{mySocketNo+1}";
                    
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
                    GameStart(multi_level);
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
                                if (MessageBox.Show("상대가 게임을 나가려고 합니다.\n이전 게임을 다시 하시겠습니까? (YES or No)", $"Player{mySocketNo+1}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    network.SendMessage(COMMAND[2], mySocketNo, serverNo, "retry", "recv 보류 COMMAND[3] messagebox yes");
                                    lb_Turn.Text = "매칭 대기중";
                                }
                                else
                                {
                                    network.SendMessage(COMMAND[4], mySocketNo, opponentNo, "exit", "COMMAND[3] 오류");
                                    ChooseMatchingLevel();
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
                        lb_Turn.Text = "내 차례";
                        if (!timer_PlayTime.Enabled) timer_PlayTime.Start();
                    }
                }
                else if (command.Equals(COMMAND[5]))
                {
                    if (data.Equals("gameover"))
                    {
                        Opp_info.State = GAMESTATE.게임오버;
                        return;
                    }
                    else if (data.StartsWith("clear"))
                    {
                        Opp_info.State = GAMESTATE.클리어;
                        string[] dataList = data.Split(" ");
                        opp_timer = Convert.ToInt32(dataList[1]);
                    }
                    else if (data.Equals("retire"))
                    {
                        GameOver();
                        MessageBox.Show("게임을 강제종료했습니다.\n다음 게임을 준비하세요");
                        ChooseMatchingLevel();
                    }
                    else
                    {
                        if (data.Equals("solo"))
                        {
                            MessageBox.Show("싱글 모드로 전환됩니다\n멀티로 다시 플레이 하시려면 네트워크 게임에 다시 접속해주세요", $"Player{mySocketNo + 1}");
                            ChangeSoloMode();
                            return;
                        }
                        else if (data.Equals("multi"))
                        {
                            MessageBox.Show("상대가 혼자 마무리하고 싶어합니다.\n상대방을 찾기 위해 매칭 상태로 돌아갑니다", $"Player{mySocketNo + 1}");
                            network.SendMessage(COMMAND[2], mySocketNo, serverNo, "retry", "recv 보류 multi");
                            SafeInvoke(lb_Turn, () => lb_Turn.Text = "매칭 대기중"); 
                            return;
                        }

                        string result = "";
                        GameOver();
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

                        
                        if (MessageBox.Show($"{result}\n이전 게임을 다시 하시겠습니까? Yes 또는 다른 게임을 선택하려면 No를 선택하세요", $"Player{mySocketNo + 1}", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            network.SendMessage(COMMAND[2], mySocketNo, mySocketNo, "retry", "recv 보류 messagebox yes");
                        }
                        else
                        {
                            network.SendMessage(COMMAND[3], mySocketNo, opponentNo, "", "recv 보류 messagebox no");
                            ChooseMatchingLevel();
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
                                if (opponentNo != -1 )
                                    MessageBox.Show("이미 게임에 참여하고 있습니다.");
                                break;
                            case "opponent_left":
                                MessageBox.Show("상대가 게임을 나가려고 합니다.");
                                GameOver(); // 오류 발생 시 게임 종료 후 처리
                                break;
                            case "보류":
                                server_off();
                                MessageBox.Show("게임을 강제종료했습니다.\n다음 게임을 준비하세요", "오류");
                                break;
                            default:
                                MessageBox.Show("다른 오류가 발생했습니다.");
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
                            network.SendMessage(CommandType.System, mySocketNo, serverNo, SystemAcion.Error, "오류발생", "recv 보류 오류 else");
                    }
                }
                */
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
            DetectorMine(multi_level);
            gameoverFlag = false;
            SoloFlag = false;
            networkPlayFlag = true;
            MineFlagCnt = MINE_FLAG_COUNT[multi_level];
            toolStripTextBox_MineFlag.Text = MineFlagCnt.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();

            Me_info = new();
            Opp_info = new();
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

            if (mySocketNo != -1 && opponentNo != -1)
                SendGameCommand( mySocketNo, opponentNo, GameAction.Result, "lose", "보류");

            if (network.IsReallyConnected())  // network.Socket 확인 후 Socket 객체 해제
            {
                ChooseMatchingLevel();
            }
            else
            {
                network.Dispose();
                MessageBox.Show("게임을 강제종료하려면 다음 게임을 준비하세요");
            }
        }

        void server_off()
        {
            network.Dispose();
            network = null;
            lb_Turn.Text = "싱글 모드";


            GameOver();
            GameReset();
            GameStart(solo_level);
        }

        private void 시작하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkConnection();
            if (network == null || !network.IsReallyConnected())
            {
                if (MessageBox.Show("게임을 강제종료하려면 다음 게임을 준비하세요. 이전 게임을 하시겠습니까", "오류", MessageBoxButtons.OK) == DialogResult.OK)
                    return;
            }

            ChooseMatchingLevel();
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
            ChooseLevel();
            if ( SoloFlag ) multi_level = solo_level;
            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level ,multi_level.ToString(), "난이도 선택");
            GameReset();
            GameStart(multi_level);
            lb_Turn.Text = "싱글 모드";
        }

        private async void 지뢰찾기_MouseClick(object sender, MouseEventArgs e)
        {
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

                if (result == 1) GameOver();
                else if (result == 2) GameClear();

                GameReset();
                GameStart(solo_level);
            }
            else
            {
                int result = OpenSurroundingButtons(button, multi_level);
                if (result == 0) return;

                SafeInvoke(lb_Turn, () => lb_Turn.Text = "상대 차례");

                if (result == 1)
                {
                    await MultiGameoverAsync();
                }
                else if (result == 2)
                {
                    await MultiGameClearAsync();
                }
            }
        }
        private async void Button_Clicked_Event(object? sender, EventArgs e)
        {
            if (superClickFlag) return;
            superClickFlag = true;

            Mine button = (Mine)sender;

            if (!button.isEnabled())
            {
                superClickFlag = false;
                return;
            }

            
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


            await Task.Delay(100);
            superClickFlag = false;
        }

        void ChangeSoloMode()
        {
            if (!SoloFlag)
            {
                // 먼저 네트워크 해제
                if (network != null)
                {
                    network.Disconnect();
                    network = null;
                }

                // 그 다음 모드 전환
                SoloFlag = true;
                solo_level = multi_level;
                SafeInvoke(lb_Turn, () => lb_Turn.Text = "싱글 모드");
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
            // ServerAction { Click, Mine };

        }
        void HandleClientCommand(int sendNo, int recvNo, string action, string data)
        {
            // ClientAction { Retry, Level, LeaveRoom, Exit };
        }
        void HandleGameCommand(int sendNo, int recvNo, string action, string data)
        {
            // GameAction { Click, Result }
            if (action == "Click")
                HandleClickCommand(sendNo, recvNo, data);
            else if (action == "Result")
            {; }
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
            }
        }

        void HandleConnectAction(string data)
        {
            int no = Convert.ToInt32(data);
            mySocketNo = no;
        }
        void HandleClickCommand(int sendNo, int recvNo, string data)
        {

        }


    }
}

