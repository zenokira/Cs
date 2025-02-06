using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace EchoClient
{
    public partial class Form1 : Form
    {
        NetWork netWork;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_MSG.Select();
            netWork = new ();
            netWork.Connect();
        }

        private void textBox_MSG_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_SEND_Click(object sender, EventArgs e)
        {
            netWork.SendMessage(textBox_MSG.Text);
            textBox_MSG.Text = "";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btn_SEND.PerformClick();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 메인 스레드인지 검사 (재귀)
        public void send2MainThread(object o)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(()=> send2MainThread(o)));
                return;
            }
            processData(o);
        }
        
        private void processData(object o)
        {
            listBox_MSGLIST.Items.Add(o);
        }
    }
}
