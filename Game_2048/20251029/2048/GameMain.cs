﻿using System.Text.Json;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.ListView;

namespace _2048
{
    public partial class GameMain : Form
    {
        GamePan gamepan = new();
        Bitmap bitmap_gamepan;

        bool start_flag = false;
        int move_count = 0;
        int pre_move_count = 0;

        private UserStore userStore = new();

        public GameMain()
        {
            InitializeComponent();
            start_flag = false;
            timer_save.Interval = 1000;
        }

        private void GameMain_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Size = new System.Drawing.Size(500, 600);
            bitmap_gamepan = new Bitmap(gamepan.SIZE.Width, gamepan.SIZE.Height);
            LoadUserInfoFromJson();
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

            this.Invalidate(); // 마지막에 한 번만 화면 갱신
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
            if (start_flag) return;
            start_flag = true;
            gamepan.NewTile();
            this.Invalidate();
        }


        private void GameClear()
        {
            if (MessageBox.Show("클리어\n해당 기록을 랭킹에 등록하시겠습니까?\n Yes or No", "클리어", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var user = Input_UserData();

                if (user != null)
                {
                    userStore.Add(user);

                    ShowRecord();
                }
            }
            SaveUserInfoToJson();

            gamepan.GameReset();
            gamepan.NewTile();
            GameReset();
            this.Invalidate();
        }



        private UserInfo? Input_UserData()
        {
            Input_Form input_Form = new Input_Form(move_count);

            if (input_Form.ShowDialog() == DialogResult.OK)
            {
                return input_Form.GetData();
            }

            return null;
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
                gamepan.GameReset();
                GameReset();
                this.Invalidate();
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
            string str = $"움직인 횟수 : {move_count}";
            Font font = new Font("Arial", 16);
            Brush brush = Brushes.Black;
            Point start_point = new Point(this.Size.Width / 3, this.Height / 5 - 30);
            g.DrawString(str, font, brush, start_point);
            MovePreCountUP();
        }
        private void SaveUserInfoToJson()
        {
            // 현재 솔루션 기준 상대경로 (프로젝트 루트)
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..", "record.json"));

            var data = new List<Dictionary<string, object>>();

            foreach (var item in userStore)
            {
                data.Add(new Dictionary<string, object>
                {
                    ["이름"] = item.Name,
                    ["움직인횟수"] = item.Moves,
                });
            }

            // JSON으로 저장
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        private void LoadUserInfoFromJson() // json 읽어서 userStore 에 저장
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..", "record.json"));

            if (!File.Exists(path))
                return;

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
                if (data == null) return;

                userStore.Clear();

                foreach (var d in data)
                {
                    string name = d["이름"].ToString() ?? "";
                    string moves = d["움직인횟수"].ToString() ?? "";

                    var user = new UserInfo(name, moves.ToString());
                    userStore.Add(user); // UserStore에 추가
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"불러오기 실패: {ex.Message}");
            }
        }

        private bool _isClosing = false;
        private void GameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosing = true;
            try
            {
                SaveUserInfoToJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Closing Save Error : {ex}");
            }
        }

        private int lastSavedCount = 0;   // 마지막 저장 시점의 아이템 개수
        private int tickCounter = 0;
        private void timer_save_Tick(object sender, EventArgs e)
        {
            if (_isClosing) return;
            if (++tickCounter < 30) return; // 30초마다 체크 (Timer.Interval=1000)
            tickCounter = 0;

            try
            {
                SaveUserInfoToJson();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auto-save error : {ex}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowRecord();
        }


        private void ShowRecord()
        {
            var recordForm = new Record(userStore);
            recordForm.Show(this);  // or ShowDialog()
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