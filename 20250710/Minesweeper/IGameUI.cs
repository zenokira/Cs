using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace Minesweeper
{
    public interface IGameUI
    {
        int MySocketNo { get; set; }

        int ServerNo { get; set; }
        int OpponentNo { get; set; }
        bool MyTurnFlagProp { get; set; }
        bool ServerFlagProp { get; set; }
        bool SoloFlagProp { get; set; }

        Size FormSize { get; }  // 이름 명확히: 폼의 사이즈임을 강조
        GameManager GameManagerInstance { get; }
        Network NetworkInstance { get; set; }

        Alert AlertInstance { get; set; }

        Timer Timer_PlayTime { get; }

        ToolStripTextBox ToolStripTextBox_MineFlag { get; }
        Label Lb_Turn { get; }
        MenuStrip MenuStrip1 { get; }

        Mine[,] GetButtons();
        void SetButtons(Mine[,] buttons);

        // 필요한 메서드들
        void CallUpdateState();
        void CallGameOver();
        void CallButtonReset();
        void CallMakeGamePanButton(int level);
        void CallSoloGameInit();
        void CallNetWorkGameInit();
        void CallChooseLevel();
        List<Point> CallMineLaying(string data);
    }
}
