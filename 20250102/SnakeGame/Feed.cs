using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace SnakeGame
{
    
    internal class Feed
    {
        public const int FEED_SIZE = 20;
        public const int FEED_BLANK = 5;

        int MenuHeight;

        List<Label> feedList = new();
        Control.ControlCollection form_control;
        public Feed(Control.ControlCollection control , int menuHeight)
        {
            MenuHeight = menuHeight;
            form_control = control;
            feedList.Clear();
        }

        public Label CreateFeed(Point location)
        {
            Label lbl = new();
            lbl.Name = "feed";
            lbl.BackColor = Color.IndianRed;
            lbl.Size = new Size(FEED_SIZE, FEED_SIZE);
            lbl.TabIndex = 0;
            lbl.Text = "";
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Location = location;

            return lbl;
        }

        public void FeedAdd(Label feed)
        {
            feedList.Add(feed);
            form_control.Add(feed);
        }
        public void FeedListClear()
        {
            int cnt = feedList.Count;
            if (cnt <= 0) return;
            for (int i = cnt - 1; i >= 0; i--)
            {
                form_control.Remove(feedList[i]);
                feedList.RemoveAt(i);
            }
        }
        public void removeFeed(Label feed)
        {
            int cnt = feedList.Count;
            for (int i = cnt - 1; i >= 0; i--)
            {
                if (feedList[i] == feed)
                {
                    form_control.Remove(feedList[i]);
                    feedList.RemoveAt(i);
                    break;
                }
            }
        }

        public List<Label> getFeedList()
        {
            return feedList;
        }

        
    } 
}
