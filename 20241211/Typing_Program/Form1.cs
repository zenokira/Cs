using System.Windows.Forms.VisualStyles;

namespace Typing_Program
{
    public partial class Form1 : Form
    {
        int cntCorrect = 0;
        int cntWrong = 0;

        double rate = 0;
        public Form1()
        {
            InitializeComponent();
        }

        Random rand = new Random();
        private void Form1_Load(object sender, EventArgs e)
        {
            lb_WordList.Items.Clear();

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

            rate = (double)cntCorrect / ((double)(cntWrong+cntCorrect)) * 100;
            pg_CorrectRate.Value = (int)rate;
            lbl_CorrectRate.Text = rate.ToString() + "%";
            tb_TypingText.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            char word = (char)('A' + rand.Next(26));
            lb_WordList.Items.Add(word);
        }
    }
}
