using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Alert : Form
    {
        public Alert()
        {
            InitializeComponent();
        }

        public Alert(string msg, string title)
        {
            InitializeComponent();

            this.Text = title;
            lb_MSG.Text = msg;
        }
        private void Alert_Load(object sender, EventArgs e)
        {

        }

        private void btn_YES_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btn_NO_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            this.Close();
        }

        public void ForceClose(DialogResult result = DialogResult.None)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ForceClose(result)));
            }
            else
            {
                this.Close();
            }
        }
    }
}
