using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    // gameHandler.cs

    public class GameCommandHandler
    {
        private readonly IGameUI IGame;
        public GameCommandHandler(IGameUI context) => IGame = context;

        public void HandleGameCommand(int sendNo, int recvNo, string action, string data)
        {
            switch (action)
            {
                case "Click":
                    HandleClick();
                    break;
                case "Result":
                    HandleResult(sendNo, recvNo, data);
                    break;
                case "MineMap":
                    HandleMineMap(data);
                    break;
                case "ReGame":
                    HandleReGame();
                    break;
                    // case "FirstClick": // 향후 구현 필요시
            }
        }

        public void HandleClick()
        {
            Console.WriteLine("혹시 버튼 클릭했는데 받는건가?");
            IGame.MyTurnFlagProp = true;
            IGame.CallUpdateState();
            if (!IGame.Timer_PlayTime.Enabled)
                IGame.Timer_PlayTime.Start();
        }

        public void HandleResult(int sendNo, int recvNo, string data)
        {
            string result = data switch
            {
                "Win" => "승리하셨습니다.",
                "Lose" => "패배하셨습니다.",
                "Draw" => "무승부입니다.",
                _ => "알 수 없는 결과입니다."
            };

            IGame.AlertInstance = new Alert($"{result}\n다시 하시려면 Yes, 대기실로 돌아가시려면 No", $"Player{IGame.MySocketNo + 1} 결과");
            IGame.AlertInstance.ShowDialog();

            if (IGame.AlertInstance == null) return;

            if (IGame.AlertInstance.DialogResult == DialogResult.Yes)
            {
                IGame.NetworkInstance.SendMessage(CommandType.Game, IGame.MySocketNo, IGame.OpponentNo, GameAction.ReGame, "", "결과");
            }
            else if (IGame.AlertInstance.DialogResult == DialogResult.No)
            {
                IGame.NetworkInstance.SendMessage(CommandType.Client, IGame.MySocketNo, IGame.OpponentNo, ClientAction.LeaveRoom, "", "결과");
            }
    
        }

        public void HandleMineMap(string data)
        {
            List<Point> mine = IGame.CallMineLaying(data);
            foreach (Point pt in mine)
            {
                IGame.GetButtons()[pt.Y, pt.X].LayingMine();
            }

            IGame.GameManagerInstance.MultiGameInit(IGame.GetButtons());
            IGame.CallNetWorkGameInit();
        }

        public void HandleReGame()
        {
            IGame.CallButtonReset();
            IGame.CallMakeGamePanButton(IGame.GameManagerInstance.MULTI_LEVEL);
            IGame.SoloFlagProp = false;
            IGame.ServerFlagProp = true;
            IGame.ToolStripTextBox_MineFlag.Text = IGame.GameManagerInstance.MineFlagCount.ToString();
        }
    }

    public class SystemCommandHandler
    {
        private readonly IGameUI IGame;
        public SystemCommandHandler(IGameUI context) => IGame = context;
        public void HandleSystemCommand(int sendNo, int recvNo, string action, string data)
        {
            switch (action)
            {
                case "Connect":
                    HandleConnect(data);
                    break;
                case "Disconnect":
                    HandleDisconnect(); 
                    break;
                case "CloseRoom":
                    HandleCloseRoom();
                    break;
                case "Wait":
                    HandleWait(data);
                    break;
                    // 필요시 Exit, Error 등 추가
            }
        }

        public void HandleConnect(string data)
        {
            if (IGame.MySocketNo != -1) return;
            IGame.MySocketNo = Convert.ToInt32(data);
        }

        public void HandleDisconnect()
        {
            Alert alert = new Alert("서버가 종료되어 싱글모드로 돌아갑니다.\n빠르게 재오픈 할 예정입니다");
            alert.Show();

            IGame.ServerFlagProp = false;
            IGame.SoloFlagProp = true;
            IGame.NetworkInstance.Disconnect();
            IGame.NetworkInstance = null;
            IGame.CallUpdateState();


        }

        public void HandleCloseRoom()
        {
            IGame.OpponentNo = -1;
            IGame.SoloFlagProp = true;
            IGame.CallGameOver();
            IGame.CallButtonReset();
            IGame.CallChooseLevel();
            IGame.NetworkInstance.SendMessage(CommandType.Client, IGame.MySocketNo, IGame.ServerNo, ClientAction.Level, IGame.GameManagerInstance.MULTI_LEVEL.ToString(), "CloseRoom");
            IGame.CallSoloGameInit();
            IGame.CallUpdateState();
        }

        public void HandleWait(string data)
        {
            if (data == "OppExit" )
            {
                if (IGame.AlertInstance != null)
                {
                    IGame.AlertInstance?.ForceClose();
                    IGame.AlertInstance = null;
                }
                Alert alert = new Alert("상대방이 나갔습니다");
                alert.ShowDialog();
            }

            IGame.CallGameOver();
            IGame.CallButtonReset();
            IGame.CallChooseLevel();
            IGame.SoloFlagProp = true;
            IGame.NetworkInstance.SendMessage(CommandType.Client, IGame.MySocketNo, IGame.ServerNo, ClientAction.Level, IGame.GameManagerInstance.MULTI_LEVEL.ToString(), "Wait");
            IGame.CallSoloGameInit();
            IGame.CallUpdateState();
        }
    }

    public class ClientCommandHandler
    {
        private readonly IGameUI IGame;
        public ClientCommandHandler(IGameUI context) => IGame = context;
        public void HandleClientCommand(int sendNo, int recvNo, string action, string data)
        {
            switch (action)
            {
                case "Exit":
                    HandleExit();
                    break;
                    // 필요시 다른 ClientAction 도 여기에
            }
        }
        public void HandleExit()
        {
            IGame.NetworkInstance.Dispose();
            IGame.ServerFlagProp = false;
            IGame.CallButtonReset();
            IGame.CallSoloGameInit();
            IGame.CallUpdateState();
        }
    }

    public class ServerCommandHandler
    {
        private readonly IGameUI IGame;
        public ServerCommandHandler(IGameUI context) => IGame = context;
        public void HandleServerCommand(int sendNo, int recvNo, string action, string data)
        {
            switch (action)
            {
                case "OpponentInfo":
                    HandleOpponentInfo(sendNo);
                    break;
                case "TurnInfo":
                    HandleTurnInfo(data);
                    break;
                case "Matching":
                    HandleMatching();
                    break;
                    // 필요시 다른 action 추가
            }
        }
        public void HandleOpponentInfo(int no)
        {
            IGame.OpponentNo = no;
        }

        public void HandleTurnInfo(string data)
        {
            int turnSock = Convert.ToInt32(data);
            IGame.MyTurnFlagProp = turnSock == IGame.MySocketNo;
            IGame.Lb_Turn.Location = new Point(IGame.FormSize.Width / 2 - IGame.Lb_Turn.Width / 2, IGame.MenuStrip1.Height);
        }

        public void HandleMatching()
        {
            GameManager gameManager = IGame.GameManagerInstance;
            IGame.CallButtonReset();
            IGame.CallFormResize(gameManager.MULTI_LEVEL);
            IGame.CallMakeGamePanButton(gameManager.MULTI_LEVEL);

            IGame.SoloFlagProp = false;
            IGame.ServerFlagProp = true;
            IGame.GameInProgress = true;
        }
    }

}