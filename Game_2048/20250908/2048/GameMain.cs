using System.Windows.Forms;

namespace _2048
{

    public partial class GameMain : Form
    {
        GamePan gamepan = new();
        Bitmap bitmap_gamepan;
        Graphics graphics;
        bool start_flag = false;

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
        }

        private void GameMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!start_flag) return;

            gamepan.MoveTile(e, out bool moved);
            if (!moved) return;

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
                GameOver();
            }

            this.Invalidate(); // �������� �� ���� ȭ�� ����
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
                    return true; // ������ ó�� �Ϸ�
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
            if (MessageBox.Show("Ŭ����\n�ٽ� �Ͻ÷��� Yes �� �����ּ���", "Ŭ����", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gamepan.GameReset();
                gamepan.NewTile();
                this.Invalidate();
            }
        }

        private void GameOver()
        {
            if (MessageBox.Show("���� ����\n�ٽ� �Ͻ÷��� Yes �� �����ּ���", "���� ����", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                gamepan.GameReset();
                gamepan.NewTile();
                this.Invalidate();
            }
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