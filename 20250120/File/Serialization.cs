using System;
using System.IO;
using System.Text.Json;

namespace Serialization
{
    class NameCard
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var fileName = "a.json";

            using (Stream ws = new FileStream(fileName, FileMode.Create))
            {
                NameCard nc = new NameCard()
                {
                    Name = "김민정",
                    Phone = "010-1234-5678",
                    Age = 24
                };

                string jsonString = JsonSerializer.Serialize<NameCard>(nc);
                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                ws.Write(jsonBytes,0,jsonBytes.Length);
            }

            using (Stream rs = new FileStream(fileName, FileMode.Open))
            {
                byte[] jsonBytes = new byte[rs.Length];
                rs.Read(jsonBytes,0,jsonBytes.Length); 
                string jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                NameCard nc2 = JsonSerializer.Deserialize<NameCard>(jsonString);

                Console.WriteLine($"Name : {nc2.Name}");
                Console.WriteLine($"Phone : {nc2.Phone}");
                Console.WriteLine($"Age : {nc2.Age}");
            }
           

        }
    }
}
