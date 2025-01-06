using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    class Obstacle
    {
        public const int OBSTACLE_SIZE = 30;

        private List<Label> obstacle;
        Control.ControlCollection form_control;

        Random random = new Random();

        int MenuHeight;
        public Obstacle(Control.ControlCollection control, int menuHeight)
        {
            form_control = control;
            Initialize();
            MenuHeight = menuHeight;
        }
        void Initialize()
        {
            obstacle = new List<Label>();
        }

        public void Add(Label label)
        {
            obstacle.Add(label);
            form_control.Add(label);
        }
        public void Clear()
        {
            int cnt = obstacle.Count();
            for (int i = cnt - 1; i >= 0; i--)
            {
                form_control.Remove(obstacle[i]);
            }
            obstacle.Clear();
        }

        public Label creteObstacle()
        {
            Label lbl = new();
            lbl.Name = "obstacle";
            lbl.BackColor = Color.Gray;
            lbl.Size = new Size(OBSTACLE_SIZE, OBSTACLE_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            return lbl;
        }

        public int Count()
        {
            return obstacle.Count;
        }
        public Label this[int index]
        {
            get { return obstacle[index]; }

            set
            {
                obstacle.Add(value);
                form_control.Add(value);
            }
        }

        public bool LocationContains(Point pt)
        {
            foreach (Label label in obstacle)
            {
                if (label.Location == pt) return true;
            }
            return false;
        }
    }
}