namespace Minesweeper
{
    partial class 지뢰찾기
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timer_PlayTime = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            메뉴ToolStripMenuItem = new ToolStripMenuItem();
            새로시작ToolStripMenuItem = new ToolStripMenuItem();
            asdToolStripMenuItem = new ToolStripMenuItem();
            접속하기ToolStripMenuItem = new ToolStripMenuItem();
            시작하기ToolStripMenuItem = new ToolStripMenuItem();
            기권하기ToolStripMenuItem = new ToolStripMenuItem();
            환경설정ToolStripMenuItem = new ToolStripMenuItem();
            toolStripTextBox_Timer = new ToolStripTextBox();
            toolStripTextBox_MineFlag = new ToolStripTextBox();
            toolStripTextBox_MyNum = new ToolStripTextBox();
            toolStripTextBox_OPNum = new ToolStripTextBox();
            lb_Turn = new Label();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // timer_PlayTime
            // 
            timer_PlayTime.Interval = 1000;
            timer_PlayTime.Tick += timer_PlayTime_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 메뉴ToolStripMenuItem, toolStripTextBox_Timer, toolStripTextBox_MineFlag, toolStripTextBox_MyNum, toolStripTextBox_OPNum });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(509, 34);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            메뉴ToolStripMenuItem.AutoSize = false;
            메뉴ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 새로시작ToolStripMenuItem, asdToolStripMenuItem, 환경설정ToolStripMenuItem });
            메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            메뉴ToolStripMenuItem.Size = new Size(40, 30);
            메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 새로시작ToolStripMenuItem
            // 
            새로시작ToolStripMenuItem.Name = "새로시작ToolStripMenuItem";
            새로시작ToolStripMenuItem.Size = new Size(150, 22);
            새로시작ToolStripMenuItem.Text = "새로 시작";
            새로시작ToolStripMenuItem.Click += 새로시작ToolStripMenuItem_Click;
            // 
            // asdToolStripMenuItem
            // 
            asdToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 접속하기ToolStripMenuItem, 시작하기ToolStripMenuItem, 기권하기ToolStripMenuItem });
            asdToolStripMenuItem.Name = "asdToolStripMenuItem";
            asdToolStripMenuItem.Size = new Size(150, 22);
            asdToolStripMenuItem.Text = "네트워크 게임";
            // 
            // 접속하기ToolStripMenuItem
            // 
            접속하기ToolStripMenuItem.Name = "접속하기ToolStripMenuItem";
            접속하기ToolStripMenuItem.Size = new Size(122, 22);
            접속하기ToolStripMenuItem.Text = "접속하기";
            접속하기ToolStripMenuItem.Click += 접속하기ToolStripMenuItem_Click;
            // 
            // 시작하기ToolStripMenuItem
            // 
            시작하기ToolStripMenuItem.Name = "시작하기ToolStripMenuItem";
            시작하기ToolStripMenuItem.Size = new Size(122, 22);
            시작하기ToolStripMenuItem.Text = "시작하기";
            시작하기ToolStripMenuItem.Click += 시작하기ToolStripMenuItem_Click;
            // 
            // 기권하기ToolStripMenuItem
            // 
            기권하기ToolStripMenuItem.Name = "기권하기ToolStripMenuItem";
            기권하기ToolStripMenuItem.Size = new Size(122, 22);
            기권하기ToolStripMenuItem.Text = "기권하기";
            기권하기ToolStripMenuItem.Click += 기권하기ToolStripMenuItem_Click;
            // 
            // 환경설정ToolStripMenuItem
            // 
            환경설정ToolStripMenuItem.Name = "환경설정ToolStripMenuItem";
            환경설정ToolStripMenuItem.Size = new Size(150, 22);
            환경설정ToolStripMenuItem.Text = "환경설정";
            환경설정ToolStripMenuItem.Click += 환경설정ToolStripMenuItem_Click;
            // 
            // toolStripTextBox_Timer
            // 
            toolStripTextBox_Timer.Alignment = ToolStripItemAlignment.Right;
            toolStripTextBox_Timer.AutoSize = false;
            toolStripTextBox_Timer.BackColor = Color.OrangeRed;
            toolStripTextBox_Timer.ForeColor = Color.White;
            toolStripTextBox_Timer.Name = "toolStripTextBox_Timer";
            toolStripTextBox_Timer.ReadOnly = true;
            toolStripTextBox_Timer.Size = new Size(50, 30);
            toolStripTextBox_Timer.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // toolStripTextBox_MineFlag
            // 
            toolStripTextBox_MineFlag.Alignment = ToolStripItemAlignment.Right;
            toolStripTextBox_MineFlag.AutoSize = false;
            toolStripTextBox_MineFlag.BackColor = Color.Black;
            toolStripTextBox_MineFlag.ForeColor = Color.White;
            toolStripTextBox_MineFlag.Name = "toolStripTextBox_MineFlag";
            toolStripTextBox_MineFlag.ReadOnly = true;
            toolStripTextBox_MineFlag.Size = new Size(50, 30);
            toolStripTextBox_MineFlag.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // toolStripTextBox_MyNum
            // 
            toolStripTextBox_MyNum.BackColor = Color.DodgerBlue;
            toolStripTextBox_MyNum.Name = "toolStripTextBox_MyNum";
            toolStripTextBox_MyNum.ReadOnly = true;
            toolStripTextBox_MyNum.Size = new Size(50, 30);
            // 
            // toolStripTextBox_OPNum
            // 
            toolStripTextBox_OPNum.BackColor = Color.DodgerBlue;
            toolStripTextBox_OPNum.Name = "toolStripTextBox_OPNum";
            toolStripTextBox_OPNum.ReadOnly = true;
            toolStripTextBox_OPNum.Size = new Size(50, 30);
            // 
            // lb_Turn
            // 
            lb_Turn.Location = new Point(202, 34);
            lb_Turn.Name = "lb_Turn";
            lb_Turn.Size = new Size(90, 25);
            lb_Turn.TabIndex = 1;
            lb_Turn.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // 지뢰찾기
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 476);
            Controls.Add(lb_Turn);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "지뢰찾기";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "지뢰찾기";
            FormClosing += 지뢰찾기_FormClosing;
            Load += Form1_Load;
            MouseClick += 지뢰찾기_MouseClick;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer_PlayTime;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 메뉴ToolStripMenuItem;
        private ToolStripMenuItem 새로시작ToolStripMenuItem;
        private ToolStripMenuItem 환경설정ToolStripMenuItem;
        private ToolStripTextBox toolStripTextBox_MineFlag;
        private ToolStripTextBox toolStripTextBox_Timer;
        private ToolStripMenuItem asdToolStripMenuItem;
        private ToolStripMenuItem 접속하기ToolStripMenuItem;
        private ToolStripMenuItem 기권하기ToolStripMenuItem;
        private ToolStripTextBox toolStripTextBox_MyNum;
        private ToolStripTextBox toolStripTextBox_OPNum;
        private Label lb_Turn;
        private ToolStripMenuItem 시작하기ToolStripMenuItem;
    }
}
