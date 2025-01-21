namespace SnakeGame
{
    partial class Manual
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
            lbl_Obstacle = new Label();
            lbl_Snake = new Label();
            lbl_Feed = new Label();
            lbl_CMTObstacle = new Label();
            label1 = new Label();
            label2 = new Label();
            lbl_ArrowUp = new Label();
            lbl_ArrowLeft = new Label();
            label3 = new Label();
            lbl_ArrowRight = new Label();
            lbl_Comment = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // lbl_Obstacle
            // 
            lbl_Obstacle.BackColor = Color.Gray;
            lbl_Obstacle.Location = new Point(70, 40);
            lbl_Obstacle.Name = "lbl_Obstacle";
            lbl_Obstacle.Size = new Size(30, 30);
            lbl_Obstacle.TabIndex = 0;
            lbl_Obstacle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_Snake
            // 
            lbl_Snake.BackColor = Color.LightSeaGreen;
            lbl_Snake.Location = new Point(70, 100);
            lbl_Snake.Name = "lbl_Snake";
            lbl_Snake.Size = new Size(30, 30);
            lbl_Snake.TabIndex = 1;
            lbl_Snake.Text = "◁";
            lbl_Snake.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_Feed
            // 
            lbl_Feed.BackColor = Color.IndianRed;
            lbl_Feed.Location = new Point(75, 165);
            lbl_Feed.Name = "lbl_Feed";
            lbl_Feed.Size = new Size(20, 20);
            lbl_Feed.TabIndex = 2;
            lbl_Feed.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_CMTObstacle
            // 
            lbl_CMTObstacle.AutoSize = true;
            lbl_CMTObstacle.Location = new Point(130, 40);
            lbl_CMTObstacle.Name = "lbl_CMTObstacle";
            lbl_CMTObstacle.Size = new Size(227, 30);
            lbl_CMTObstacle.TabIndex = 3;
            lbl_CMTObstacle.Text = "장애물입니다.\r\n뱀의 머리가 부딪히면 죽으니 피해주세요";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(130, 160);
            label1.Name = "label1";
            label1.Size = new Size(286, 45);
            label1.TabIndex = 4;
            label1.Text = "먹이입니다.\r\n뱀이 먹으면 점수가 오르고 성장합니다\r\n레벨별로 먹어야 하는 먹이 개수는 점점 늘어납니다.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(130, 100);
            label2.Name = "label2";
            label2.Size = new Size(214, 45);
            label2.TabIndex = 5;
            label2.Text = "뱀입니다.\r\n화살표가 앞으로 나아가는 방향입니다.\r\n돌아다니며 먹이를 먹어주세요";
            // 
            // lbl_ArrowUp
            // 
            lbl_ArrowUp.BackColor = Color.Green;
            lbl_ArrowUp.Font = new Font("맑은 고딕", 15F);
            lbl_ArrowUp.Location = new Point(235, 230);
            lbl_ArrowUp.Name = "lbl_ArrowUp";
            lbl_ArrowUp.Size = new Size(30, 30);
            lbl_ArrowUp.TabIndex = 6;
            lbl_ArrowUp.Text = "△";
            lbl_ArrowUp.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_ArrowLeft
            // 
            lbl_ArrowLeft.BackColor = Color.Green;
            lbl_ArrowLeft.Font = new Font("맑은 고딕", 15F);
            lbl_ArrowLeft.Location = new Point(205, 260);
            lbl_ArrowLeft.Name = "lbl_ArrowLeft";
            lbl_ArrowLeft.Size = new Size(30, 30);
            lbl_ArrowLeft.TabIndex = 10;
            lbl_ArrowLeft.Text = "◁";
            lbl_ArrowLeft.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.Green;
            label3.Font = new Font("맑은 고딕", 15F);
            label3.Location = new Point(235, 290);
            label3.Name = "label3";
            label3.Size = new Size(30, 30);
            label3.TabIndex = 11;
            label3.Text = "▽";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_ArrowRight
            // 
            lbl_ArrowRight.BackColor = Color.Green;
            lbl_ArrowRight.Font = new Font("맑은 고딕", 15F);
            lbl_ArrowRight.Location = new Point(265, 260);
            lbl_ArrowRight.Name = "lbl_ArrowRight";
            lbl_ArrowRight.Size = new Size(30, 30);
            lbl_ArrowRight.TabIndex = 12;
            lbl_ArrowRight.Text = "▷";
            lbl_ArrowRight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_Comment
            // 
            lbl_Comment.AutoSize = true;
            lbl_Comment.Font = new Font("맑은 고딕", 13F);
            lbl_Comment.Location = new Point(110, 330);
            lbl_Comment.Name = "lbl_Comment";
            lbl_Comment.Size = new Size(276, 50);
            lbl_Comment.TabIndex = 13;
            lbl_Comment.Text = "방향키를 이용하여\r\n뱀의 머리를 조종 할 수 있습니다";
            lbl_Comment.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 15F);
            label4.ForeColor = SystemColors.HotTrack;
            label4.Location = new Point(80, 390);
            label4.Name = "label4";
            label4.Size = new Size(348, 28);
            label4.TabIndex = 14;
            label4.Text = "★10레벨을 클리어하면 승리합니다★";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Manual
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(484, 461);
            Controls.Add(label4);
            Controls.Add(lbl_Comment);
            Controls.Add(lbl_ArrowRight);
            Controls.Add(label3);
            Controls.Add(lbl_ArrowLeft);
            Controls.Add(lbl_ArrowUp);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lbl_CMTObstacle);
            Controls.Add(lbl_Feed);
            Controls.Add(lbl_Snake);
            Controls.Add(lbl_Obstacle);
            Name = "Manual";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "메뉴얼";
            Load += Manual_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbl_Obstacle;
        private Label lbl_Snake;
        private Label lbl_Feed;
        private Label lbl_CMTObstacle;
        private Label label1;
        private Label label2;
        private Label lbl_ArrowUp;
        private Label lbl_ArrowLeft;
        private Label label3;
        private Label lbl_ArrowRight;
        private Label lbl_Comment;
        private Label label4;
    }
}