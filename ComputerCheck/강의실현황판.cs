namespace ComputerCheck
{
    public partial class ���ǽ���Ȳ�� : Form
    {
        Class1 ���ǽ�1 = new Class1();
        Class2 ���ǽ�2 = new Class2();
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
        }
    }
}
