using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ComputerCheck
{
    partial class 강의실현황판
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
            groupBox_6F = new GroupBox();
            button_CLASS4 = new Button();
            button_CLASS3 = new Button();
            button_CLASS2 = new Button();
            button_CLASS1 = new Button();
            groupBox_7F = new GroupBox();
            button_CLASS9 = new Button();
            button_CLASS8 = new Button();
            button_CLASS7 = new Button();
            button_CLASS6 = new Button();
            groupBox_6F.SuspendLayout();
            groupBox_7F.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox_6F
            // 
            groupBox_6F.Controls.Add(button_CLASS4);
            groupBox_6F.Controls.Add(button_CLASS3);
            groupBox_6F.Controls.Add(button_CLASS2);
            groupBox_6F.Controls.Add(button_CLASS1);
            groupBox_6F.Location = new Point(40, 20);
            groupBox_6F.Name = "groupBox_6F";
            groupBox_6F.Size = new Size(706, 200);
            groupBox_6F.TabIndex = 0;
            groupBox_6F.TabStop = false;
            groupBox_6F.Text = "6층";
            // 
            // button_CLASS4
            // 
            button_CLASS4.Location = new Point(526, 59);
            button_CLASS4.Name = "button_CLASS4";
            button_CLASS4.Size = new Size(125, 75);
            button_CLASS4.TabIndex = 3;
            button_CLASS4.Text = "4강의실";
            button_CLASS4.UseVisualStyleBackColor = true;
            button_CLASS4.Click += button_Click;
            // 
            // button_CLASS3
            // 
            button_CLASS3.Location = new Point(359, 59);
            button_CLASS3.Name = "button_CLASS3";
            button_CLASS3.Size = new Size(125, 75);
            button_CLASS3.TabIndex = 2;
            button_CLASS3.Text = "3강의실";
            button_CLASS3.UseVisualStyleBackColor = true;
            button_CLASS3.Click += button_Click;
            // 
            // button_CLASS2
            // 
            button_CLASS2.Location = new Point(201, 59);
            button_CLASS2.Name = "button_CLASS2";
            button_CLASS2.Size = new Size(125, 75);
            button_CLASS2.TabIndex = 1;
            button_CLASS2.Text = "2강의실";
            button_CLASS2.UseVisualStyleBackColor = true;
            button_CLASS2.Click += button_Click;
            // 
            // button_CLASS1
            // 
            button_CLASS1.Location = new Point(50, 59);
            button_CLASS1.Name = "button_CLASS1";
            button_CLASS1.Size = new Size(125, 75);
            button_CLASS1.TabIndex = 0;
            button_CLASS1.Text = "1강의실";
            button_CLASS1.UseVisualStyleBackColor = true;
            button_CLASS1.Click += button_Click;
            // 
            // groupBox_7F
            // 
            groupBox_7F.Controls.Add(button_CLASS9);
            groupBox_7F.Controls.Add(button_CLASS8);
            groupBox_7F.Controls.Add(button_CLASS7);
            groupBox_7F.Controls.Add(button_CLASS6);
            groupBox_7F.Location = new Point(40, 236);
            groupBox_7F.Name = "groupBox_7F";
            groupBox_7F.Size = new Size(706, 200);
            groupBox_7F.TabIndex = 1;
            groupBox_7F.TabStop = false;
            groupBox_7F.Text = "7층";
            // 
            // button_CLASS9
            // 
            button_CLASS9.Location = new Point(526, 67);
            button_CLASS9.Name = "button_CLASS9";
            button_CLASS9.Size = new Size(125, 75);
            button_CLASS9.TabIndex = 5;
            button_CLASS9.Text = "9강의실";
            button_CLASS9.UseVisualStyleBackColor = true;
            button_CLASS9.Click += button_Click;
            // 
            // button_CLASS8
            // 
            button_CLASS8.Location = new Point(359, 67);
            button_CLASS8.Name = "button_CLASS8";
            button_CLASS8.Size = new Size(125, 75);
            button_CLASS8.TabIndex = 4;
            button_CLASS8.Text = "8강의실";
            button_CLASS8.UseVisualStyleBackColor = true;
            button_CLASS8.Click += button_Click;
            // 
            // button_CLASS7
            // 
            button_CLASS7.Location = new Point(201, 67);
            button_CLASS7.Name = "button_CLASS7";
            button_CLASS7.Size = new Size(125, 75);
            button_CLASS7.TabIndex = 3;
            button_CLASS7.Text = "7강의실";
            button_CLASS7.UseVisualStyleBackColor = true;
            button_CLASS7.Click += button_Click;
            // 
            // button_CLASS6
            // 
            button_CLASS6.Location = new Point(50, 67);
            button_CLASS6.Name = "button_CLASS6";
            button_CLASS6.Size = new Size(125, 75);
            button_CLASS6.TabIndex = 2;
            button_CLASS6.Text = "6강의실";
            button_CLASS6.UseVisualStyleBackColor = true;
            button_CLASS6.Click += button_Click;
            // 
            // 강의실현황판
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(groupBox_7F);
            Controls.Add(groupBox_6F);
            Name = "강의실현황판";
            Text = "강의실현황판";
            FormClosing += 강의실현황판_FormClosing;
            Load += 강의실현황판_Load;
            groupBox_6F.ResumeLayout(false);
            groupBox_7F.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox_6F;
        private Button button_CLASS4;
        private Button button_CLASS3;
        private Button button_CLASS2;
        private Button button_CLASS1;
        private GroupBox groupBox_7F;
        private Button button_CLASS9;
        private Button button_CLASS8;
        private Button button_CLASS7;
        private Button button_CLASS6;
    }
}