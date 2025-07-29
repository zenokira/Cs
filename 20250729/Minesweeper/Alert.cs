using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Alert : Form
    {
        Dictionary<string, Point> buttonPositions = new()
        {
            { "Yes",  new Point(99, 67) },
            { "No",   new Point(207, 67) }, 
            { "OK",   new Point(160, 67) } 
        };

        List<Button> buttons = new List<Button>(); 
        public Alert()
        {
            InitializeComponent();
        }

        public Alert(string msg, string title = "알림", string type = "OK" )
        {
            InitializeComponent();

            this.Text = title;
            lb_MSG.Text = msg;

            Initialize(type);
        }
        private void Alert_Load(object sender, EventArgs e)
        {

        }

        public void SetMsgText(string msg)
        {
            lb_MSG.Text = msg;
        }

        private void Initialize(string type)
        {
            if ( type.Equals ("YesOrNo"))
            {
                buttons.Add(CreateButton("Yes"));
                buttons.Add(CreateButton("No"));
            }
            else if ( type.Equals("OK"))
            {
                buttons.Add(CreateButton("OK"));
            }

            foreach (Button button in buttons)
            {
                this.Controls.Add(button);
            }
        }


        private Button CreateButton(string type)
        {
            Button btn = new Button();
            btn.Text = type;
            btn.Size = new Size(80, 25);

            btn.Location = buttonPositions[type];

            btn.Click += type switch
            {
                "Yes" => btn_YES_Click,
                "No" => btn_NO_Click,
                "OK" => btn_OK_Click,
                _ => throw new ArgumentException("잘못된 버튼 타입입니다.", nameof(type))
            };
            return btn;
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

        private void btn_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
