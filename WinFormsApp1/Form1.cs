namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        Point startPt, endPt;
        bool penFlag = false;
        bool colorFlag = true;
        bool eraserFlag = false;
        bool brFlag;
        Color color1 , color2;
        int penSize;

        Graphics gp;

        Pen pen;
        Brush brush;
        Rectangle eraserRect;


        MyButton[] toolButton = new MyButton[8];
        MyButton[] colorButton = new MyButton[2];

        public Form1()
        {
            Initialize();
            InitializeComponent();
            button_pen_Click(toolButton[0], EventArgs.Empty);
        }
        private void Initialize()
        {
            color1 = new Color(); color1 = Color.Black;
            color2 = new Color(); color2 = Color.White;
            pen = new Pen(color1);
            brush = new SolidBrush(color2);
            eraserRect = new Rectangle();
            eraserRect.X = 0;
            eraserRect.Y = 0;
            eraserRect.Width = 10;
            eraserRect.Height = 10;


            for (int i = 0; i < toolButton.Length; i++)
            {
                toolButton[i] = new MyButton();
            }
            for (int i = 0; i < colorButton.Length; i++)
            {
                colorButton[i] = new MyButton();
            }
        }
  
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            penFlag = !penFlag;
        }

      

        private void button_pen_Click(object sender, EventArgs e)
        {
            Color color;
            if (colorFlag) color = color1;
            else
            {
                color = color2;
            }
            pen = new Pen(color, penSize);
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_paint_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_eraser_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_textbox_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_line_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_circle_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_triangle_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }

        private void button_rectangle_Click(object sender, EventArgs e)
        {
            setUseBtnFlagOn((MyButton)sender);
        }
        private void button_colordlg1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                color1 = cd.Color;
                colorButton[0].BackColor = color1;
            }
            colorFlag = true;
        }

        private void button_colordlg2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                color2 = cd.Color;
                colorButton[1].BackColor = color2;
            }
            colorFlag = false;
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
           
            startPt = e.Location;
            if (searchUseBtn() == 0)
            {
                penbrUpdate(color1,color2);
                gp.DrawLine(pen, startPt, startPt);
                penFlag = true;
            }
            else if (searchUseBtn() == 2)
            {
                penbrUpdate(Color.White, Color.White);
                eraserRect.Offset(startPt);
                gp.FillRectangle(brush,eraserRect);
                eraserFlag = true;
            }
            else
            {

            }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (penFlag)
            {
                gp.DrawLine(pen, startPt, e.Location);
                startPt = e.Location;
            }
            else if(eraserFlag)
            {
                eraserRect.X = e.X; eraserRect.Y = e.Y;
                gp.FillRectangle(brush, eraserRect);
            }
            else
            {
                return;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            penbrUpdate(color1, color2);

            switch (searchUseBtn())
            {
                case 0:
                    penFlag = false;
                    break;
                case 2:
                    eraserFlag = false;
                    break;
                case 4:
                    gp.DrawLine(pen, startPt, e.Location);
                    break;
                case 5:
                case 6:
                case 7:
                    drawPolygon(e.Location, searchUseBtn());
                    break;
                default:
                    return;
            }
            
        }

        private void penbrUpdate(Color c1, Color c2)
        {
            pen.Dispose(); brush.Dispose();
            pen = new Pen(c1);
            brush = new SolidBrush(c2);
        }
        private void button_radio1_CheckedChanged(object sender, EventArgs e)
        {
            radioBtn_Check(sender, e);
        }

        private void button_radio2_CheckedChanged(object sender, EventArgs e)
        {
            radioBtn_Check(sender, e);
        }

        private void radioBtn_Check(object sender, EventArgs e)
        {
            if (radioButton[0].Checked) brFlag = false;
            else if (radioButton[1].Checked) brFlag = true;
        }

        private void drawPolygon(Point end , int type)
        {
            if(type == 5)
            {
                drawEllipse(end);
            }
            else if (type == 6)
            {
                drawTriangle(end);
            }
            else if(type == 7)
            {
                drawRect(end);
            }
        }
        private void drawTriangle(Point end)
        {
            int Width = getLengthP2P(startPt.X, end.X);
            int Height = getLengthP2P(startPt.Y, end.Y);

            Point spt = searchStartPoint(startPt, end);

            Point[] pt = new Point[3]; 
            pt[0].X = spt.X + Width / 2; pt[0].Y = spt.Y;
            pt[1].X = spt.X; pt[1].Y = spt.Y+Height;
            pt[2].X = end.X; pt[2].Y = spt.Y+Height;

            if(brFlag)
                gp.FillPolygon(brush, pt);
            gp.DrawPolygon(pen, pt);
        }

        private void drawEllipse(Point end)
        {
            int Width = getLengthP2P(startPt.X, end.X);
            int Height = getLengthP2P(startPt.Y, end.Y);

            Point spt = searchStartPoint(startPt, end);
            if (brFlag)
                gp.FillEllipse(brush, spt.X,spt.Y,Width,Height);
            gp.DrawEllipse(pen, spt.X, spt.Y, Width, Height);
        }
        private void drawRect(Point end)
        {
            int Width = getLengthP2P(startPt.X, end.X);
            int Height = getLengthP2P(startPt.Y, end.Y);

            Point spt = searchStartPoint(startPt, end);
            if (brFlag)
                gp.FillRectangle(brush, spt.X, spt.Y, Width, Height);
            gp.DrawRectangle(pen, spt.X, spt.Y, Width, Height);
        }

        private Point searchStartPoint(Point pt1, Point pt2)
        {
            Point result = new Point();

            result.X = pt1.X < pt2.X ? pt1.X : pt2.X;
            result.Y = pt1.Y < pt2.Y ? pt1.Y : pt2.Y;
          
            return result;
        }


        private int getLengthP2P(int s, int e)
        {
            return Math.Abs(s - e);
        }




        private void setUseBtnFlagOn(MyButton btn)
        {
            setBtnFlagOffAll();
            btn.flagOn();
        }
        private void setBtnFlagOffAll()
        {
            for (int i = 0; i < toolButton.Length; i++)
            {
                toolButton[i].flagOff();
            }
        }
        private int searchUseBtn()
        {
            for(int i = 0; i < toolButton.Length; i++)
            {
                if (toolButton[i].isUseBtn()) return i;
            }
            return -1;
        }
    }


    class MyButton : Button
    {
        private bool useflag = false;
        public MyButton()
        {
            flagOff();
        }
        public void flagOn()
        {
            useflag = true;
        }
        public void flagOff()
        {
            useflag = false;
        }
        public bool isUseBtn()
        {
            return useflag;
        }
    }







}