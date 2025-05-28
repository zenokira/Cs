using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class User
    {
        private GAMESTATE? myState;
        private PLAYERORDER? playorder;
        public User()
        {

        }

        public User(PLAYERORDER po)
        {
            playorder = po;
        }

        public void Clear()
        {
            myState = null;
            playorder = null;
        }
        public GAMESTATE State { get; set; }
        public PLAYERORDER Order { get; set; }
    }
}
