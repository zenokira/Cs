using System;

namespace EX_14_1
{
    class MainApp
    {
        static void Main(string[] args)
        {
            Func<int> func_1 = () => 10;
            Func<int, int> func_2 = (a) => a * 2;

            Console.WriteLine(func_1() + func_2(30));
        }
    }
}
