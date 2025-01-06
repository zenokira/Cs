using System.Windows.Forms;

namespace SnakeGame
{
    partial class SnakeGame
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
            timer_Countdown = new System.Windows.Forms.Timer(components);
            strip_Status = new StatusStrip();
            toolStripStatusLabel_LV = new ToolStripStatusLabel();
            toolStripStatusLabel_Bodycnt = new ToolStripStatusLabel();
            toolStripStatusLabel_Jumsu = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            메뉴ToolStripMenuItem = new ToolStripMenuItem();
            시작ToolStripMenuItem = new ToolStripMenuItem();
            다시시작ToolStripMenuItem = new ToolStripMenuItem();
            조작법ToolStripMenuItem = new ToolStripMenuItem();
            종료ToolStripMenuItem = new ToolStripMenuItem();
            strip_Status.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // timer_Game
            // 
            timer_Game.Tick += timer_Game_Tick;
            // 
            // timer_Countdown
            // 
            timer_Countdown.Interval = 500;
            timer_Countdown.Tick += timer_Countdown_Tick;
            // 
            // strip_Status
            // 
            strip_Status.AutoSize = false;
            strip_Status.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel_LV, toolStripStatusLabel_Bodycnt, toolStripStatusLabel_Jumsu });
            strip_Status.Location = new Point(0, 428);
            strip_Status.Name = "strip_Status";
            strip_Status.Size = new Size(800, 22);
            strip_Status.Stretch = false;
            strip_Status.TabIndex = 0;
            strip_Status.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_LV
            // 
            toolStripStatusLabel_LV.Name = "toolStripStatusLabel_LV";
            toolStripStatusLabel_LV.Size = new Size(32, 17);
            toolStripStatusLabel_LV.Text = "LV : ";
            // 
            // toolStripStatusLabel_Bodycnt
            // 
            toolStripStatusLabel_Bodycnt.Name = "toolStripStatusLabel_Bodycnt";
            toolStripStatusLabel_Bodycnt.Size = new Size(70, 17);
            toolStripStatusLabel_Bodycnt.Text = "몸통 길이 : ";
            // 
            // toolStripStatusLabel_Jumsu
            // 
            toolStripStatusLabel_Jumsu.Name = "toolStripStatusLabel_Jumsu";
            toolStripStatusLabel_Jumsu.Size = new Size(42, 17);
            toolStripStatusLabel_Jumsu.Text = "점수 : ";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 메뉴ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            메뉴ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 시작ToolStripMenuItem, 다시시작ToolStripMenuItem, 조작법ToolStripMenuItem, 종료ToolStripMenuItem });
            메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            메뉴ToolStripMenuItem.Size = new Size(43, 20);
            메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 시작ToolStripMenuItem
            // 
            시작ToolStripMenuItem.Name = "시작ToolStripMenuItem";
            시작ToolStripMenuItem.Size = new Size(126, 22);
            시작ToolStripMenuItem.Text = "시작";
            시작ToolStripMenuItem.Click += 시작ToolStripMenuItem_Click;
            // 
            // 다시시작ToolStripMenuItem
            // 
            다시시작ToolStripMenuItem.Name = "다시시작ToolStripMenuItem";
            다시시작ToolStripMenuItem.Size = new Size(126, 22);
            다시시작ToolStripMenuItem.Text = "다시 시작";
            다시시작ToolStripMenuItem.Click += 다시시작ToolStripMenuItem_Click;
            // 
            // 조작법ToolStripMenuItem
            // 
            조작법ToolStripMenuItem.Name = "조작법ToolStripMenuItem";
            조작법ToolStripMenuItem.Size = new Size(126, 22);
            조작법ToolStripMenuItem.Text = "조작법";
            조작법ToolStripMenuItem.Click += 조작법ToolStripMenuItem_Click;
            // 
            // 종료ToolStripMenuItem
            // 
            종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            종료ToolStripMenuItem.Size = new Size(126, 22);
            종료ToolStripMenuItem.Text = "종료";
            종료ToolStripMenuItem.Click += 종료ToolStripMenuItem_Click;
            // 
            // SnakeGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(800, 450);
            Controls.Add(strip_Status);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            Name = "SnakeGame";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Load += SnakeGame_Load;
            strip_Status.ResumeLayout(false);
            strip_Status.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private System.Windows.Forms.Timer timer_Game;
        private System.Windows.Forms.Timer timer_Countdown;
        private StatusStrip strip_Status;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem 메뉴ToolStripMenuItem;
        private ToolStripMenuItem 시작ToolStripMenuItem;
        private ToolStripMenuItem 다시시작ToolStripMenuItem;
        private ToolStripMenuItem 조작법ToolStripMenuItem;
        private ToolStripMenuItem 종료ToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel_LV;
        private ToolStripStatusLabel toolStripStatusLabel_Bodycnt;
        private ToolStripStatusLabel toolStripStatusLabel_Jumsu;
    }
}
