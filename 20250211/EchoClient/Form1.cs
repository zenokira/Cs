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

        const int BUFSIZE = 512;

        const int SOCKET_NO_START_IDX = COMMAND_START_IDX + COMMAND_LENGTH;
        const int SOCKET_NO_LENGTH = 2;

        const int DATA_START_IDX = SOCKET_NO_START_IDX + SOCKET_NO_LENGTH;
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
            ;
            if (isChatCommand(ParsingData((string)o, 0)))
            {
                listBox_MSGLIST.Items.Add(ParsingData((string)o, 2));
            }
            else if (isMakeCommand(ParsingData((string)o, 0)))
            {

            }
        }


        string ParsingData(string data, int n)
        {
            string[] splitString = data.Split(" ");

            string str = n switch
            {
                0 => ParsingCommand(splitString),
                1 => ParsingSockNo(splitString),
                2 => ParsingMessage(splitString)
            };

            return str;
        }

        string ParsingCommand(string[] data)
        {
            return data[0];
        }

        string ParsingSockNo(string[] data)
        {
            return data[1];
        }

        string ParsingMessage(string[] data)
        {
            string str = "";
            for (int i = 2; i < data.Length; i++)
            {
                str += data[i];
            }
            return str;
        }

        bool isMakeCommand(string command)
        {
            return command.Equals("make");
        }
        bool isChatCommand(string command)
        {
            return command.Equals("chat");
        }
    }
}
