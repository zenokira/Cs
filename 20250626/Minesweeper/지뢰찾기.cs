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

namespace Minesweeper
{
    public enum GAMESTATE { ������, ������, ���ӿ���, Ŭ����, ���� };
    public enum PLAYERORDER { ����, �İ� }

    public enum CommandType { Server, Client, Game, System };
    /*
    client: Ŭ���̾�Ʈ �� ��ȣ�ۿ� (level ����, retry ��û ��)
    game: ���� �� ����(click, result ) [ click - alive, clear, dead   result - win, lose, draw ]
    server: ���� �޽��� ( recv �� �����ϰ� ���� �̰ɷ� send �ϱ� ���� )
    system: ����/���� ���� ��
    */
    public enum ServerAction { OpponentInfo, TurnInfo, Matching }
    public enum ClientAction { Retry, Level, LeaveRoom, Exit };
    public enum GameAction { Click, Result, FirstClick, MineMap, ReGame }
    public enum SystemAction { Connect, Exit, Error, Disconnect, CloseRoom, Wait };
    public partial class ����ã�� : Form
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


        readonly string START_WORD = "<SOW>";
        readonly string END_WORD = "<EOW>";

        Mine[,] buttons;

        public ����ã��()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            SetMystate();
            SoloGameInit();
            SetToolStripMenuVisible(false);
        }

        private void Initialize()
        {
            gameManager = new();
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

        private Task AsyncAction(Action action)
        {
            return Task.Run(action);
        }

        private Task SafeAsyncInvoke(Action action)
        {
            return Task.Run(() =>
            {
                SafeInvoke(action);
            });
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
                toolStripTextBox_MineFlag.Text = (gameManager.GetMineFlagCnt(false) - cnt).ToString();
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
            size.Height += gameManager.ButtonStartY + gameManager.ButtonSize * cols + �޴�ToolStripMenuItem.Height;

            this.Size = size;
        }
        void SoloGameInit()
        {
            int level = gameManager.SOLO_LEVEL;
            FormResize(level);
            MakeGamePanButton(level);

            gameManager.ClearButtons();
            gameManager.SetButtons(buttons);
            gameManager.SoloGameInit();

            SoloFlag = true;
            gameManager.MINE_FLAG_CNT = gameManager.MineCount[level];
            toolStripTextBox_MineFlag.Text = gameManager.MINE_FLAG_CNT.ToString();
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }

        void MultiGameInit()
        {
            int level = gameManager.MULTI_LEVEL;
            FormResize(level);
            MakeGamePanButton(level);

            gameManager.SetButtons(buttons);
            gameManager.MINE_FLAG_CNT = gameManager.MineCount[level];
            toolStripTextBox_MineFlag.Text = gameManager.MINE_FLAG_CNT.ToString();
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();
        }







        void OpenZeroButton(Point clckpt, bool isSolo)
        {

            List<Point> toOpen = gameManager.GetButtonsToOpenByZeroDetector(clckpt, isSolo);
            foreach (Point pt in toOpen)
            {
                buttons[pt.Y, pt.X].ButtonClick(); // �̰� UI�� å��
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
            gameManager.ClearButtons();
            gameManager.SetButtons(buttons);
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
                    return GAMESTATE.���ӿ���;
                }

                if (btn.isZeroCount())
                    OpenZeroButton(pt, isSolo);
                else if (btn.isEnabled())
                    btn.ButtonClick();
            }

            if (gameManager.IsGameClear(true))
            {
                return GAMESTATE.Ŭ����;
            }

            return GAMESTATE.������;
        }

        private void GameClear()
        {
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
                button.Text = "��";
                button.BackColor = Color.Red;
                button.Enabled = false;
            }

            if (timer_PlayTime.Enabled)
            {
                timer_PlayTime.Stop();
                timer = 0;
            }
        }

        private void ButtonReset()
        {
            timer_PlayTime.Stop();
            timer = 0;

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
        private void ���ν���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServerFlag) return;
            ButtonReset();
            MakeGamePanButton(gameManager.SOLO_LEVEL);
            SoloGameInit();
        }

        private void ȯ�漳��ToolStripMenuItem_Click(object sender, EventArgs e)
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
                    if (ServerFlag)
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

            toolStripTextBox_MineFlag.Text = gameManager.MINE_FLAG_CNT.ToString();
            timer = 0;
            toolStripTextBox_Timer.Text = "0";
            timer_PlayTime.Start();

            toolStripTextBox_MyNum.Text = $"Player{mySocketNo + 1}";

            SetMystate();


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

            // action ��ȯ
            action = s[3];

            // data ó��
            if (s.Length > 5)
                data = ConcatData(s, 4);  // s[4]���� ������ �̾���̱�
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


        private void ����ã��_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (network == null) return;

            if (network.IsSocketNULL()) return;

            if (network.IsConnected())
            {
                network.SendMessage(CommandType.System, mySocketNo, opponentNo, SystemAction.Exit, "exit", "����");
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
                    MessageBox.Show("��Ʈ��ũ ���ῡ �����߽��ϴ�");
                }
                else
                {
                    //MessageBox.Show("���� �����߽��ϴ�.");
                }
            }
            else if (network.IsConnected()) return;
        }

        private void ����ϱ�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (network == null) return;
            if (!network.IsConnected()) return;
            if (opponentNo == -1) return;

            if (MessageBox.Show("��� �� ��Ʈ��ũ ������ ����˴ϴ�. �̱۸��� ���ư��ðڽ��ϱ�?", "�˸�", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Exit, "��������", "���");
                ServerFlag = false;
                SoloFlag = true;
                SetMystate();
            }
        }

        private void �����ϱ�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkConnection();
            if (network == null || !network.IsReallyConnected())
            {
                if (MessageBox.Show("������ ���������Ϸ��� ���� ������ �غ��ϼ���. ���� ������ �Ͻðڽ��ϱ�", "����", MessageBoxButtons.OK) == DialogResult.OK)
                    return;
            }
            ServerFlag = true;

            if (ServerFlag)
            {
                ChooseAndSendLevel();
                ButtonReset();
                MakeGamePanButton(gameManager.MULTI_LEVEL);
                SoloGameInit();
                SetToolStripMenuVisible(true);
                SetMystate();
            }
        }

        void ChooseAndSendLevel()
        {
            ChooseLevel();
            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level, gameManager.MULTI_LEVEL.ToString(), "Choose & Send");
        }

        void SetMystate()
        {
            string text = "";
            if (ServerFlag)
            {
                if (SoloFlag) text = "��Ī �����";
                else
                {
                    if (MyTurnFlag) text = "�� ����";
                    else text = "��� ����";
                }
            }
            else
            {
                text = "�̱� ���";
            }

            SafeInvoke(() => lb_Turn.Text = text);
        }

        void SetToolStripMenuVisible(bool visible)
        {
            �泪����ToolStripMenuItem.Visible = visible;
            ����ϱ�ToolStripMenuItem.Visible = visible;
        }
        public void LogPacket(string message)
        {
            try
            {
                // �α� ���� ���� �� �α� �ۼ�
                using (StreamWriter writer = new StreamWriter($"network_log{mySocketNo}.txt", true))
                {
                    // �α� �ۼ� �ð� �߰�
                    writer.WriteLine($"{message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"�α� �ۼ� ����: {ex.Message}");
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

        private async void ����ã��_MouseClick(object sender, MouseEventArgs e)
        {
            if (buttons == null) return;
            foreach (Mine btn in buttons)
            {
                if (btn.Bounds.Contains(e.Location))
                {
                    if (btn.Enabled)
                    {
                        return; // Button_Clicked_Event ȣ�� �� ����
                    }
                    break;
                }
            }



            Mine? button = Find_Button_Coordinates(e.Location);

            if (button == null) return;

            if (SoloFlag)
            {
                GAMESTATE gamestate = OpenSurroundingButtons(button, true);


                switch (gamestate)
                {
                    case GAMESTATE.���ӿ���:
                        GameOver();
                        MessageBox.Show("���ڸ� �����̽��ϴ�", "�˸�", MessageBoxButtons.OK);
                        break;
                    case GAMESTATE.Ŭ����:
                        GameClear();
                        MessageBox.Show("Ŭ���� �ϼ̽��ϴ�", "�˸�", MessageBoxButtons.OK);
                        break;
                    case GAMESTATE.������:
                        return;
                }

                ChooseLevel();
                ButtonReset();
                MakeGamePanButton(gameManager.SOLO_LEVEL);
                SoloGameInit();
            }
            else
            {

                if (!MyTurnFlag) return;
                Console.WriteLine("[MouseClick] MyTurnFlag = false");
                GAMESTATE gamestate = OpenSurroundingButtons(button, true);

                switch (gamestate)
                {
                    case GAMESTATE.���ӿ���:
                        network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "�ڵ�����");
                        GameOver();
                        break;
                    case GAMESTATE.Ŭ����:
                        network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer}", "�ڵ�����");
                        GameClear();
                        break;
                    case GAMESTATE.������:
                        return;
                }

                if (timer_PlayTime.Enabled) timer_PlayTime.Stop();
                MyTurnFlag = false;
                SetMystate();
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
            GAMESTATE gamestate = GAMESTATE.������;
            if (mine.isMineHave())
            {
                GameOver();
                MessageBox.Show("���ڸ� �����̽��ϴ�", "�˸�", MessageBoxButtons.OK);
                gamestate = GAMESTATE.���ӿ���;
            }
            else if (mine.isZeroCount())
            {
                OpenZeroButton(mine.GetButtonXYPoint(), true);
            }
            else
            {
                mine.ButtonClick();
            }

            if (gameManager.IsGameClear(true))
            {
                GameClear();
                MessageBox.Show("Ŭ���� �ϼ̽��ϴ�", "�˸�", MessageBoxButtons.OK);
                gamestate = GAMESTATE.Ŭ����;
            }

            if (gamestate == GAMESTATE.���ӿ��� || gamestate == GAMESTATE.Ŭ����)
            {
                ChooseLevel();
                ButtonReset();
                SoloGameInit();
            }
        }

        private async void �泪����ToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            if (network == null || network.IsSocketNULL()) return;
            if (MessageBox.Show("���� �濡�� ���� �������� ���ðڽ��ϱ�?\nYes or No", "�泪����", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                opponentNo = -1;
                network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.LeaveRoom, "Leave", "�泪����");
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
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, "Dead", "��ư Ŭ�� ��Ƽ");
                GameOver();
                Application.DoEvents();
                return;
            }
            else if (mine.isZeroCount())
            {
                OpenZeroButton(mine.GetButtonXYPoint(), true);
            }
            else
            {
                mine.ButtonClick();
            }

            if (gameManager.IsGameClear(true))
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Result, $"Clear {timer}", "��ư Ŭ�� ��Ƽ");
            }
            else
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.Click, "Alive", "��ư Ŭ�� ��Ƽ");
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
        /// CommantType Handler ����
        /// </summary>
        /// <param name="sendNo"></param>
        /// <param name="recvNo"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        void HandleServerCommand(int sendNo, int recvNo, string action, string data)
        {
            // ServerAction { OpponentInfo, TurnInfo };

            switch (action)
            {
                case "OpponentInfo":
                    HandleOpponentInfoAction(sendNo);
                    break;
                case "TurnInfo":
                    HandleTurnInfoAction(data);
                    break;
                case "Matching":
                    HandleMatchingAction();
                    break;
            }
        }

        private void HandleMatchingAction()
        {
            ButtonReset();
            MakeGamePanButton(gameManager.MULTI_LEVEL);
            SoloFlag = false;
            ServerFlag = true;
            
        }
        private void HandleTurnInfoAction(string data)
        {
            int turnSock = Convert.ToInt32(data);

            MyTurnFlag = turnSock == mySocketNo;
            lb_Turn.Location = new Point(this.Size.Width / 2 - lb_Turn.Width / 2, menuStrip1.Height);
        }

        private void HandleOpponentInfoAction(int no)
        {
            opponentNo = no;
        }

        void HandleClientCommand(int sendNo, int recvNo, string action, string data)
        {
            // ClientAction { Retry, Level, LeaveRoom, Exit };

            switch (action)
            {
                case "Exit":
                    HandleExitCommand();
                    break;

            }
        }
        void HandleExitCommand()
        {
            network.Dispose();
            ButtonReset();
            MakeGamePanButton(gameManager.SOLO_LEVEL);
            SoloGameInit();
            ServerFlag = false;
            SetMystate();
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
                    //case "FirstClick":
                    //HandleFirstClickCommand(data);
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
            ButtonReset();
            MakeGamePanButton(gameManager.MULTI_LEVEL);
            SoloFlag = false;
            ServerFlag = true;
            toolStripTextBox_MineFlag.Text = gameManager.MineFlagCount.ToString();
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

            GameOver();
            ButtonReset();
            ChooseLevel();
            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level, gameManager.MULTI_LEVEL.ToString(), "Wait");
            SoloGameInit();
            SetMystate();
        }

        void HandleCloseRoomAction()
        {
            opponentNo = -1;
            SoloFlag = true;
            GameOver();
            ButtonReset();
            ChooseLevel();
            network.SendMessage(CommandType.Client, mySocketNo, serverNo, ClientAction.Level, gameManager.MULTI_LEVEL.ToString(), "CloseRoom");
            SoloGameInit();
            SetMystate(); 
        }

        void HandleConnectAction(string data)
        {
            if (mySocketNo != -1) return;

            int no = Convert.ToInt32(data);
            mySocketNo = no;
        }
        void HandleClickCommand()
        {
            Console.WriteLine("Ȥ�� ��ư Ŭ���ߴµ� �޴°ǰ�?");
            MyTurnFlag = true;
            SetMystate();
            if (!timer_PlayTime.Enabled) timer_PlayTime.Start();
        }

        void HandleResultCommand(int sendNo, int recvNo, string data)
        {
            string result = data switch
            {
                "Win" => "�¸��ϼ̽��ϴ�.",
                "Lose" => "�й��ϼ̽��ϴ�.",
                "Draw" => "���º��Դϴ�.",
                _ => "�� �� ���� ����Դϴ�."
            };

            if (MessageBox.Show($"{result}\n�ٽ� �Ͻ÷��� Yes ���Ƿ� ���ư��÷��� No �� �����ּ���", $"Player{mySocketNo + 1} ���", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                network.SendMessage(CommandType.Game, mySocketNo, opponentNo, GameAction.ReGame, "", "���");
            }
            else
            {
                network.SendMessage(CommandType.Client, mySocketNo, opponentNo, ClientAction.LeaveRoom, "", "���");
            }
        }

        void HandleMineMapCommand(string data)
        {
            List<Point> mine = MineLaying(data);

            foreach (Point pt in mine)
            {
                buttons[pt.Y, pt.X].LayingMine();
            }

            gameManager.MultiGameInit();
            NetWorkGameInit();
        }

        // ���� �ڵ� Ŭ�� 1��
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
    }
}