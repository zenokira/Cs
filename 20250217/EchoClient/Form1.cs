using Accessibility;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        readonly string[] commandList = { "exit", "make", "number", "remove", "error" };

        NetWork netWork;

        List<object> list = new();

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
            if (netWork.isConnect())
            {
                netWork.SendMessage(textBox_MSG.Text);
                textBox_MSG.Text = "";
            }
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

            string command = "";
            string dst = "";
            string msg = "";

            if (!ParsingData(data, out command, out dst, out msg)) ;
           
            UsingCommand(command, dst, msg);
        }

        void UsingCommand(string command, string dst, string msg)
        {
            switch (WhatCommand(command))
            {
                case 0:
                    netWork.disConnect();
                    listBox_MsgList.Items.Add("연결이 종료되었습니다");
                    break;
                case 1:
                    MakeButton(msg);
                    break;
                case 2:
                    label1.Text = dst;
                    netWork.SetSockNo(Convert.ToInt32(dst));
                    break;
                case 3:
                    Button btn = FindButtonInList(msg);
                    if (btn == null) return;
                    RemoveButton(btn);
                    break;
                default:
                    listBox_MsgList.Items.Add(msg);
                    break;
            }
        }


      
        Button? FindButtonInList(string msg)
        {
            foreach (var item in list)
            {
                Button btn = (Button)item;
                if (btn.Text.Equals(msg))
                {
                    return btn;
                }
            }
            return null;
        }
        void RemoveButton(Button btn)
        {
            Controls.Remove(btn);
            list.Remove(btn);
        }

        bool ParsingData(string input, out string command, out string dst, out string msg)
        {
            string[] splitString = input.Split(" ");
            dst = ""; msg = "";
            command = splitString[0];

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

        int WhatCommand(string cmd)
        {
            if (!commandList.Contains(cmd)) return - 1;

            for (int i = 0; i < commandList.Length; i++)
            {
                if (commandList[i].Equals(cmd)) return i;
            }
            return -1;
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
            button.Size = new Size(80, 30);
            int ptX = (list.Count * button.Width);
            int width = this.Width / 100 * 100;
            button.Location = new Point(ptX % width, ptX / width * 30);
          
            list.Add(button);

            Controls.Add(button);
        }
    }

}
