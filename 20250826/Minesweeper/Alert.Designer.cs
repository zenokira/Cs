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
            lb_MSG = new Label();
            SuspendLayout();
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
            ClientSize = new Size(384, 101);
            Controls.Add(lb_MSG);
            Name = "Alert";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Alert";
            Load += Alert_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lb_MSG;
    }
}