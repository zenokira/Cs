using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerCheck
{
    public partial class Class1 : Form
    {
        const int COMPUTER_COUNT = 9;
        const string FILENAME = "1강의실.json";
        string LoadJsonString;

        bool OpenChckFlag = false;


        string[] CommentString = new string[COMPUTER_COUNT]
            {"X","X","X","X","X","X","X","X","X"};
        public Class1()
        {
            InitializeComponent();
        }

        private void Class1_Load(object sender, EventArgs e)
        {
            if (!OpenChckFlag)
            {
                LoadJson();
                InitTextBox();
                OpenChckFlag = !OpenChckFlag;
            } 
        }
        string getTodayNow()
        {
            return DateTime.Now.ToString();
        }
        public void SaveJson()
        {
            using (Stream ws = new FileStream(FILENAME, FileMode.Create))
            {
                string jsonString = $"[{getTodayNow()}]\n";

                for (int i = 0; i < COMPUTER_COUNT; i++)
                {
                    jsonString += $"[컴퓨터{i+1}] {CommentString[i]}\n";
                }

                jsonString += $"\n\n{LoadJsonString}";
                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                ws.Write(jsonBytes, 0, jsonBytes.Length);
                ws.Close();
            }
        }
        void LoadJson()
        {
            try
            {
                using (Stream rs = new FileStream(FILENAME, FileMode.Open))
                {
                    if (rs.Length == 0)
                    {
                        rs.Close();
                        return;
                    }
                    byte[] jsonBytes = new byte[rs.Length];
                    rs.Read(jsonBytes, 0, jsonBytes.Length);
                    LoadJsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                    string[] str = LoadJsonString.Split('\n');

                    for (int i = 0; i < COMPUTER_COUNT; i++)
                    {
                        CommentString[i] = sliceToComment(str[i + 1]);
                    }
                    rs.Close();
                }
            }catch (FileNotFoundException e)
            {
                using (Stream ws = new FileStream("Error.txt", FileMode.Create))
                {
                    string jsonString = $"[{getTodayNow()}] {e}\n";
                    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                    ws.Write(jsonBytes, 0, jsonBytes.Length);
                    ws.Close();
                }
            }
        }

        void InitTextBox()
        {
            textBox1.Text = CommentString[0];
            textBox2.Text = CommentString[1];
            textBox3.Text = CommentString[2];
            textBox4.Text = CommentString[3];
            textBox5.Text = CommentString[4];
            textBox6.Text = CommentString[5];
            textBox7.Text = CommentString[6];
            textBox8.Text = CommentString[7];
            textBox9.Text = CommentString[8];
        }

        string sliceToComment(string str)
        {
            string s = str;

            s = str.Substring(str.IndexOf(']') + 2);

            return s;
        }

        public void CompareComment()
        {
            if (textBox1.Text.Equals(CommentString[0])) CommentString[0] = "X";
            else CommentString[0] = textBox1.Text;

            if (textBox2.Text.Equals(CommentString[1])) CommentString[1] = "X";
            else CommentString[1] = textBox2.Text;

            if (textBox3.Text.Equals(CommentString[2])) CommentString[2] = "X";
            else CommentString[2] = textBox3.Text;

            if (textBox4.Text.Equals(CommentString[3])) CommentString[3] = "X";
            else CommentString[3] = textBox4.Text;

            if (textBox5.Text.Equals(CommentString[4])) CommentString[4] = "X";
            else CommentString[4] = textBox5.Text;

            if (textBox6.Text.Equals(CommentString[5])) CommentString[5] = "X";
            else CommentString[5] = textBox6.Text;

            if (textBox7.Text.Equals(CommentString[6])) CommentString[6] = "X";
            else CommentString[6] = textBox7.Text;

            if (textBox8.Text.Equals(CommentString[7])) CommentString[7] = "X";
            else CommentString[7] = textBox8.Text;

            if (textBox9.Text.Equals(CommentString[8])) CommentString[8] = "X";
            else CommentString[8] = textBox9.Text;
        }
    }
}
