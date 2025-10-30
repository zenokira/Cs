using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2048
{
    public partial class Input_Form : Form
    {
        private int move_cnt ;
        private string name = "";
        public Input_Form()
        {
            InitializeComponent();
        }

        public Input_Form(int cnt)
        {
            InitializeComponent();
            move_cnt = cnt;
            tb_MoveCnt.Text = move_cnt.ToString();
        }
        private void Input_Form_Load(object sender, EventArgs e)
        {

        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            name = tb_Name.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public UserInfo GetData()
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return new UserInfo(name, move_cnt);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
