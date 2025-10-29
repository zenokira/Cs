using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
    internal class Util
    {
         
    }

    public class UserInfo
    {
        public string Name { get; set; }
        public string Moves { get; set; }

        public UserInfo(string name, string Moves)
        {
            this.Name = name;
            this.Moves = Moves;
        }
    }

    public class CountComparer : IComparer<UserInfo>
    {
        public int Compare(UserInfo? x, UserInfo? y)
        {
            // x?.Moves => x 가 null 이 아니면 Moves 를 가져온다, 변환에 성공하면 xi 에 넣는다 실패시 0
            // int.TryParse(x?.Moves, out var xi) 성공시 true, 실패시 false  이후 3항연산자를 통해 xi 를 cx 에 or MaxValue 를 cx 에..
            int cx = int.TryParse(x?.Moves, out var xi) ? xi : int.MaxValue;
            int cy = int.TryParse(y?.Moves, out var yi) ? yi : int.MaxValue;
            return cx.CompareTo(cy);
        }
    }

    public class UserStore : IEnumerable<UserInfo>
    {
        private readonly List<UserInfo> _items = new();
        public event EventHandler? Changed;
        public void Add(UserInfo user)
        {
            _items.Add(user);
            _items.Sort(new CountComparer()); //  Add 시 자동 정렬
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void AddRange(IEnumerable<UserInfo> users)
        {
            _items.AddRange(users);
            _items.Sort(new CountComparer());
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            _items.Clear();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<UserInfo> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _items.Count;
        public UserInfo this[int index] => _items[index];
    }
}
