namespace Minesweeper
{
    partial class Alert
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_YES = new Button();
            btn_NO = new Button();
            lb_MSG = new Label();
            SuspendLayout();
            // 
            // btn_YES
            // 
            btn_YES.Location = new Point(99, 67);
            btn_YES.Name = "btn_YES";
            btn_YES.Size = new Size(82, 22);
            btn_YES.TabIndex = 0;
            btn_YES.Text = "YES";
            btn_YES.UseVisualStyleBackColor = true;
            btn_YES.Click += btn_YES_Click;
            // 
            // btn_NO
            // 
            btn_NO.Location = new Point(207, 67);
            btn_NO.Name = "btn_NO";
            btn_NO.Size = new Size(82, 22);
            btn_NO.TabIndex = 0;
            btn_NO.Text = "NO";
            btn_NO.UseVisualStyleBackColor = true;
            btn_NO.Click += btn_NO_Click;
            // 
            // lb_MSG
            // 
            lb_MSG.AutoSize = true;
            lb_MSG.Location = new Point(20, 16);
            lb_MSG.Name = "lb_MSG";
            lb_MSG.Size = new Size(0, 15);
            lb_MSG.TabIndex = 1;
            // 
            // Alert
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 101);
            Controls.Add(lb_MSG);
            Controls.Add(btn_NO);
            Controls.Add(btn_YES);
            Name = "Alert";
            Text = "Alert";
            Load += Alert_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_YES;
        private Button btn_NO;
        private Label lb_MSG;
    }
}