namespace _2048
{
    partial class GameMain
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
            btn_start = new Button();
            timer_save = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // btn_start
            // 
            btn_start.Location = new Point(372, 22);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(84, 37);
            btn_start.TabIndex = 0;
            btn_start.Text = "게임 시작";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // timer_save
            // 
            timer_save.Tick += timer_save_Tick;
            
            // 
            // GameMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 561);
            Controls.Add(btn_start);
            Name = "GameMain";
            Text = "Form1";
            FormClosing += GameMain_FormClosing;
            Load += GameMain_Load;
            Paint += GameMain_Paint;
            KeyDown += GameMain_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private Button btn_start;
        private System.Windows.Forms.Timer timer_save;
    }
}
