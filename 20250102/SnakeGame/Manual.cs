using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnakeGame
{
    public partial class Manual : Form
    {
        public const int FORM_WIDTH = 500;
        public const int FORM_HEIGHT = 500;
        public const int FORM_X_CENTER = FORM_WIDTH / 2;
        public const int FORM_Y_CENTER = FORM_HEIGHT / 2;

        const int COMMENT_X = OBJECT_X + 50;
        const int COMMENT_WIDTH = FORM_WIDTH - 200;
        const int COMMENT_HEIGHT = 50;
       
        const int OBJECT_X = 100;
        const int OBSTACLE_Y = 30;
        const int SNAKE_Y = 80;
        const int FEED_Y = 130;

        const int GAP_SIZE = 20;
        const int ARROW_KEY_LABEL_SIZE = 40;
        const int TEXT_SIZE = 400;
        public Manual()
        {
            InitializeComponent();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            Size size = this.Size - this.ClientSize;


            this.Width = size.Width + FORM_WIDTH;
            this.Height = size.Height + FORM_HEIGHT;

            Label left = createArrowKeyLabel(new Point (FORM_X_CENTER - ARROW_KEY_LABEL_SIZE - GAP_SIZE, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE / 2),0);
            Label right = createArrowKeyLabel(new Point(FORM_X_CENTER + GAP_SIZE, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE / 2), 1);
            Label up = createArrowKeyLabel(new Point(FORM_X_CENTER - ARROW_KEY_LABEL_SIZE/2, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE  - GAP_SIZE), 2);
            Label down = createArrowKeyLabel(new Point(FORM_X_CENTER - ARROW_KEY_LABEL_SIZE / 2, FORM_Y_CENTER + GAP_SIZE), 3);
            Label text = createTextLabel();

            Label obstacle = CreateObstacle();
            Label snake = CreateSnakeHead();
            Label feed = CreateFeed();

            Label obstacle_comment = CreateObstacleComment();
            Label snake_comment = CreateSnakeHeadComment();
            Label feed_comment = CreateFeedComment();


            Controls.Add(obstacle);
            Controls.Add(snake);
            Controls.Add(feed);

            Controls.Add(obstacle_comment);
            Controls.Add(snake_comment);
            Controls.Add(feed_comment);

            Controls.Add(left); 
            Controls.Add(right);
            Controls.Add(up);
            Controls.Add(down);
            Controls.Add(text);
        }

        Label createArrowKeyLabel(Point location , int n)
        {
            Font font = new Font(Font.Name, 20);
            Label lbl = new();
            lbl.Font = font;
            lbl.Name = "ArrowKey";
            lbl.BackColor = Color.Green;
            lbl.Size = new Size(ARROW_KEY_LABEL_SIZE, ARROW_KEY_LABEL_SIZE);
            lbl.TabIndex = 0;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Location = location;

            if (n == 0) lbl.Text = "◁";
            else if (n == 1) lbl.Text = "▷";
            else if (n == 2) lbl.Text = "△";
            else if (n == 3) lbl.Text = "▽";

            return lbl;
        }

        Label createTextLabel()
        {
            Font font = new Font(Font.Name, 15);
            Label lbl = new();
            lbl.Font = font;
            lbl.Name = "Manual";
            lbl.TabIndex = 0;
            lbl.Size = new Size(TEXT_SIZE, font.Height * 2  + 10);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Location = new Point ( 50, FORM_Y_CENTER + FORM_Y_CENTER / 3);
            lbl.Text = "방향키를 이용하여 \n뱀의 머리를 조종 할 수 있습니다";
            return lbl;
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
            lbl.Location = new Point(0, 0);
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
            lbl.Location = new Point(OBJECT_X + Feed.FEED_BLANK, FEED_Y + Feed.FEED_BLANK);

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
