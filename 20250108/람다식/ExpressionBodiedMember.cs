using System;

namespace ExpressionBodiedMember
{
    class FriendList
    {
        private List<string> list = new List<string>();

        public void Add(string name) => list.Add(name);
        public void Remove(string name) => list.Remove(name);
        public void PrintAll()
        {
            foreach (var item in list)
                Console.WriteLine(item);
        }

        public FriendList() => Console.WriteLine("FriendList()");
        ~FriendList() => Console.WriteLine("~FriendList()");

        public int Capacity
        {
            get => list.Capacity;
            set => list.Capacity = value;   
        }

        public string this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
    }
    class MainApp
    {
        static void Main(string[] args)
        {
            FriendList obj = new FriendList();
            obj.Add("Eeny");
            obj.Add("Meeny");
            obj.Add("Miny");
            obj.PrintAll();

            obj.Remove("Eeny");
            obj.PrintAll();

            Console.WriteLine($"{obj.Capacity}");
            obj.Capacity = 10;
            Console.WriteLine($"{obj.Capacity}");

            Console.WriteLine($"{obj[0]}");
            obj[0] = "Moe";
            obj.PrintAll();
        }
    }
}
