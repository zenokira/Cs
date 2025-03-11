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
            환경설정ToolStripMenuItem = new ToolStripMenuItem();
            toolStripTextBox_Timer = new ToolStripTextBox();
            toolStripTextBox_MineFlag = new ToolStripTextBox();
            asdToolStripMenuItem = new ToolStripMenuItem();
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
            menuStrip1.Items.AddRange(new ToolStripItem[] { 메뉴ToolStripMenuItem, toolStripTextBox_Timer, toolStripTextBox_MineFlag });
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
            새로시작ToolStripMenuItem.Size = new Size(180, 22);
            새로시작ToolStripMenuItem.Text = "새로 시작";
            새로시작ToolStripMenuItem.Click += 새로시작ToolStripMenuItem_Click;
            // 
            // 환경설정ToolStripMenuItem
            // 
            환경설정ToolStripMenuItem.Name = "환경설정ToolStripMenuItem";
            환경설정ToolStripMenuItem.Size = new Size(180, 22);
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
            // asdToolStripMenuItem
            // 
            asdToolStripMenuItem.Name = "asdToolStripMenuItem";
            asdToolStripMenuItem.Size = new Size(180, 22);
            asdToolStripMenuItem.Text = "네트워크 게임";
            asdToolStripMenuItem.Click += asdToolStripMenuItem_Click;
            // 
            // 지뢰찾기
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 476);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "지뢰찾기";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "지뢰찾기";
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
    }
}
