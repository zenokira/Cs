﻿namespace Minesweeper
{
    partial class Setting
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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(46, 69);
            button1.Name = "button1";
            button1.Size = new Size(107, 45);
            button1.TabIndex = 0;
            button1.Text = "초급";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(182, 69);
            button2.Name = "button2";
            button2.Size = new Size(107, 45);
            button2.TabIndex = 0;
            button2.Text = "중급";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(316, 69);
            button3.Name = "button3";
            button3.Size = new Size(107, 45);
            button3.TabIndex = 0;
            button3.Text = "고급";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Setting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(449, 171);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Setting";
            Text = "Setting";
            Load += Setting_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
    }
}