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
            lbl_Sea = new Label();
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            tb_RainTyping = new TextBox();
            timer2 = new System.Windows.Forms.Timer(components);
            lbl_Text = new Label();
            lbl_Jumsu = new Label();
            label1 = new Label();
            lbl_LV = new Label();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lb_WordList
            // 
            lb_WordList.FormattingEnabled = true;
            lb_WordList.ItemHeight = 15;
            lb_WordList.Location = new Point(29, 21);
            lb_WordList.Name = "lb_WordList";
            lb_WordList.Size = new Size(195, 304);
            lb_WordList.TabIndex = 0;
            // 
            // tb_TypingText
            // 
            tb_TypingText.Location = new Point(29, 343);
            tb_TypingText.Name = "tb_TypingText";
            tb_TypingText.Size = new Size(195, 23);
            tb_TypingText.TabIndex = 1;
            tb_TypingText.KeyDown += tb_TypingText_KeyDown;
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // pg_CorrectRate
            // 
            pg_CorrectRate.Location = new Point(128, 457);
            pg_CorrectRate.Name = "pg_CorrectRate";
            pg_CorrectRate.Size = new Size(96, 31);
            pg_CorrectRate.TabIndex = 2;
            // 
            // tb_Correct
            // 
            tb_Correct.Location = new Point(74, 385);
            tb_Correct.Name = "tb_Correct";
            tb_Correct.ReadOnly = true;
            tb_Correct.Size = new Size(85, 23);
            tb_Correct.TabIndex = 3;
            tb_Correct.Text = "0";
            tb_Correct.TextAlign = HorizontalAlignment.Center;
            // 
            // tb_Wrong
            // 
            tb_Wrong.Location = new Point(78, 420);
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
            lbl_Correct.Location = new Point(29, 388);
            lbl_Correct.Name = "lbl_Correct";
            lbl_Correct.Size = new Size(43, 15);
            lbl_Correct.TabIndex = 4;
            lbl_Correct.Text = "정답수";
            // 
            // lbl_Wrong
            // 
            lbl_Wrong.AutoSize = true;
            lbl_Wrong.Location = new Point(29, 423);
            lbl_Wrong.Name = "lbl_Wrong";
            lbl_Wrong.Size = new Size(43, 15);
            lbl_Wrong.TabIndex = 4;
            lbl_Wrong.Text = "오답수";
            // 
            // lbl_CorrectRateText
            // 
            lbl_CorrectRateText.AutoSize = true;
            lbl_CorrectRateText.Location = new Point(29, 473);
            lbl_CorrectRateText.Name = "lbl_CorrectRateText";
            lbl_CorrectRateText.Size = new Size(54, 15);
            lbl_CorrectRateText.TabIndex = 4;
            lbl_CorrectRateText.Text = "정답률 : ";
            // 
            // lbl_CorrectRate
            // 
            lbl_CorrectRate.AutoSize = true;
            lbl_CorrectRate.Location = new Point(89, 507);
            lbl_CorrectRate.Name = "lbl_CorrectRate";
            lbl_CorrectRate.Size = new Size(0, 15);
            lbl_CorrectRate.TabIndex = 4;
            lbl_CorrectRate.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_Sea
            // 
            lbl_Sea.BackColor = Color.Black;
            lbl_Sea.Location = new Point(326, 325);
            lbl_Sea.Name = "lbl_Sea";
            lbl_Sea.Size = new Size(561, 53);
            lbl_Sea.TabIndex = 6;
            lbl_Sea.Text = "label2";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 516);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(959, 22);
            statusStrip1.TabIndex = 7;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Maximum = 5;
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            // 
            // tb_RainTyping
            // 
            tb_RainTyping.Location = new Point(494, 415);
            tb_RainTyping.Name = "tb_RainTyping";
            tb_RainTyping.Size = new Size(227, 23);
            tb_RainTyping.TabIndex = 8;
            tb_RainTyping.KeyDown += tb_RainTyping_KeyDown;
            // 
            // timer2
            // 
            timer2.Interval = 1000;
            timer2.Tick += timer2_Tick;
            // 
            // lbl_Text
            // 
            lbl_Text.AutoSize = true;
            lbl_Text.Location = new Point(326, 418);
            lbl_Text.Name = "lbl_Text";
            lbl_Text.Size = new Size(42, 15);
            lbl_Text.TabIndex = 9;
            lbl_Text.Text = "점수 : ";
            // 
            // lbl_Jumsu
            // 
            lbl_Jumsu.Location = new Point(374, 420);
            lbl_Jumsu.Name = "lbl_Jumsu";
            lbl_Jumsu.Size = new Size(100, 25);
            lbl_Jumsu.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(326, 455);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 9;
            label1.Text = "레벨 : ";
            // 
            // lbl_LV
            // 
            lbl_LV.Location = new Point(374, 457);
            lbl_LV.Name = "lbl_LV";
            lbl_LV.Size = new Size(100, 25);
            lbl_LV.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(959, 538);
            Controls.Add(lbl_LV);
            Controls.Add(label1);
            Controls.Add(lbl_Jumsu);
            Controls.Add(lbl_Text);
            Controls.Add(tb_RainTyping);
            Controls.Add(statusStrip1);
            Controls.Add(lbl_Sea);
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
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
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
        private Label lbl_Sea;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar toolStripProgressBar1;
        private TextBox tb_RainTyping;
        private System.Windows.Forms.Timer timer2;
        private Label lbl_Text;
        private Label lbl_Jumsu;
        private Label label1;
        private Label lbl_LV;
    }
}
