using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ConsoleApp6
{
    class MainApp: Form
    {
        Random rand;

        public MainApp(string title)
        {
            rand = new Random();
            this.Text = title;
            this.MouseDown += new MouseEventHandler(form_MouseDown);
            this.MouseWheel += new MouseEventHandler(form_MouseWheel);
        }
        static void Main(string[] args)
        {
            
            MainApp form = new MainApp("title");
            form.Width = 300;
            form.Height = 200;


            Application.Run(new MainApp("mouse"));
        }


        void form_MouseDown(object sender, MouseEventArgs e)
        {
            Form form = (Form)sender;
            int oldWidth = form.Width;
            int oldHeight = form.Height;

            if(e.Button == MouseButtons.Left)
            {
                Color oldColor = this.BackColor;
                this.BackColor = Color.FromArgb(
                    rand.Next(0, 255),
                    rand.Next(0, 255), 
                    rand.Next(0, 255));
            }
            else if (e.Button == MouseButtons.Right)
            {
                if( this.BackgroundImage != null)
                {
                    this.BackgroundImage = null;
                    return;
                }

                string file = "winter.jpg";
                if(System.IO.File.Exists(file) == false)
                {
                    MessageBox.Show("이미지 파일이 없습니다.");
                }
                else
                    this.BackgroundImage = Image.FromFile(file);
            }
            Console.WriteLine("윈도우의 크기가 변경되었습니다.");
        }

        void form_MouseWheel(object sender, MouseEventArgs e)
        {
            this.Opacity = this.Opacity + (e.Delta > 0 ? 0.1 : -0.1);
            Console.WriteLine("{0}", this.Opacity);
        }
        public void MyMouseHandler(object sender, MouseEventArgs e)
        {
            Console.WriteLine("sender");
            Console.WriteLine("X : {0}, Y : {1}", e.X, e.Y);
            Console.WriteLine("{0} {1}", e.Button, e.Clicks);
            Console.WriteLine();
        }
    }

    class MessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x0F || m.Msg == 0x200 || m.Msg == 0x113) return false;
            
            Console.WriteLine("{0} : {1}", m.ToString(), m.Msg);

            if (m.Msg == 0x201)
                Application.Exit();
            
            return true;
        }
    }
}
