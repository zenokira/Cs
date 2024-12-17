
namespace SnakeGame
{
    public partial class Form1 : Form
    {
        Snake snake = new Snake();
        int Jumsu = 0;
        int LV = 1;
        int Length = 1;
        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Jumsu = 0;
            LV = 1;
            Length = 1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Controls.Add(snake.GetSnakeHead());

            // 넓이 139 높이 39
            this.Width = 600 -14;
            this.Height = 900;
           // strip_Status.Items.
            MessageBox.Show($"{this.Width} {this.Height}");
            timer_Game.Start();
        }

        string StatusText( int Jumsu, int LV, int Length)
        {
            return $"점수 : {Jumsu}  레벨 : {LV}  몸통길이 : {Length}";
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
            snake.GoSnake();
            cnt++;

            if(cnt % 30 == 0)
            {
                Controls.Add(snake.GrowthSnake());
            }
        }
    }
}
