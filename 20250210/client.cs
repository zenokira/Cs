using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace EchoClient
{
    public partial class Form1 : Form
    {
        const int COMMAND_START_IDX = 0;
        const int COMMAND_LENGTH = 10;

        const int BUFSIZE = 513;

        const int SOCKET_NO_START_IDX = COMMAND_START_IDX + COMMAND_LENGTH + 1;
        const int SOCKET_NO_LENGTH = 2;

        const int DATA_START_IDX = SOCKET_NO_START_IDX + SOCKET_NO_LENGTH + 1;
        const int DATA_LENGTH = BUFSIZE - COMMAND_LENGTH - SOCKET_NO_LENGTH;

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
            byte[] data = SetData(textBox_MSG.Text);
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

        byte[] SetData(string _data)
        {
            byte[] data = new byte[BUFSIZE];
            byte[] command = new byte[COMMAND_LENGTH];
            byte[] socketNo = new byte[SOCKET_NO_LENGTH];
            byte[] cmdByte = Encoding.UTF8.GetBytes("comment");
            byte[] socketByte = Encoding.UTF8.GetBytes("1");
            

            Array.Copy(cmdByte, command, cmdByte.Length);
            Array.Copy(socketByte, socketNo, socketByte.Length);

       


            Array.Copy(command, data, COMMAND_LENGTH);
            Array.Copy(socketNo, data, SOCKET_NO_LENGTH);
            Array.Copy(_data.ToArray(), data, DATA_LENGTH-1);
           
            return data;
        }
    }
}
