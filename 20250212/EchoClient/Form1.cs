using Microsoft.VisualBasic.Devices;
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
            netWork = new();
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

        // 메인 스레드인지 검사 (재귀)
        public void send2MainThread(object o)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => send2MainThread(o)));
                return;
            }
            processData(o);
        }

        private void processData(object o)
        {
            string data = (string)o;


            if (data.StartsWith("연결성공"))
            {
               
                data = data.Replace("연결성공", "").Trim(' ');
                listBox_MsgList.Items.Add("연결성공");
                netWork.SetSockNo(Convert.ToInt32(data));
                return;
            }

            string command = "";
            string dst = "";
            string msg = "";

            if (!ParsingData(data, out command, out dst, out msg)) return;

            if (isMakeCommand(command))
            {
                MakeButton(msg);
            }
            else
                listBox_MsgList.Items.Add(msg);
           
        }


        bool ParsingData(string input, out string command, out string dst, out string msg)
        {
            string[] splitString = input.Split(" ");
            dst = ""; msg = "";
            command = splitString[0];

            if (splitString.Length < 2) return false;

            dst = splitString[1];

            if (splitString.Length < 3) return false;
            
            msg = ParsingMessage(input, splitString[2]);
           
            return true;
        }

        string ParsingMessage(string data, string start)
        {
            if (start.Equals("")) return "";
            string splitString = data.Substring(data.IndexOf(start));
            return splitString;
        }

        bool isMakeCommand(string command)
        {
            return command.Equals("make");
        }
        bool isChatCommand(string command)
        {
            return command.Equals("chat");
        }

        private void textBox_MSG_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_SEND.PerformClick();
            }
        }

        void MakeButton(string data)
        {
            Button button = new();
            button.Text = data.Substring(data.IndexOf(" ")+1);
            Controls.Add(button);
        }
    }
}
