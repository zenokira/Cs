using System;
using System.IO;

namespace TextFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter sw = new StreamWriter(
                new FileStream("a.txt", FileMode.Create)))
            {
                sw.Write(int.MaxValue);
                sw.Write("Good morning!");
                sw.Write(uint.MaxValue);
                sw.Write("안녕하세요!");
                sw.Write(double.MaxValue);
            }

            using (StreamReader sr = new StreamReader(
               new FileStream("a.txt", FileMode.Open))) {
            
                Console.WriteLine($"File size : {sr.BaseStream.Length} bytes");
                while (sr.EndOfStream == false)
                {
                    Console.WriteLine($"{sr.ReadLine()}");
                }
            }
           

        }
    }
}
