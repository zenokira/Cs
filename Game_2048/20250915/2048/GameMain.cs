using System.Windows.Forms;

namespace _2048
{

    public partial class GameMain : Form
    {
        GamePan gamepan = new();
        Bitmap bitmap_gamepan;
        Graphics graphics;
        bool start_flag = false;
        int move_count = 0;
        int pre_move_count = 0;

        public GameMain()
        {
            InitializeComponent();
            start_flag = false;
        }

        private void GameMain_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Size = new System.Drawing.Size(500, 600);
            bitmap_gamepan = new Bitmap(gamepan.SIZE.Width, gamepan.SIZE.Height);
        }

        private void GameMain_Paint(object sender, PaintEventArgs e)
        {
            if (bitmap_gamepan != null)
            {
                Bitmap boardBmp = gamepan.DrawBoard();
                e.Graphics.DrawImage(boardBmp, gamepan.STARTPOINT);
            }

            if (IsMove()) ShowMoveCount(e.Graphics);

        }

        private void GameMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!start_flag) return;

            gamepan.MoveTile(e, out bool moved);
            if (!moved) return;
            else MoveCountUP();

            if (gamepan.IsGameClear())
            {
                GameClear();
                this.Invalidate();
                return;
            }

            gamepan.ResetNewTiles();
            gamepan.NewTile();

            if (gamepan.IsGameOver())
            {
                this.Invalidate();
                GameOver();
            }
            
            this.Invalidate(); // 마지막에 한 번만 화면 갱신
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                
                    GameMain_KeyDown(this, new KeyEventArgs(keyData));
                    return true; // 폼에서 처리 완료
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

     
        private void btn_start_Click(object sender, EventArgs e)
        {
            GameStart();
        }

        private void GameStart()
        {
            start_flag = true;
            gamepan.NewTile();
            this.Invalidate();
        }


        private void GameClear()
        {
            if (MessageBox.Show("클리어\n다시 하시려면 Yes 를 눌러주세요", "클리어", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gamepan.GameReset();
                gamepan.NewTile();
                GameReset();
                this.Invalidate();
            }
            else
            {
                start_flag = false;
            }
        }

        private void GameOver()
        {
            if (MessageBox.Show("게임 오버\n다시 하시려면 Yes 를 눌러주세요", "게임 오버", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gamepan.GameReset();
                gamepan.NewTile();
                GameReset();
                this.Invalidate();
            }
            else
            {
                start_flag = false;
            }
        }

        private void GameReset()
        {
            move_count = 0;
            pre_move_count = 0;
        }
        private void MoveCountUP()
        {
            move_count++;
        }
        private void MovePreCountUP()
        {
            if (move_count -1 == pre_move_count)    pre_move_count++;
        }
        private bool IsMove()
        {
            return move_count > pre_move_count || move_count == 0;
        }

        private void ShowMoveCount(Graphics g)
        {
            string str = $"움직인 횟수 : {move_count}";
            Font font = new Font("Arial", 16);
            Brush brush = Brushes.Black;
            Point start_point = new Point(this.Size.Width / 3, this.Height / 5 - 30);
            g.DrawString(str, font, brush, start_point);
            MovePreCountUP();
        }
    }
}

/*
Console.WriteLine("Form.Size: " + this.Size);
Console.WriteLine("ClientSize: " + this.ClientSize);

Form.Size: { Width = 500, Height = 600}
ClientSize: { Width = 484, Height = 561}

width gap = 16
height gap = 39

rectangle 400x400
rectangle.left = 42  rectangle.top = 141
*/