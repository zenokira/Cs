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
            this.Size = new System.Drawing.Size(500, 600);
            bitmap_gamepan = new Bitmap(gamepan.SIZE.Width, gamepan.SIZE.Height);
            graphics = Graphics.FromImage(bitmap_gamepan);


            graphics.FillRectangle(new SolidBrush(Color.Ivory), gamepan.RECT);
            for (int i = 0; i < 5; i++)
            {
                graphics.FillRectangle(new SolidBrush(Color.BlueViolet), 0, gamepan.row[i], gamepan.row_Rect.Width, gamepan.row_Rect.Height);
                graphics.FillRectangle(new SolidBrush(Color.BlueViolet),  gamepan.col[i], 0, gamepan.col_Rect.Width, gamepan.col_Rect.Height);
            }
        }

        private void GameMain_Paint(object sender, PaintEventArgs e)
        {
            if (bitmap_gamepan != null)
            {
                e.Graphics.DrawImage(bitmap_gamepan, gamepan.STARTPOINT);
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