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
            strip_Status.SuspendLayout();
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 215);
            Controls.Add(strip_Status);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            strip_Status.ResumeLayout(false);
            strip_Status.PerformLayout();
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
    }
}
