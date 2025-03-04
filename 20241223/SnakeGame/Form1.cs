
using System.Diagnostics;
using System.Windows.Forms;

namespace SnakeGame
{

    public partial class Form1 : Form
    {
        const int LVUP_POINT = 10;
        const int TICK_REDUCE = 40;
        const int FEED_BLANK = 5;
        const int FEED_SIZE = 20;
        const int FEED_TICK = 10;
        const int TIMER_INTERVAL = 400;

        public const int PIXEL_CNT = 20;
        public const int PIXEL_SIZE = 30;
        public const int CENTER_POINT = PIXEL_SIZE * (PIXEL_CNT / 2);
        
        Point[] NotUsePoint = new Point[4]{
            new Point(CENTER_POINT - PIXEL_SIZE, CENTER_POINT),
            new Point(CENTER_POINT + PIXEL_SIZE, CENTER_POINT),
            new Point(CENTER_POINT, CENTER_POINT - PIXEL_SIZE),
            new Point(CENTER_POINT, CENTER_POINT + PIXEL_SIZE)
        };
        
        bool keydownFlag = false;

        Random rand = new Random();

        Snake snake;
        Obstacle obstacle;
        List<Label> feedList = new List<Label>();
        int Jumsu = 0;
        int Length = 0;
        int LV = 1;
        int feed_tick = FEED_TICK;

        List<Point> useLocationList = new List<Point>();

        Point CenterPoint = new Point(CENTER_POINT, CENTER_POINT);
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

        private void Restart()
        {
            useLocationList.Clear();
            snake.BodyClear();
            removeFeed();
            removeObstacle();

            Initialize();
            snake.setHeadLocation(CenterPoint);
            setObstacle();
            StatusTextUpdate();
            timer_Game.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            // 넓이 16 높이 39  (this.Size - this.ClientSize)
            Size size = this.Size - this.ClientSize + pixel * PIXEL_CNT ;
            this.Width = size.Width;
            this.Height = size.Height + strip_Status.Height;

            gamepan.Size = pixel * PIXEL_CNT;

            obstacle = new Obstacle(Controls);
            snake = new Snake(Controls);
            snake.setHeadLocation(CenterPoint);


            setObstacle();
            StatusTextUpdate();
            timer_Game.Start();
        }


        void StatusTextUpdate()
        {
            toolStripStatusLabel_LV.Text = "LV : " + LV.ToString();
            toolStripStatusLabel_Jumsu.Text = "점수 : " + Jumsu.ToString();
            toolStripStatusLabel_Bodycnt.Text = "몸통길이 : " + Length.ToString();
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

            int resultGameover = isGameOver();
            if (resultGameover != 0)
            {
                timer_Game.Stop();
                string msg = "";
                if (resultGameover == 1) msg = "경기장 밖으로 떨어졌습니다.";
                else if (resultGameover == 2) msg = "스스로를 물어버렸습니다.";
                else if (resultGameover == 3) msg = "장애물에 머리를 부딪쳐 죽었습니다.";

                msg += "\n다시 시작하시겠습니까?";
                if (MessageBox.Show(msg, "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Restart();
                }
                else
                {
                    
                }
            }
            if (isSnakeFeeding())
            {
                snake.Growth();
                Length++;
                Jumsu++;

                if(isLevelUP())  
                    LevelUP();

            }
            
        
            if (tick % feed_tick == 0)
            {
                Label feed = CreateFeed();
                useLocationList.Add(feed.Location);
                addFeed(feed);
            }
            StatusTextUpdate();
        }

        void addFeed(Label feed)
        {
            feedList.Add(feed);
            Controls.Add(feed);
        }

        void removeLocation(Label label)
        {
            useLocationList.Remove(label.Location);
        }
        void removeFeed(Label feed)
        {
            removeLocation(feed);
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
        int isGameOver()
        {
            Label lbl = snake.GetSnakeHead();
            if (lbl.Top < gamepan.Top || lbl.Left < gamepan.Left ||
                lbl.Bottom > gamepan.Bottom || lbl.Right > gamepan.Right)
                return 1;
            else if (isSnakeBiteSelf())
                return 2;
            else if (isCrashObstacle())
                return 3;
            else
                return 0;
        }

        bool isCrashObstacle()
        {
            return obstacle.LocationContains(snake.getHeadLocation());     
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
            Point pt = new();
            bool flag = false;

            while (!flag)
            {
                pt = new Point(rand.Next(PIXEL_CNT) * PIXEL_SIZE + FEED_BLANK, rand.Next(PIXEL_CNT) * PIXEL_SIZE + FEED_BLANK);
                
                if (snake.SnakeHeadRect().Contains(pt)) continue;

                if (useLocationList.Contains(new Point(pt.X- FEED_BLANK, pt.Y - FEED_BLANK))) continue;

                flag = true;
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
            removeObstacle();
            
            LV++;
            Jumsu = 0;
            Length = snake.BodyCount();
            timer_Game.Interval = TIMER_INTERVAL - LV * TICK_REDUCE;
            feed_tick = FEED_TICK + LV;
            setObstacle();
            snake.setHeadLocation(CenterPoint);
            timer_Game.Start();
        }

        void ClearUseLocationList()
        {
            useLocationList.Clear();
        }
        void removeObstacle()
        {
            obstacle.Clear();
        }

        void setObstacle()
        {
            int cnt = LV + rand.Next(LV);

            while (cnt > 0)
            {
                Label lbl = obstacle.creteObstacle();
                Point pt = new Point();
                while (true)
                {
                    pt.X = rand.Next(PIXEL_CNT) * PIXEL_SIZE;
                    pt.Y = rand.Next(PIXEL_CNT) * PIXEL_SIZE;
                    if (!useLocationList.Contains(pt) && !NotUsePoint.Contains(pt)) break;
                }
                lbl.Location = pt;
                useLocationList.Add(pt);
                obstacle.Add(lbl);
                cnt--;
            }
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
            return Jumsu >= LVUP_POINT+ LV * 2;
        }
    }
}
