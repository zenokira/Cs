using System.Text.Json;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _2048
{

    public partial class GameMain : Form
    {
        GamePan gamepan = new();
        Bitmap bitmap_gamepan;
        Record record;

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
            record = new();
            LoadListViewFromJson();
        }

        private void GameMain_Paint(object sender, PaintEventArgs e)
        {
            if (bitmap_gamepan != null)
            {
                bitmap_gamepan = gamepan.DrawBoard();
                e.Graphics.DrawImage(bitmap_gamepan, gamepan.STARTPOINT);
            }

            if (IsMove()) ShowMoveCount(e.Graphics);
        }

        private void GameMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!start_flag) return;

            gamepan.MoveTile(e.KeyCode, out bool moved);
            if (!moved) return;

            MoveCountUP();



            gamepan.ResetNewTiles();
            gamepan.NewTile();

            if (IsGameEnd())
            {
                this.Invalidate();


                if (gamepan.IsGameOver())
                {
                    GameOver();
                    return;
                }
                else if (gamepan.IsGameClear())
                {
                    GameClear();
                    return;
                }
            }

            this.Invalidate(); // �������� �� ���� ȭ�� ����
        }

        private bool IsGameEnd()
        {
            return gamepan.IsGameClear() || gamepan.IsGameOver();
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
            //record.ShowDialog();  // ���ڵ� �׽�Ʈ��
            GameStart();
        }

        private void GameStart()
        {
            if (start_flag) return;
            start_flag = true;
            gamepan.NewTile();
            this.Invalidate();
        }


        private void GameClear()
        {
            if (MessageBox.Show("Ŭ����\n�ش� ����� ��ŷ�� ����Ͻðڽ��ϱ�?\n Yes or No", "Ŭ����", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var (name, cnt) = Input_UserData();
                record.AddListViewItem(name, cnt);

            }

            record.ShowDialog();
            gamepan.GameReset();
            gamepan.NewTile();
            GameReset();
            this.Invalidate();

        }



        private (string, string) Input_UserData()
        {
            Input_Form input_Form = new Input_Form(move_count);

            if (input_Form.ShowDialog() == DialogResult.OK)
            {
                return input_Form.GetData();
            }

            return ("", "");
        }
        private void GameOver()
        {
            if (MessageBox.Show("���� ����\n�ٽ� �Ͻ÷��� Yes �� �����ּ���", "���� ����", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            if (move_count - 1 == pre_move_count) pre_move_count++;
        }
        private bool IsMove()
        {
            return move_count > pre_move_count || move_count == 0;
        }

        private void ShowMoveCount(Graphics g)
        {
            string str = $"������ Ƚ�� : {move_count}";
            Font font = new Font("Arial", 16);
            Brush brush = Brushes.Black;
            Point start_point = new Point(this.Size.Width / 3, this.Height / 5 - 30);
            g.DrawString(str, font, brush, start_point);
            MovePreCountUP();
        }
        private void SaveListViewToJson()
        {
            // ���� �ַ�� ���� ����� (������Ʈ ��Ʈ)
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..", "record.json"));
            System.Windows.Forms.ListView listView = record.GetListView();

            // ListView �� Dictionary ����Ʈ�� ��ȯ
            var data = new List<Dictionary<string, object>>();

            foreach (ListViewItem item in listView.Items)
            {
                data.Add(new Dictionary<string, object>
                {
                    ["�̸�"] = item.SubItems[1].Text,
                    ["������Ƚ��"] = item.SubItems[2].Text
                });
            }

            // JSON���� ����
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        private void LoadListViewFromJson()
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..", "record.json"));
            System.Windows.Forms.ListView listView = record.GetListView();

            if (!File.Exists(path))
                return;

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

                listView.Items.Clear();

                if (data == null) return;

                // ������Ƚ�� �������� ����
                var ordered = data.OrderBy(d => int.Parse(d["������Ƚ��"].ToString() ?? "0"))
                                  .ThenBy(d => d["�̸�"].ToString());

                int rank = 1;
                foreach (var d in ordered)
                {
                    var name = d["�̸�"].ToString();
                    var moves = d["������Ƚ��"].ToString();

                    var item = new ListViewItem(rank.ToString())
                    {
                        UseItemStyleForSubItems = false
                    };

                    var nameSub = new ListViewItem.ListViewSubItem(item, name) { };
                    var movesSub = new ListViewItem.ListViewSubItem(item, moves) { };

                    item.SubItems.Add(nameSub);
                    item.SubItems.Add(movesSub);
                    listView.Items.Add(item);

                    rank++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�ҷ����� ����: {ex.Message}");
            }
        }

        private void GameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveListViewToJson();
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