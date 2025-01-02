namespace SnakeGame
{
    partial class Rule
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
            lbl_WinRuleText = new Label();
            lbl_WinRule = new Label();
            lbl_LvUpRule = new Label();
            lbl_LvUpRuleText = new Label();
            lbl_LoseRule = new Label();
            lbl_LoseRuleText = new Label();
            lbl_LvUp = new Label();
            lbl_LvUpText = new Label();
            SuspendLayout();
            // 
            // lbl_WinRuleText
            // 
            lbl_WinRuleText.AutoSize = true;
            lbl_WinRuleText.Font = new Font("맑은 고딕", 15F);
            lbl_WinRuleText.Location = new Point(60, 65);
            lbl_WinRuleText.Name = "lbl_WinRuleText";
            lbl_WinRuleText.Size = new Size(99, 28);
            lbl_WinRuleText.TabIndex = 0;
            lbl_WinRuleText.Text = "승리 조건";
            lbl_WinRuleText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_WinRule
            // 
            lbl_WinRule.AutoSize = true;
            lbl_WinRule.Font = new Font("맑은 고딕", 12F);
            lbl_WinRule.Location = new Point(165, 72);
            lbl_WinRule.Name = "lbl_WinRule";
            lbl_WinRule.Size = new Size(242, 21);
            lbl_WinRule.TabIndex = 1;
            lbl_WinRule.Text = "레벨 10에 도달하면 승리합니다.";
            lbl_WinRule.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl_LvUpRule
            // 
            lbl_LvUpRule.AutoSize = true;
            lbl_LvUpRule.Font = new Font("맑은 고딕", 12F);
            lbl_LvUpRule.Location = new Point(165, 140);
            lbl_LvUpRule.Name = "lbl_LvUpRule";
            lbl_LvUpRule.Size = new Size(538, 21);
            lbl_LvUpRule.TabIndex = 3;
            lbl_LvUpRule.Text = "각 레벨 별 주어진 일정 개수 이상의 먹이를 섭취하면 레벨이 올라갑니다.";
            lbl_LvUpRule.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl_LvUpRuleText
            // 
            lbl_LvUpRuleText.AutoSize = true;
            lbl_LvUpRuleText.Font = new Font("맑은 고딕", 15F);
            lbl_LvUpRuleText.Location = new Point(40, 134);
            lbl_LvUpRuleText.Name = "lbl_LvUpRuleText";
            lbl_LvUpRuleText.Size = new Size(119, 28);
            lbl_LvUpRuleText.TabIndex = 2;
            lbl_LvUpRuleText.Text = "레벨업 조건";
            lbl_LvUpRuleText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_LoseRule
            // 
            lbl_LoseRule.AutoSize = true;
            lbl_LoseRule.Font = new Font("맑은 고딕", 12F);
            lbl_LoseRule.Location = new Point(40, 325);
            lbl_LoseRule.Name = "lbl_LoseRule";
            lbl_LoseRule.Size = new Size(277, 63);
            lbl_LoseRule.TabIndex = 7;
            lbl_LoseRule.Text = "1. 경기장 밖으로 떨어질 경우\r\n2. 장애물에 머리를 부딪힐 경우\r\n3. 뱀이 자신의 몸통을 물어버린 경우";
            lbl_LoseRule.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl_LoseRuleText
            // 
            lbl_LoseRuleText.AutoSize = true;
            lbl_LoseRuleText.Font = new Font("맑은 고딕", 15F);
            lbl_LoseRuleText.Location = new Point(40, 287);
            lbl_LoseRuleText.Name = "lbl_LoseRuleText";
            lbl_LoseRuleText.Size = new Size(99, 28);
            lbl_LoseRuleText.TabIndex = 6;
            lbl_LoseRuleText.Text = "패배 조건";
            lbl_LoseRuleText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_LvUp
            // 
            lbl_LvUp.AutoSize = true;
            lbl_LvUp.Font = new Font("맑은 고딕", 10F);
            lbl_LvUp.Location = new Point(165, 194);
            lbl_LvUp.Name = "lbl_LvUp";
            lbl_LvUp.Size = new Size(380, 19);
            lbl_LvUp.TabIndex = 5;
            lbl_LvUp.Text = "레벨이 오를 수록 장애물의 개수와 뱀의 속도가 올라갑니다";
            lbl_LvUp.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl_LvUpText
            // 
            lbl_LvUpText.AutoSize = true;
            lbl_LvUpText.Font = new Font("맑은 고딕", 13F);
            lbl_LvUpText.Location = new Point(93, 188);
            lbl_LvUpText.Name = "lbl_LvUpText";
            lbl_LvUpText.Size = new Size(66, 25);
            lbl_LvUpText.TabIndex = 4;
            lbl_LvUpText.Text = "레벨업";
            lbl_LvUpText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Rule
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(731, 418);
            Controls.Add(lbl_LoseRule);
            Controls.Add(lbl_LoseRuleText);
            Controls.Add(lbl_LvUp);
            Controls.Add(lbl_LvUpText);
            Controls.Add(lbl_LvUpRule);
            Controls.Add(lbl_LvUpRuleText);
            Controls.Add(lbl_WinRule);
            Controls.Add(lbl_WinRuleText);
            Name = "Rule";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rule";
            Load += Rule_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbl_WinRuleText;
        private Label lbl_WinRule;
        private Label lbl_LvUpRule;
        private Label lbl_LvUpRuleText;
        private Label lbl_LoseRule;
        private Label lbl_LoseRuleText;
        private Label lbl_LvUp;
        private Label lbl_LvUpText;
    }
}