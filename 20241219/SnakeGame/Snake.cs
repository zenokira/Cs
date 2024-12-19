using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnakeGame
{

    public enum SnakeVector { LEFT, UP, RIGHT, DOWN };
    public enum SnakeType { Head, Body };
    internal class Snake
    {
        const int PIXEL_SIZE = 30;
        const int PIXEL_CNT = 10;

        Control.ControlCollection form_control;
        Label Head = new();
        List<Label> Body = new();

        SnakeVector vec;
        public Snake()
        {
            InitSnakeHead();
            vec = SnakeVector.LEFT;
        }

        public Snake(Control.ControlCollection control)
        {
            InitSnakeHead();
            vec = SnakeVector.LEFT;
            form_control = control;
            Initialize();
        }

        void Initialize()
        {
            InitSnakeHead();
            form_control.Add(Head);
        }
        public int BodyCount()
        {
            return Body.Count;
        }

        public Point HeadLocation()
        {
            return Head.Location;
        }
        public void BodyClear()
        {
            int cnt = Body.Count;
            for(int i = cnt-1; i >= 0; i--)
            {
                form_control.Remove(Body[i]);
                Body.RemoveAt(i);
            }
        }

        public Label GetSnakeHead()
        {
            return Head;
        }

        public List<Label> GetSnakeBody()
        {
            return Body;
        }

        public void Direction(SnakeVector dir)
        {
            if (((int)vec + 1) % 2 != ((int)dir) % 2) return;

            vec = dir;
           
        }
        public Label GetSnakeTail()
        {
            return Body[Body.Count-1];
        }
        public void Growth()
        {
            Label tail = CreateSnakeBody();
            if (Body.Count <= 0) tail.Location = Head.Location;
            else tail.Location = Body[Body.Count - 1].Location;
            Body.Add(tail);
            form_control.Add(tail);
        }
        Label CreateSnakeBody()
        {
            Label lbl = new();
            lbl.Name = "body";
            lbl.BackColor = Color.LightSeaGreen;
            lbl.Size = new Size(PIXEL_SIZE, PIXEL_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            return lbl;
        }

        void InitSnakeHead()
        {
            Head.BackColor = Color.LightSeaGreen;
            Head.Name = "head";
            Head.Size = new Size(30, 30);
            Head.Location = new Point(PIXEL_CNT / 2 * PIXEL_SIZE, PIXEL_CNT / 2 * PIXEL_SIZE);
            Head.TabIndex = 0;
            Head.Text = ":";
            Head.TextAlign = ContentAlignment.MiddleLeft;
        }

        public Point getHeadLocation()
        {
            return Head.Location;            
        }

        public Point getBodyLocation(Label body)
        {
            return body.Location;
        }
        public bool isBumpWall(Rectangle rect)
        {
            return true;
        }

    
        public void Go()
        {
            Point currentHead = Head.Location;
            HeadMove();

            if (Body.Count <= 0) return;

            BodyMove(currentHead);      
        }

        void HeadMove()
        {
            if (vec == SnakeVector.UP)
                Head.Location = new Point(Head.Left, Head.Top - PIXEL_SIZE);
            else if (vec == SnakeVector.DOWN)
                Head.Location = new Point(Head.Left, Head.Top + PIXEL_SIZE);
            else if (vec == SnakeVector.LEFT)
                Head.Location = new Point(Head.Left - PIXEL_SIZE, Head.Top);
            else if (vec == SnakeVector.RIGHT)
                Head.Location = new Point(Head.Left + PIXEL_SIZE, Head.Top);
        }

        void BodyMove(Point pt)
        {
            Point destination = pt;
            for (int i = 0; i < Body.Count; i++)
            {
                Point current = Body[i].Location;
                Body[i].Location = destination;
                destination = current;
            }
        }

        public Rectangle SnakeHeadRect()
        {
            return new Rectangle(Head.Location, Head.Size);
        }
    }
}
