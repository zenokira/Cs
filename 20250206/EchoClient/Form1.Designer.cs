namespace EchoClient
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
            btn_SEND = new Button();
            label1 = new Label();
            listBox_MSGLIST = new ListBox();
            textBox_MSG = new TextBox();
            SuspendLayout();
            // 
            // btn_SEND
            // 
            btn_SEND.Location = new Point(599, 396);
            btn_SEND.Name = "btn_SEND";
            btn_SEND.Size = new Size(85, 23);
            btn_SEND.TabIndex = 0;
            btn_SEND.Text = "전송";
            btn_SEND.UseVisualStyleBackColor = true;
            btn_SEND.Click += btn_SEND_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(51, 42);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // listBox_MSGLIST
            // 
            listBox_MSGLIST.FormattingEnabled = true;
            listBox_MSGLIST.ItemHeight = 15;
            listBox_MSGLIST.Location = new Point(38, 90);
            listBox_MSGLIST.Name = "listBox_MSGLIST";
            listBox_MSGLIST.Size = new Size(546, 274);
            listBox_MSGLIST.TabIndex = 2;
            // 
            // textBox_MSG
            // 
            textBox_MSG.Location = new Point(38, 396);
            textBox_MSG.Name = "textBox_MSG";
            textBox_MSG.Size = new Size(546, 23);
            textBox_MSG.TabIndex = 3;
            textBox_MSG.TextChanged += textBox_MSG_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 547);
            Controls.Add(textBox_MSG);
            Controls.Add(listBox_MSGLIST);
            Controls.Add(label1);
            Controls.Add(btn_SEND);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_SEND;
        private Label label1;
        private ListBox listBox_MSGLIST;
        private TextBox textBox_MSG;
    }
}
