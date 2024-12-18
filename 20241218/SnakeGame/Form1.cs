
using System.Diagnostics;
using System.Windows.Forms;

namespace SnakeGame
{
   
    public partial class Form1 : Form
    {
        const int PIXEL_CNT = 30;


        Random rand = new Random();
        Snake snake;
        List<Label> feedList = new List<Label>();
        int Jumsu = 0;
        int LV = 1;
        int Length = 1;

  
        Size pixel = new Size(30, 30);
        Rectangle gamepan = new();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Initialize()
        {
            Jumsu = 0;
            LV = 1;
            Length = 1;
            feedList.Clear();
        }

        void test(string s)
        {
            MessageBox.Show(s);
        }
        string teststring(int n1, int n2)
        {
            return $"{n1} {n2}";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            this.Size += pixel * PIXEL_CNT - this.ClientSize;

           test(teststring(Size.Width,Size.Height));
            snake = new Snake(Controls);
            snake.GetSnakeHead();
            

            // 넓이 16 높이 39
            StatusTextUpdate();
            timer_Game.Start();
        }


        void StatusTextUpdate( )
        {
            toolStripStatusLabel_LV.Text = "LV : " + LV.ToString();
            toolStripStatusLabel_Jumsu.Text = "점수 : " + Jumsu.ToString();
            toolStripStatusLabel_Bodycnt.Text = "몸통길이 : " + Length.ToString();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                snake.Direction(SnakeVector.LEFT);
                return true;
            }
            else if(keyData == Keys.Right)
            {
                snake.Direction(SnakeVector.RIGHT);
                return true;
            }
            else if (keyData == Keys.Up)
            {
                snake.Direction(SnakeVector.UP);
                return true;
            }
            else if (keyData == Keys.Down)
            {
                snake.Direction(SnakeVector.DOWN);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        int cnt = 0;
        private void timer_Game_Tick(object sender, EventArgs e)
        {
            snake.Go();
            cnt++;
            if(isGameOver())
            {
                timer_Game.Stop();
                MessageBox.Show("벽에 뱀이 머리를 박아 죽었습니다.");
            }
            if (isSnakeFeeding())
            {
                snake.Growth();
                Length++;
                Jumsu++;
            }
            if (cnt % 20 == 0)
            {
                Label feed = CreateFeed();
 
                feedList.Add(feed);
                Controls.Add(feed);
            }
            StatusTextUpdate();
        }

        bool isSnakeFeeding()
        {
            foreach (var feed in feedList)
            {
                if (snake.GetSnakeHead().Location.Equals(feed.Location))
                {
                    feedList.Remove(feed);
                    Controls.Remove(feed);
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
            else
                return false;
        }

        

        Label CreateFeed()
        {
            Label lbl = new();
            lbl.Name = "feed";
            lbl.BackColor = Color.IndianRed;
            lbl.Size = new Size(30, 30);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = RandomFeedLocation();
            return lbl;
        }

        Point RandomFeedLocation()
        {
            int x = gamepan.Width / 30;
            int y = gamepan.Height / 30;

            Point pt = new(); ;
            bool flag = false;
            
            while (!flag)
            {
                pt = new Point(rand.Next(x) * 30, rand.Next(y) * 30);

                if (snake.GetSnakeHead().Location == pt) continue;
                
                flag = true;
                
                foreach (var item in feedList)
                {
                    if (item.Location == pt) flag = false;
                }

                foreach (var item in snake.GetSnakeBody())
                {
                    if (item.Location == pt) flag = false;
                }
            }

            return pt;
        }
    }
}
