namespace ComputerCheck
{
    public partial class 강의실현황판 : Form
    {
        Class1 강의실1 = new Class1();
        Class2 강의실2 = new Class2();
        public 강의실현황판()
        {
            InitializeComponent();
        }
        private void 강의실현황판_Load(object sender, EventArgs e)
        {

        }

        public void button_Click(object sender, EventArgs e)
        {
            if (sender == button_CLASS1)
            {
                강의실1.ShowDialog();
            }
            else if (sender == button_CLASS2)
            {
                강의실2.ShowDialog();
            }
        }

        private void 강의실현황판_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAllClasses();
        }

        void SaveAllClasses()
        {
            강의실1.CompareComment();
            강의실1.SaveJson();
            강의실2.CompareComment();
            강의실2.SaveJson();
        }
    }
}
