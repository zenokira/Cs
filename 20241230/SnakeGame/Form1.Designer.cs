namespace SnakeGame
{
    partial class Form1
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
            timer_Game = new System.Windows.Forms.Timer(components);
            strip_Status = new StatusStrip();
            toolStripStatusLabel_LV = new ToolStripStatusLabel();
            toolStripStatusLabel_Jumsu = new ToolStripStatusLabel();
            toolStripStatusLabel_Bodycnt = new ToolStripStatusLabel();
            timer_Countdown = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            메뉴MToolStripMenuItem = new ToolStripMenuItem();
            시작ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            다시시작ToolStripMenuItem = new ToolStripMenuItem();
            조작법ToolStripMenuItem = new ToolStripMenuItem();
            ㄱToolStripMenuItem = new ToolStripMenuItem();
            종료ToolStripMenuItem = new ToolStripMenuItem();
            fileSystemWatcher1 = new FileSystemWatcher();
            strip_Status.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
            SuspendLayout();
            // 
            // timer_Game
            // 
            timer_Game.Interval = 200;
            timer_Game.Tick += timer_Game_Tick;
            // 
            // strip_Status
            // 
            strip_Status.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel_LV, toolStripStatusLabel_Jumsu, toolStripStatusLabel_Bodycnt });
            strip_Status.Location = new Point(0, 193);
            strip_Status.Name = "strip_Status";
            strip_Status.Size = new Size(415, 22);
            strip_Status.TabIndex = 0;
            // 
            // toolStripStatusLabel_LV
            // 
            toolStripStatusLabel_LV.Name = "toolStripStatusLabel_LV";
            toolStripStatusLabel_LV.Size = new Size(0, 17);
            // 
            // toolStripStatusLabel_Jumsu
            // 
            toolStripStatusLabel_Jumsu.Name = "toolStripStatusLabel_Jumsu";
            toolStripStatusLabel_Jumsu.Size = new Size(0, 17);
            // 
            // toolStripStatusLabel_Bodycnt
            // 
            toolStripStatusLabel_Bodycnt.Name = "toolStripStatusLabel_Bodycnt";
            toolStripStatusLabel_Bodycnt.Size = new Size(0, 17);
            // 
            // timer_Countdown
            // 
            timer_Countdown.Interval = 1000;
            timer_Countdown.Tick += timer_Countdown_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 메뉴MToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(415, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴MToolStripMenuItem
            // 
            메뉴MToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 시작ToolStripMenuItem, toolStripMenuItem1, 다시시작ToolStripMenuItem, 종료ToolStripMenuItem });
            메뉴MToolStripMenuItem.Name = "메뉴MToolStripMenuItem";
            메뉴MToolStripMenuItem.Size = new Size(74, 20);
            메뉴MToolStripMenuItem.Text = "메뉴 ( M )";
            // 
            // 시작ToolStripMenuItem
            // 
            시작ToolStripMenuItem.Name = "시작ToolStripMenuItem";
            시작ToolStripMenuItem.Size = new Size(154, 22);
            시작ToolStripMenuItem.Text = "시작 ( S )";
            시작ToolStripMenuItem.Click += 시작ToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(154, 22);
            toolStripMenuItem1.Text = "다시 시작 ( R )";
            toolStripMenuItem1.Click += 다시시작toolStripMenuItem1_Click;
            // 
            // 다시시작ToolStripMenuItem
            // 
            다시시작ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 조작법ToolStripMenuItem, ㄱToolStripMenuItem });
            다시시작ToolStripMenuItem.Name = "다시시작ToolStripMenuItem";
            다시시작ToolStripMenuItem.Size = new Size(154, 22);
            다시시작ToolStripMenuItem.Text = "게임 방법 설명";
            // 
            // 조작법ToolStripMenuItem
            // 
            조작법ToolStripMenuItem.Name = "조작법ToolStripMenuItem";
            조작법ToolStripMenuItem.Size = new Size(154, 22);
            조작법ToolStripMenuItem.Text = "조작법";
            조작법ToolStripMenuItem.Click += 조작법ToolStripMenuItem_Click;
            // 
            // ㄱToolStripMenuItem
            // 
            ㄱToolStripMenuItem.Name = "ㄱToolStripMenuItem";
            ㄱToolStripMenuItem.Size = new Size(154, 22);
            ㄱToolStripMenuItem.Text = "게임 진행 방법";
            ㄱToolStripMenuItem.Click += 게임진행방법ToolStripMenuItem_Click;
            // 
            // 종료ToolStripMenuItem
            // 
            종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            종료ToolStripMenuItem.Size = new Size(154, 22);
            종료ToolStripMenuItem.Text = "종료 ( X )";
            종료ToolStripMenuItem.Click += 종료ToolStripMenuItem_Click;
            // 
            // fileSystemWatcher1
            // 
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.SynchronizingObject = this;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 215);
            Controls.Add(strip_Status);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            strip_Status.ResumeLayout(false);
            strip_Status.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer_Game;
        private StatusStrip strip_Status;
        private ToolStripStatusLabel toolStripStatusLabel_LV;
        private ToolStripStatusLabel toolStripStatusLabel_Jumsu;
        private ToolStripStatusLabel toolStripStatusLabel_Bodycnt;
        private System.Windows.Forms.Timer timer_Countdown;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 메뉴MToolStripMenuItem;
        private ToolStripMenuItem 시작ToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem 다시시작ToolStripMenuItem;
        private ToolStripMenuItem 종료ToolStripMenuItem;
        private ToolStripMenuItem 조작법ToolStripMenuItem;
        private ToolStripMenuItem ㄱToolStripMenuItem;
        private FileSystemWatcher fileSystemWatcher1;
    }
}
