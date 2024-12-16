using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Typing_Program
{
    public partial class Form1 : Form
    {
        int cntCorrect = 0;
        int cntWrong = 0;
        int LV = 0;
        double rate = 0;
        int jumsu = 0;

        string[] fruits_name = { "apple", "banana", "cherry", "mango", "peach", "avocado", "lime", "lemon", "kiwi", "tomato", "coconut" }; 
        public Form1()
        {
            InitializeComponent();
        }

        Random rand = new Random();
        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize(); 
        }

        public void Initialize()
        {
            cntCorrect = 0;
            cntWrong = 0;
            rate = 0;
            LV = 0;

            tb_Correct.Text = cntCorrect.ToString();
            tb_Wrong.Text = cntWrong.ToString();
            pg_CorrectRate.Value = ((int)rate);
            lbl_CorrectRate.Text = rate.ToString("n2");
            lb_WordList.Items.Clear();
            timer1.Interval = 1000;
            toolStripProgressBar1.Value = 5;
            lbl_Jumsu.Text = jumsu.ToString();
            //timer1.Start();
            lbl_LV.Text = LV.ToString();
            timer2.Start();
        }

        public void Restart()
        {
            cntCorrect = 0;
            cntWrong = 0;
            rate = 0;
            LV = 0;
            jumsu = 0;

            lbl_LV.Text = LV.ToString();
            tb_Correct.Text = cntCorrect.ToString();
            tb_Wrong.Text = cntWrong.ToString();
            pg_CorrectRate.Value = ((int)rate);
            lbl_CorrectRate.Text = rate.ToString("n2");
            lbl_Jumsu.Text = jumsu.ToString();
            lb_WordList.Items.Clear();

           
            timer1.Interval = 1000;
            lb_WordList.Items.Clear();

            toolStripProgressBar1.Value = 5;

            for (int i = listFruit.Count-1; i >= 0; i-- )
            {
                removelabel(listFruit[i]);
            }
            RemoveList.Clear();

            timer2.Interval = 1000;
            timer2.Start();
        }

        public void LevelUP()
        {
            MessageBox.Show("LVUP");
            cntCorrect = 0;
            cntWrong = 0;
            rate = 0;
            jumsu = 0;
            LV++;
            toolStripProgressBar1.Value = 5;

            lbl_LV.Text =   LV.ToString();
            tb_Correct.Text = cntCorrect.ToString();
            tb_Wrong.Text = cntWrong.ToString();
            pg_CorrectRate.Value = ((int)rate);
            lbl_CorrectRate.Text = rate.ToString("n2");
            lbl_Jumsu.Text= jumsu.ToString();
            lb_WordList.Items.Clear();
            
            for (int i = listFruit.Count - 1; i >= 0; i--)
            {
                removelabel(listFruit[i]);
            }
            RemoveList.Clear();
           // timer1.Interval = 1000 - LV * 100;
            timer2.Interval = 1000 - LV * 50;
           // timer1.Start();
            timer2.Start();
        }

        private void tb_TypingText_KeyDown(object sender, KeyEventArgs e)
        {
            char key = (char)e.KeyCode;
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Enter) return;

            if (lb_WordList.Items.Contains(key))
            {
                lb_WordList.Items.Remove(key);
                cntCorrect++;
                tb_Correct.Text = cntCorrect.ToString();
            }
            else
            {
                cntWrong++;
                tb_Wrong.Text = cntWrong.ToString();
            }

            rate = (double)cntCorrect / ((double)(cntWrong + cntCorrect)) * 100;
            pg_CorrectRate.Value = (int)rate;
            lbl_CorrectRate.Text = rate.ToString("n2");

            if (isClear())
            {
                timer1.Stop();
                MessageBox.Show("클리어");
                LevelUP();
            }
            else if (isFail())
            {
                timer1.Stop();
                if (MessageBox.Show("실패했습니다\n다시 시작하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Restart();
                }
                else
                {
                    Application.Exit();
                }
            }
            tb_TypingText.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            char word = (char)('A' + rand.Next(26));
            lb_WordList.Items.Add(word);
        }

        private bool isClear()
        {
            if (cntCorrect >= 50 && rate >= 90)
                return true;
            else
                return false;
        }

        private bool isFail()
        {
            if (cntWrong >= 30 && rate <= 85)
                return true;
            else
                return false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
           
            removeSeaWord();

            makelabel(makeNewPoint(), fruits_name[rand.Next(fruits_name.Length)]);
            
            foreach (var item in listFruit)
            {
                item.Top += 20;
            }

            if (isWordInSea())
            {
                toolStripProgressBar1.Value -= RemoveList.Count;
            }

            if (isGameOver())
            {
                GameOver();
            }
        }

        void GameOver()
        {
            timer2.Stop();
            if (MessageBox.Show("실패했습니다\n다시 시작하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Restart();
            }
            else
            {
                Application.Exit();
            }
        }
        private void tb_RainTyping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (listFruit.Count <= 0) return;

            TextBox tb = (TextBox)sender;

            Label lbl = isSearchWord(tb.Text);

            if (lbl == null)
            {
                incorrectWord();
                
                if (isGameOver())
                {
                    GameOver();
                }
            }
            else correctWord(lbl);
           
            tb.Text = "";
            if (jumsu >= 10)
            {
                timer2.Stop();
                LevelUP();
            } 
        }
        List<Label> listFruit = new ();

        Point makeNewPoint()
        {
            int min = lbl_Sea.Left;
            int max = lbl_Sea.Right-40;
            Point pt = new Point(rand.Next(min,max), lbl_Sea.Top-310);
            return pt;
        }
        void makelabel(Point pt, string fruit)
        {
            Label lbl_RainWord = new();
            lbl_RainWord.AutoSize = true;
            lbl_RainWord.Location = pt;
            lbl_RainWord.Name = "lbl_RainWord";
            lbl_RainWord.Size = new Size(36, 15);
            lbl_RainWord.TabIndex = 5;
            lbl_RainWord.Text = fruit;

            Controls.Add(lbl_RainWord);
            listFruit.Add(lbl_RainWord);
        }

        void correctWord(Label lbl)
        {
            removelabel(lbl);
            jumsu++;
            lbl_Jumsu.Text = jumsu.ToString();
        }

        void incorrectWord()
        {
            toolStripProgressBar1.Value--;
        }

        Label isSearchWord(string fruit)
        {
            foreach (var item in listFruit)
            {
                if (item.Text.Equals(fruit))
                {
                    return item;
                }
            }
            return null;
        }

        void removelabel(Label lbl)
        {
            Controls.Remove(lbl);
            listFruit.Remove(lbl);
        }

        List<Label> RemoveList = new();

        bool isWordInSea()
        {
            foreach (var item in listFruit)
            {
                if(item.Bottom >= lbl_Sea.Top)
                {
                    RemoveList.Add(item);
                }
            }
            if(RemoveList.Count > 0) { return true; }
            return false;
        }

        bool isGameOver()
        {
            if (toolStripProgressBar1.Value <= 0)
                return true;
            else
                return false;
        }

        void removeSeaWord()
        {
            if (RemoveList.Count > 0)
            {
                foreach (var item in RemoveList)
                {
                    removelabel(item);
                }
                RemoveList.Clear();
            }
        }
    }
}
