namespace Typing_Program
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
            lb_WordList = new ListBox();
            tb_TypingText = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            pg_CorrectRate = new ProgressBar();
            tb_Correct = new TextBox();
            tb_Wrong = new TextBox();
            lbl_Correct = new Label();
            lbl_Wrong = new Label();
            lbl_CorrectRateText = new Label();
            lbl_CorrectRate = new Label();
            SuspendLayout();
            // 
            // lb_WordList
            // 
            lb_WordList.FormattingEnabled = true;
            lb_WordList.ItemHeight = 15;
            lb_WordList.Location = new Point(68, 37);
            lb_WordList.Name = "lb_WordList";
            lb_WordList.Size = new Size(323, 379);
            lb_WordList.TabIndex = 0;
            // 
            // tb_TypingText
            // 
            tb_TypingText.Location = new Point(68, 461);
            tb_TypingText.Name = "tb_TypingText";
            tb_TypingText.Size = new Size(323, 23);
            tb_TypingText.TabIndex = 1;
            tb_TypingText.KeyDown += tb_TypingText_KeyDown;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // pg_CorrectRate
            // 
            pg_CorrectRate.Location = new Point(432, 109);
            pg_CorrectRate.Name = "pg_CorrectRate";
            pg_CorrectRate.Size = new Size(323, 31);
            pg_CorrectRate.TabIndex = 2;
            // 
            // tb_Correct
            // 
            tb_Correct.Location = new Point(477, 37);
            tb_Correct.Name = "tb_Correct";
            tb_Correct.ReadOnly = true;
            tb_Correct.Size = new Size(85, 23);
            tb_Correct.TabIndex = 3;
            tb_Correct.Text = "0";
            tb_Correct.TextAlign = HorizontalAlignment.Center;
            // 
            // tb_Wrong
            // 
            tb_Wrong.Location = new Point(628, 37);
            tb_Wrong.Name = "tb_Wrong";
            tb_Wrong.ReadOnly = true;
            tb_Wrong.Size = new Size(85, 23);
            tb_Wrong.TabIndex = 3;
            tb_Wrong.Text = "0";
            tb_Wrong.TextAlign = HorizontalAlignment.Center;
            // 
            // lbl_Correct
            // 
            lbl_Correct.AutoSize = true;
            lbl_Correct.Location = new Point(432, 40);
            lbl_Correct.Name = "lbl_Correct";
            lbl_Correct.Size = new Size(43, 15);
            lbl_Correct.TabIndex = 4;
            lbl_Correct.Text = "정답수";
            // 
            // lbl_Wrong
            // 
            lbl_Wrong.AutoSize = true;
            lbl_Wrong.Location = new Point(579, 40);
            lbl_Wrong.Name = "lbl_Wrong";
            lbl_Wrong.Size = new Size(43, 15);
            lbl_Wrong.TabIndex = 4;
            lbl_Wrong.Text = "오답수";
            // 
            // lbl_CorrectRateText
            // 
            lbl_CorrectRateText.AutoSize = true;
            lbl_CorrectRateText.Location = new Point(432, 159);
            lbl_CorrectRateText.Name = "lbl_CorrectRateText";
            lbl_CorrectRateText.Size = new Size(54, 15);
            lbl_CorrectRateText.TabIndex = 4;
            lbl_CorrectRateText.Text = "정답률 : ";
            // 
            // lbl_CorrectRate
            // 
            lbl_CorrectRate.AutoSize = true;
            lbl_CorrectRate.Location = new Point(492, 159);
            lbl_CorrectRate.Name = "lbl_CorrectRate";
            lbl_CorrectRate.Size = new Size(0, 15);
            lbl_CorrectRate.TabIndex = 4;
            lbl_CorrectRate.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 588);
            Controls.Add(lbl_Wrong);
            Controls.Add(lbl_CorrectRate);
            Controls.Add(lbl_CorrectRateText);
            Controls.Add(lbl_Correct);
            Controls.Add(tb_Wrong);
            Controls.Add(tb_Correct);
            Controls.Add(pg_CorrectRate);
            Controls.Add(tb_TypingText);
            Controls.Add(lb_WordList);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lb_WordList;
        private TextBox tb_TypingText;
        private System.Windows.Forms.Timer timer1;
        private ProgressBar pg_CorrectRate;
        private TextBox tb_Correct;
        private TextBox tb_Wrong;
        private Label lbl_Correct;
        private Label lbl_Wrong;
        private Label lbl_CorrectRateText;
        private Label lbl_CorrectRate;
    }
}
