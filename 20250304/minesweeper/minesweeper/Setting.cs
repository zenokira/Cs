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
    public partial class Setting : Form
    {
        int Level;
        public Setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Level = 0;
            exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Level = 1;
            exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Level = 2;
            exit();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            Level = -1;
        }

        public int GetLevel()
        {
            return Level;
        }

        private void exit()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}