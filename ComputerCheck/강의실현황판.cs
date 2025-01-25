namespace ComputerCheck
{
    public partial class 강의실현황판 : Form
    {
        Class1 강의실1 = new Class1();
        Class2 강의실2 = new Class2();
        Class3 강의실3 = new Class3();
        Class4 강의실4 = new Class4();
        Class6 강의실6 = new Class6();
        Class7 강의실7 = new Class7();
        Class8 강의실8 = new Class8();
        Class9 강의실9 = new Class9();

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
            else if (sender == button_CLASS3)
            {
                강의실3.ShowDialog();
            }
            else if (sender == button_CLASS4)
            {
                강의실4.ShowDialog();
            }
            else if (sender == button_CLASS6)
            {
                강의실6.ShowDialog();
            }
            else if (sender == button_CLASS7)
            {
                강의실7.ShowDialog();
            }
            else if (sender == button_CLASS8)
            {
                강의실8.ShowDialog();
            }
            else if (sender == button_CLASS9)
            {
                강의실9.ShowDialog();
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
            강의실3.CompareComment();
            강의실3.SaveJson();
            강의실4.CompareComment();
            강의실4.SaveJson();
            강의실6.CompareComment();
            강의실6.SaveJson();
            강의실7.CompareComment();
            강의실7.SaveJson();
            강의실8.CompareComment();
            강의실8.SaveJson();
            강의실9.CompareComment();
            강의실9.SaveJson();
        }

        private void button_CLASS3_Click(object sender, EventArgs e)
        {

        }
    }
}