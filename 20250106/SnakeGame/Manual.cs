using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnakeGame
{
    public partial class Manual : Form
    {
        public const int FORM_WIDTH = 500;
        public const int FORM_HEIGHT = 500;
 
        public Manual()
        {
            InitializeComponent();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            Size size = this.Size - this.ClientSize;


            this.Width = size.Width + FORM_WIDTH;
            this.Height = size.Height + FORM_HEIGHT;

        }        
    }
}