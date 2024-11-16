namespace WinFormsApp1
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
            this.MouseDown += new MouseEventHandler(this.Form1_MouseDown);
            this.MouseUp += new MouseEventHandler(this.Form1_MouseUp);
            this.MouseMove += new MouseEventHandler(this.Form1_MouseMove);



            this.SuspendLayout();
            // 
            // button_pen
            // 
            this.toolButton[0].Location = new System.Drawing.Point(12, 12);
            this.toolButton[0].Name = "button_pen";
            this.toolButton[0].Size = new System.Drawing.Size(39, 38);
            this.toolButton[0].TabIndex = 0;
            this.toolButton[0].Text = "Pen";
            this.toolButton[0].UseVisualStyleBackColor = true;
            this.toolButton[0].Click += new System.EventHandler(this.button_pen_Click);
            // 
            // button_paint
            // 
            this.toolButton[1].Location = new System.Drawing.Point(75, 12);
            this.toolButton[1].Name = "button_paint";
            this.toolButton[1].Size = new System.Drawing.Size(39, 38);
            this.toolButton[1].TabIndex = 0;
            this.toolButton[1].Text = "Paint";
            this.toolButton[1].UseVisualStyleBackColor = true;
            this.toolButton[1].Click += new System.EventHandler(this.button_paint_Click);
            // 
            // button_eraser
            // 
            this.toolButton[2].Location = new System.Drawing.Point(12, 56);
            this.toolButton[2].Name = "button_eraser";
            this.toolButton[2].Size = new System.Drawing.Size(39, 38);
            this.toolButton[2].TabIndex = 2;
            this.toolButton[2].Text = "지우개";
            this.toolButton[2].UseVisualStyleBackColor = true;
            this.toolButton[2].Click += new System.EventHandler(this.button_eraser_Click);
            // 
            // button_textbox
            // 
            this.toolButton[3].Location = new System.Drawing.Point(75, 56);
            this.toolButton[3].Name = "button_textbox";
            this.toolButton[3].Size = new System.Drawing.Size(39, 38);
            this.toolButton[3].TabIndex = 1;
            this.toolButton[3].Text = "텍스트박스";
            this.toolButton[3].UseVisualStyleBackColor = true;
            this.toolButton[3].Click += new System.EventHandler(this.button_textbox_Click);
            // 
            // button_line
            // 
            this.toolButton[4].Location = new System.Drawing.Point(12, 137);
            this.toolButton[4].Name = "button_line";
            this.toolButton[4].Size = new System.Drawing.Size(39, 38);
            this.toolButton[4].TabIndex = 5;
            this.toolButton[4].Text = "직선";
            this.toolButton[4].UseVisualStyleBackColor = true;
            this.toolButton[4].Click += new System.EventHandler(this.button_line_Click);
            // 
            // button_circle
            // 
            this.toolButton[5].Location = new System.Drawing.Point(75, 137);
            this.toolButton[5].Name = "button_circle";
            this.toolButton[5].Size = new System.Drawing.Size(39, 38);
            this.toolButton[5].TabIndex = 0;
            this.toolButton[5].Text = "원";
            this.toolButton[5].UseVisualStyleBackColor = true;
            this.toolButton[5].Click += new System.EventHandler(this.button_circle_Click);
            // 
            // button_triangle
            // 
            this.toolButton[6].Location = new System.Drawing.Point(12, 181);
            this.toolButton[6].Name = "button_triangle";
            this.toolButton[6].Size = new System.Drawing.Size(39, 38);
            this.toolButton[6].TabIndex = 0;
            this.toolButton[6].Text = "삼각형";
            this.toolButton[6].UseVisualStyleBackColor = true;
            this.toolButton[6].Click += new System.EventHandler(this.button_triangle_Click);
            // 
            // button_rectangle
            // 
            this.toolButton[7].Location = new System.Drawing.Point(75, 181);
            this.toolButton[7].Name = "button_rectangle";
            this.toolButton[7].Size = new System.Drawing.Size(39, 38);
            this.toolButton[7].TabIndex = 0;
            this.toolButton[7].Text = "사각형";
            this.toolButton[7].UseVisualStyleBackColor = true;
            this.toolButton[7].Click += new System.EventHandler(this.button_rectangle_Click);
            // 

            // button_colordlg1
            // 
            this.colorButton[0].Location = new System.Drawing.Point(12, 499);
            this.colorButton[0].Name = "button_colordlg1";
            this.colorButton[0].Size = new System.Drawing.Size(39, 38);
            this.colorButton[0].TabIndex = 0;
            this.colorButton[0].Text = "색1";
            this.colorButton[0].UseVisualStyleBackColor = true;
            this.colorButton[0].Click += new System.EventHandler(this.button_colordlg1_Click);
            // 
            // button_colordlg2
            // 
            this.colorButton[1].Location = new System.Drawing.Point(75, 499);
            this.colorButton[1].Name = "button_colordlg2";
            this.colorButton[1].Size = new System.Drawing.Size(39, 38);
            this.colorButton[1].TabIndex = 0;
            this.colorButton[1].Text = "색2";
            this.colorButton[1].UseVisualStyleBackColor = true;
            this.colorButton[1].Click += new System.EventHandler(this.button_colordlg2_Click);
            // 

            // button_radio1
            // 
            this.radioButton[0] = new RadioButton();
            this.radioButton[0].Checked = true;
            this.radioButton[0].AutoSize = true;
            this.radioButton[0].Location = new System.Drawing.Point(12, 400);
            this.radioButton[0].Name = "button_radio1";
            this.radioButton[0].Size = new System.Drawing.Size(39, 38);
            this.radioButton[0].TabIndex = 0;
            this.radioButton[0].TabStop = true;
            this.radioButton[0].Text = "윤곽선";
            this.radioButton[0].UseVisualStyleBackColor = true;
            this.radioButton[0].CheckedChanged += new EventHandler(button_radio1_CheckedChanged);
           
            // 
            // button_radio2
            // 
            this.radioButton[1] = new RadioButton();
            this.radioButton[1].AutoSize = true;
            this.radioButton[1].Location = new System.Drawing.Point(75, 400);
            this.radioButton[1].Name = "button_radio1";
            this.radioButton[1].Size = new System.Drawing.Size(39, 38);
            this.radioButton[1].TabIndex = 1;
            this.radioButton[1].TabStop = false;
            this.radioButton[1].Text = "채우기";
            this.radioButton[1].UseVisualStyleBackColor = true;
            this.radioButton[1].CheckedChanged += new EventHandler(button_radio2_CheckedChanged);
            // 

            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 635);
         

            for(int i = 0; i < 8; i++)
            {
                this.Controls.Add(this.toolButton[i]);
            }

            for (int i = 0; i < 2; i++)
            {
                this.Controls.Add(this.colorButton[i]);
                this.Controls.Add(this.radioButton[i]);
            }

            

            this.Name = "Form1";
            this.Text = "Test";
            this.ResumeLayout(false);

            initialize();
        }



        private void initialize()
        {
            colorButton[0].BackColor = color1;
            colorButton[1].BackColor = color2;
            gp = this.CreateGraphics();
        }

        private RadioButton[] radioButton = new RadioButton[2];

        #endregion
    }
}