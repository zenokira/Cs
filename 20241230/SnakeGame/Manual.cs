using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Manual : Form
    {
        public const int FORM_WIDTH = 500;
        public const int FORM_HEIGHT = 500;
        public const int FORM_X_CENTER = FORM_WIDTH / 2;
        public const int FORM_Y_CENTER = FORM_HEIGHT / 2;

        const int GAP_SIZE = 50;
        const int ARROW_KEY_LABEL_SIZE = 70;
        const int TEXT_SIZE = 400;
        public Manual()
        {
            InitializeComponent();
        }

        private void Manual_Load(object sender, EventArgs e)
        {
            Size size = this.Size - this.ClientSize;


            this.Width = size.Width + FORM_WIDTH;
            this.Height = size.Height + FORM_HEIGHT;

            Label left = createArrowKeyLabel(new Point (FORM_X_CENTER - ARROW_KEY_LABEL_SIZE - GAP_SIZE, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE / 2),0);
            Label right = createArrowKeyLabel(new Point(FORM_X_CENTER + GAP_SIZE, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE / 2), 1);
            Label up = createArrowKeyLabel(new Point(FORM_X_CENTER - ARROW_KEY_LABEL_SIZE/2, FORM_Y_CENTER - ARROW_KEY_LABEL_SIZE  - GAP_SIZE), 2);
            Label down = createArrowKeyLabel(new Point(FORM_X_CENTER - ARROW_KEY_LABEL_SIZE / 2, FORM_Y_CENTER + GAP_SIZE), 3);
            Label text = createTextLabel();



            Controls.Add(left); 
            Controls.Add(right);
            Controls.Add(up);
            Controls.Add(down);
            Controls.Add(text);
        }

        Label createArrowKeyLabel(Point location , int n)
        {
            Font font = new Font(Font.Name, 40);
            Label lbl = new();
            lbl.Font = font;
            lbl.Name = "ArrowKey";
            lbl.BackColor = Color.Green;
            lbl.Size = new Size(ARROW_KEY_LABEL_SIZE, ARROW_KEY_LABEL_SIZE);
            lbl.TabIndex = 0;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Location = location;

            if (n == 0) lbl.Text = "◁";
            else if (n == 1) lbl.Text = "▷";
            else if (n == 2) lbl.Text = "△";
            else if (n == 3) lbl.Text = "▽";

            return lbl;
        }

        Label createTextLabel()
        {
            Font font = new Font(Font.Name, 15);
            Label lbl = new();
            lbl.Font = font;
            lbl.Name = "Manual";
            lbl.TabIndex = 0;
            lbl.Size = new Size(TEXT_SIZE, font.Height * 2  + 10);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Location = new Point ( 50, FORM_Y_CENTER + FORM_Y_CENTER / 2);
            lbl.Text = "방향키를 이용하여 \n뱀의 머리를 조종 할 수 있습니다";
            return lbl;
        }
    }
}
