namespace ComputerCheck
{
    public partial class ���ǽ���Ȳ�� : Form
    {
        Class1 ���ǽ�1 = new Class1();
        Class2 ���ǽ�2 = new Class2();
        Class3 ���ǽ�3 = new Class3();
        Class4 ���ǽ�4 = new Class4();
        Class6 ���ǽ�6 = new Class6();
        Class7 ���ǽ�7 = new Class7();
        Class8 ���ǽ�8 = new Class8();
        Class9 ���ǽ�9 = new Class9();

        public ���ǽ���Ȳ��()
        {
            InitializeComponent();
        }
        private void ���ǽ���Ȳ��_Load(object sender, EventArgs e)
        {

        }

        public void button_Click(object sender, EventArgs e)
        {
            if (sender == button_CLASS1)
            {
                ���ǽ�1.ShowDialog();
            }
            else if (sender == button_CLASS2)
            {
                ���ǽ�2.ShowDialog();
            }
            else if (sender == button_CLASS3)
            {
                ���ǽ�3.ShowDialog();
            }
            else if (sender == button_CLASS4)
            {
                ���ǽ�4.ShowDialog();
            }
            else if (sender == button_CLASS6)
            {
                ���ǽ�6.ShowDialog();
            }
            else if (sender == button_CLASS7)
            {
                ���ǽ�7.ShowDialog();
            }
            else if (sender == button_CLASS8)
            {
                ���ǽ�8.ShowDialog();
            }
            else if (sender == button_CLASS9)
            {
                ���ǽ�9.ShowDialog();
            }
        }

        private void ���ǽ���Ȳ��_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAllClasses();
        }

        void SaveAllClasses()
        {
            ���ǽ�1.CompareComment();
            ���ǽ�1.SaveJson();
            ���ǽ�2.CompareComment();
            ���ǽ�2.SaveJson();
            ���ǽ�3.CompareComment();
            ���ǽ�3.SaveJson();
            ���ǽ�4.CompareComment();
            ���ǽ�4.SaveJson();
            ���ǽ�6.CompareComment();
            ���ǽ�6.SaveJson();
            ���ǽ�7.CompareComment();
            ���ǽ�7.SaveJson();
            ���ǽ�8.CompareComment();
            ���ǽ�8.SaveJson();
            ���ǽ�9.CompareComment();
            ���ǽ�9.SaveJson();
        }

        private void button_CLASS3_Click(object sender, EventArgs e)
        {

        }
    }
}