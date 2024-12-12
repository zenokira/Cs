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
            //timer1.Start();

            timer2.Start();
        }

        public void Restart()
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
            lb_WordList.Items.Clear();

            toolStripProgressBar1.Value = 5;

            for (int i = listFruit.Count-1; i >= 0; i-- )
            {
                removelabel(listFruit[i]);
            }
            
           
            timer2.Interval = 1000;
            timer2.Start();
        }

        public void LevelUP()
        {
            cntCorrect = 0;
            cntWrong = 0;
            rate = 0;
            LV++;
            tb_Correct.Text = cntCorrect.ToString();
            tb_Wrong.Text = cntWrong.ToString();
            pg_CorrectRate.Value = ((int)rate);
            lbl_CorrectRate.Text = rate.ToString("n2");
            lb_WordList.Items.Clear();
            timer1.Interval = 1000 - LV * 100;
            timer1.Start();
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
            makelabel(makeNewPoint(), fruits_name[rand.Next(fruits_name.Length)]);
            foreach (var item in listFruit)
            {
                item.Top += 20;
            }

            if (isWordInSea())
            {
                toolStripProgressBar1.Value--;
            }

            if(isGameOver())
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
        }

        private void tb_RainTyping_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (listFruit.Count <= 0) return;

            TextBox tb = (TextBox)sender;

            removelabel(tb.Text);

            tb.Text = "";

        }
        List<Label> listFruit = new ();

        Point makeNewPoint()
        {
            int min = lbl_Sea.Left;
            int max = lbl_Sea.Right;
            Point pt = new Point(rand.Next(min,max), rand.Next(20,50));
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

        void removelabel(string fruit)
        {
            foreach (var item in listFruit)
            {
                if ( item.Text.Equals(fruit) )
                {
                    removelabel(item);
                    return;
                }
            }
        }

        void removelabel(Label lbl)
        {
            Controls.Remove(lbl);
            listFruit.Remove(lbl);
        }

        bool isWordInSea()
        {
            foreach (var item in listFruit)
            {
                if(item.Top >= lbl_Sea.Top)
                {
                    removelabel(item);
                    return true;
                }
            }
            return false;
        }

        bool isGameOver()
        {
            if (toolStripProgressBar1.Value == 0)
                return true;
            else
                return false;
        }
    }
}
