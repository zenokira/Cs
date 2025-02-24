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
            SuspendLayout();
            // 
            // timer_PlayTime
            // 
            timer_PlayTime.Interval = 1000;
            timer_PlayTime.Tick += timer_PlayTime_Tick;
            // 
            // 지뢰찾기
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 476);
            Name = "지뢰찾기";
            Text = "지뢰찾기";
            Load += Form1_Load;
            MouseClick += 지뢰찾기_MouseClick;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer_PlayTime;
    }
}
