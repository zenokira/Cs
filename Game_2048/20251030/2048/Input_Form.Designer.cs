namespace _2048
{
    partial class Input_Form
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
            label1 = new Label();
            label2 = new Label();
            tb_Name = new TextBox();
            btn_Add = new Button();
            btn_Cancel = new Button();
            tb_MoveCnt = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 12F);
            label1.Location = new Point(82, 69);
            label1.Name = "label1";
            label1.Size = new Size(58, 21);
            label1.TabIndex = 0;
            label1.Text = "점수 : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 12F);
            label2.Location = new Point(28, 28);
            label2.Name = "label2";
            label2.Size = new Size(112, 21);
            label2.TabIndex = 0;
            label2.Text = "등록할 이름 : ";
            // 
            // tb_Name
            // 
            tb_Name.Font = new Font("맑은 고딕", 12F);
            tb_Name.Location = new Point(146, 25);
            tb_Name.Name = "tb_Name";
            tb_Name.Size = new Size(157, 29);
            tb_Name.TabIndex = 1;
            // 
            // btn_Add
            // 
            btn_Add.Location = new Point(92, 118);
            btn_Add.Name = "btn_Add";
            btn_Add.Size = new Size(116, 34);
            btn_Add.TabIndex = 2;
            btn_Add.Text = "등록";
            btn_Add.UseVisualStyleBackColor = true;
            btn_Add.Click += btn_Add_Click;
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new Point(232, 118);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new Size(116, 34);
            btn_Cancel.TabIndex = 2;
            btn_Cancel.Text = "취소";
            btn_Cancel.UseVisualStyleBackColor = true;
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // tb_MoveCnt
            // 
            tb_MoveCnt.Font = new Font("맑은 고딕", 12F);
            tb_MoveCnt.Location = new Point(146, 66);
            tb_MoveCnt.Name = "tb_MoveCnt";
            tb_MoveCnt.ReadOnly = true;
            tb_MoveCnt.Size = new Size(157, 29);
            tb_MoveCnt.TabIndex = 3;
            // 
            // Input_Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 164);
            Controls.Add(tb_MoveCnt);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_Add);
            Controls.Add(tb_Name);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Input_Form";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Input_Form";
            Load += Input_Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tb_Name;
        private Button btn_Add;
        private Button btn_Cancel;
        private TextBox tb_MoveCnt;
    }
}