using System.Windows.Forms;

namespace _2048
{

    public partial class GameMain : Form
    {
        GamePan gamepan = new();
        Bitmap bitmap_gamepan;
        Graphics graphics;

       
        public GameMain()
        {
            InitializeComponent();
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
            bool moved = false;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    gamepan.MoveTilesLeft();
                    moved = true;
                    break;
                case Keys.Right:
                    gamepan.MoveTilesRight();
                    moved = true;
                    break;
                case Keys.Up:
                    gamepan.MoveTilesUp();
                    moved = true;
                    break;
                case Keys.Down:
                    gamepan.MoveTilesDown();
                    moved = true;
                    break;
            }

            if (!moved) return; // �̵� �� �� ��� ó�� ����

            // 2. �̵� ���� Ŭ���� üũ
            if (gamepan.IsGameClear())
            {
                this.Invalidate();
                GameClear();
                return; // Ŭ���� �� �� �̻� ���� X
            }

            // 3. �̵� �� �� Ÿ�� ����
            gamepan.NewTile();

            // 4. ȭ�� ����
            this.Invalidate();

            // 5. �� Ÿ�� �߰� �� ���� ���� üũ
            if (gamepan.IsGameOver())
            {
                GameOver();
            }
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
            gamepan.NewTile();
            this.Invalidate();
        }


        private void GameClear()
        {

        }

        private void GameOver()
        {

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