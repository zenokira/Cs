using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SnakeGame
{
    public partial class Rule : Form
    {
        public const int FORM_WIDTH = 500;
        public const int FORM_HEIGHT = 500;
        public const int FORM_X_CENTER = FORM_WIDTH / 2;
        public const int FORM_Y_CENTER = FORM_HEIGHT / 2;

        const int COMMENT_X = OBJECT_X + 100;
        const int COMMENT_WIDTH = FORM_WIDTH - 200;
        const int COMMENT_HEIGHT = 50;
        const int OBJECT_X = 100;
        const int OBSTACLE_Y = 100;
        const int SNAKE_Y = 200;
        const int FEED_Y = 300;

        public Rule()
        {
            InitializeComponent();
        }

        private void Rule_Load(object sender, EventArgs e)
        {
            Size size = this.Size - this.ClientSize;

            this.Width = size.Width + FORM_WIDTH;
            this.Height = size.Height + FORM_HEIGHT;

            Label obstacle = CreateObstacle();
            Label snake = CreateSnakeHead();
            Label feed = CreateFeed();

            Label obstacle_comment = CreateObstacleComment();
            Label snake_comment = CreateSnakeHeadComment();
            Label feed_comment = CreateFeedComment();
            

            Controls.Add( obstacle );
            Controls.Add( snake );
            Controls.Add( feed );

            Controls.Add(obstacle_comment);
            Controls.Add(snake_comment);
            Controls.Add(feed_comment);
        }

        public Label CreateObstacle()
        {
            Label lbl = new();
            lbl.Name = "Obstacle";
            lbl.BackColor = Color.Gray;
            lbl.Size = new Size(Obstacle.OBSTACLE_SIZE, Obstacle.OBSTACLE_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point(OBJECT_X, OBSTACLE_Y);
            return lbl;
        }

        public Label CreateObstacleComment()
        {
            Label lbl = new();
            lbl.Name = "Comment";
            lbl.Size = new Size(COMMENT_WIDTH, COMMENT_HEIGHT);
            lbl.TabIndex = 0;
            lbl.Text = "장애물입니다.\n뱀의 머리가 부딪히면 죽으니 피해주세요";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point(COMMENT_X, OBSTACLE_Y);
            return lbl;
        }

        public Label CreateSnakeHead()
        {
            Label lbl = new();
            lbl.BackColor = Color.LightSeaGreen;
            lbl.Name = "Snake";
            lbl.Size = new Size(Snake.SNAKE_SIZE, Snake.SNAKE_SIZE);
            lbl.Location = new Point(0,0);
            lbl.TabIndex = 0;
            lbl.Text = "◁";
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Location = new Point(OBJECT_X, SNAKE_Y);
            return lbl;
        }
        public Label CreateSnakeHeadComment()
        {
            Label lbl = new();
            lbl.Name = "Comment";
            lbl.Size = new Size(COMMENT_WIDTH, COMMENT_HEIGHT);
            lbl.TabIndex = 0;
            lbl.Text = "뱀입니다.\n화살표가 앞으로 나아가는 방향입니다.\n돌아다니며 먹이를 먹어주세요";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point(COMMENT_X, SNAKE_Y);
            return lbl;
        }

        Label CreateFeed()
        {
            Label lbl = new();
            lbl.Name = "feed";
            lbl.BackColor = Color.IndianRed;
            lbl.Size = new Size(Feed.FEED_SIZE, Feed.FEED_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point (OBJECT_X + Feed.FEED_BLANK, FEED_Y + Feed.FEED_BLANK);

            return lbl;
        }

        public Label CreateFeedComment()
        {
            Label lbl = new();
            lbl.Name = "Comment";
            lbl.Size = new Size(COMMENT_WIDTH, COMMENT_HEIGHT);
            lbl.TabIndex = 0;
            lbl.Text = "먹이입니다.\n뱀이 먹으면 점수가 오르고 성장합니다";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = new Point(COMMENT_X, FEED_Y);
            return lbl;
        }
    }
}
