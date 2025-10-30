namespace _2048
{
    partial class Record
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
            lv_record = new ListView();
            SuspendLayout();
            // 
            // lv_record
            // 
            lv_record.Location = new Point(12, 30);
            lv_record.Name = "lv_record";
            lv_record.Size = new Size(430, 253);
            lv_record.TabIndex = 0;
            lv_record.UseCompatibleStateImageBehavior = false;
            lv_record.View = View.Details;
            // 
            // Record
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(470, 352);
            Controls.Add(lv_record);
            Name = "Record";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Record";
            
            Load += Record_Load;
         
            ResumeLayout(false);
        }

        #endregion

        private ListView lv_record;
    }
}