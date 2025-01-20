using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace Serialization2
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
                var list = new List<NameCard>();
                list.Add(new NameCard() { Name = "김민정", Phone = "010-1111-0101", Age = 24 });
                list.Add(new NameCard() { Name = "유지민", Phone = "010-2222-0411", Age = 25 });
                list.Add(new NameCard() { Name = "신유나", Phone = "010-3333-1209", Age = 22 });

                string jsonString = JsonSerializer.Serialize<List<NameCard>>(list);
                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                ws.Write(jsonBytes,0,jsonBytes.Length);
            }

            using (Stream rs = new FileStream(fileName, FileMode.Open))
            {
                byte[] jsonBytes = new byte[rs.Length];
                rs.Read(jsonBytes,0,jsonBytes.Length); 
                string jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                var list2 = JsonSerializer.Deserialize<List<NameCard>>(jsonString);

                foreach (NameCard nc in list2)
                    Console.WriteLine($"Name : {nc.Name}, Phone : {nc.Phone}, Age : {nc.Age}");
            }
        }
    }
}
