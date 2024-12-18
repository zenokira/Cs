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
            lbl.Size = new Size(30, 30);
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
            Head.Location = new Point(300,300);
            Head.TabIndex = 0;
            Head.Text = ":";
            Head.TextAlign = ContentAlignment.MiddleLeft;
        }

        public bool isBumpWall(Rectangle rect)
        {
            return true;
        }

    
        public void Go()
        {
            Point oldHeadLocation = Head.Location;
            if( vec == SnakeVector.UP)
                Head.Location = new Point(Head.Left, Head.Top-30);
            else if (vec == SnakeVector.DOWN)
                Head.Location = new Point(Head.Left, Head.Top + 30);
            else if (vec == SnakeVector.LEFT)
                Head.Location = new Point(Head.Left - 30, Head.Top);
            else if (vec == SnakeVector.RIGHT)
                Head.Location = new Point(Head.Left + 30, Head.Top);

            if (Body.Count <= 0) return;

            Point oldBodyBefore = Body[0].Location;
            Body[0].Location = oldHeadLocation;
            Point oldBodyAfter = oldBodyBefore;
            for (int i = 1; i < Body.Count; i++)
            {
                oldBodyBefore = Body[i].Location;
                Body[i].Location = oldBodyAfter;
                oldBodyAfter = oldBodyBefore;
            }
        }
    }
}
