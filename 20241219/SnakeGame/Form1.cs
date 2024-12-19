
using System.Diagnostics;
using System.Windows.Forms;

namespace SnakeGame
{

    public partial class Form1 : Form
    {
        const int LVUP_POINT = 10;
        const int TICK_REDUCE = 50;
        const int FEED_BLANK = 5;
        const int FEED_SIZE = 20;
        const int PIXEL_CNT = 10;
        const int PIXEL_SIZE = 30;
        const int TIMER_INTERVAL = 400;

        bool keydownFlag = false;

        Random rand = new Random();
        Snake snake;
        List<Label> feedList = new List<Label>();
        int Jumsu = 0;
        int Length = 0;
        int LV = 1;


        Size pixel = new Size(PIXEL_SIZE, PIXEL_SIZE);
        Rectangle gamepan = new();
        public Form1()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            Jumsu = 0;
            LV = 1;
            Length = 0;
            timer_Game.Interval = TIMER_INTERVAL;
            feedList.Clear();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            // ≥–¿Ã 16 ≥Ù¿Ã 39  (this.Size - this.ClientSize)
            Size size = this.Size - this.ClientSize + pixel * PIXEL_CNT ;
            this.Width = size.Width;
            this.Height = size.Height + strip_Status.Height;

            gamepan.Size = pixel * PIXEL_CNT;

            snake = new Snake(Controls);
            snake.GetSnakeHead();


            
            StatusTextUpdate();
            timer_Game.Start();
        }


        void StatusTextUpdate()
        {
            toolStripStatusLabel_LV.Text = "LV : " + LV.ToString();
            toolStripStatusLabel_Jumsu.Text = "¡°ºˆ : " + Jumsu.ToString();
            toolStripStatusLabel_Bodycnt.Text = "∏ˆ≈Î±Ê¿Ã : " + Length.ToString();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keydownFlag) return true;
            if (keyData == Keys.Left)
            {
                snake.Direction(SnakeVector.LEFT);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Right)
            {
                snake.Direction(SnakeVector.RIGHT);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Up)
            {
                snake.Direction(SnakeVector.UP);
                keydownFlag = true;
                return true;
            }
            else if (keyData == Keys.Down)
            {
                snake.Direction(SnakeVector.DOWN);
                keydownFlag = true;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        int tick = 0;
        private void timer_Game_Tick(object sender, EventArgs e)
        {
            keydownFlag = false;
            snake.Go();
            tick++;
            if (isGameOver())
            {
                timer_Game.Stop();
            }
            if (isSnakeFeeding())
            {
                snake.Growth();
                Length++;
                Jumsu++;

                if(isLevelUP())  
                    LevelUP();

            }
            //if (tick % 5 == 0)
            {
                Label feed = CreateFeed();

                addFeed(feed);
            }
            StatusTextUpdate();
        }

        void addFeed(Label feed)
        {
            feedList.Add(feed);
            Controls.Add(feed);
        }

        void removeFeed(Label feed)
        {
            feedList.Remove(feed);
            Controls.Remove(feed);
        }

        bool isSnakeFeeding()
        {
            foreach (var feed in feedList)
            {
                if (snake.SnakeHeadRect().Contains(feed.Location))
                {
                    removeFeed(feed);
                    return true;
                }
            }
            return false;
        }
        bool isGameOver()
        {
            Label lbl = snake.GetSnakeHead();
            if (lbl.Top < gamepan.Top || lbl.Left < gamepan.Left ||
                lbl.Bottom > gamepan.Bottom || lbl.Right > gamepan.Right)
                return true;
            else if (isSnakeBiteSelf())
                return true;
            else
                return false;
        }

        bool isSnakeBiteSelf()
        {
            foreach (var body in snake.GetSnakeBody())
            {
                if (snake.getHeadLocation() == body.Location)
                    return true;
            }
            return false;
        }

        Label CreateFeed()
        {
            Label lbl = new();
            lbl.Name = "feed";
            lbl.BackColor = Color.IndianRed;
            lbl.Size = new Size(FEED_SIZE, FEED_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = RandomFeedLocation();
            return lbl;
        }

        Point RandomFeedLocation()
        {
            int x = gamepan.Width / PIXEL_SIZE;
            int y = gamepan.Height / PIXEL_SIZE;

            Point pt = new(); ;
            bool flag = false;

            while (!flag)
            {
                pt = new Point(rand.Next(x) * PIXEL_SIZE + FEED_BLANK, rand.Next(y) * PIXEL_SIZE + FEED_BLANK);

                if (snake.SnakeHeadRect().Contains(pt)) continue;

                flag = true;
                
                foreach (var item in feedList)
                {
                    if (item.Location == pt) flag = false;
                }

                foreach (var item in snake.GetSnakeBody())
                {
                    Rectangle rect = new Rectangle(item.Location,item.Size);
                    if (rect.Contains(pt)) flag = false;
                }
            }

            return pt;
        }

        void LevelUP()
        {
            timer_Game.Stop();
            snake.BodyClear();
            removeFeed();
            LV++;
            Jumsu = 0;
            Length = snake.BodyCount();
            timer_Game.Interval = TIMER_INTERVAL - LV * TICK_REDUCE;

            timer_Game.Start();
        }

        void removeFeed()
        {
            int cnt = feedList.Count;
            for (int i = cnt - 1; i >= 0; i--)
            {
                Controls.Remove(feedList[i]);
                feedList.RemoveAt(i);
            }
        }
        bool isLevelUP()
        {
            return Jumsu >= LVUP_POINT;
        }
    }
}
