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
            SuspendLayout();
            // 
            // timer_Game
            // 
            timer_Game.Interval = 200;
            timer_Game.Tick += timer_Game_Tick;
            // 
            // strip_Status
            // 
            strip_Status.Location = new Point(0, 610);
            strip_Status.Name = "strip_Status";
            strip_Status.Size = new Size(996, 22);
            strip_Status.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(996, 632);
            Controls.Add(strip_Status);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer_Game;
        private StatusStrip strip_Status;
    }
}
