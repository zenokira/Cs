using System;

namespace StatementLambda
{
    class MainApp
    {
        delegate string Concatenate(string[] args);

        static void Main(string[] args)
        {
            string[] arr = new string[5] { "abc", "bcd", "cde", "def", "efg" };
            Concatenate concat = (arr) =>
            {
                string result = "";
                foreach (string arg in arr)
                {
                    result += arg;
                }
                return result;
            };

            Console.WriteLine(concat(arr));
        }

    }
}
