using System;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace EX_15_1
{
    class Car
    {
        public int Cost { get; set; }
        public int MaxSpeed { get; set; }
    }

    class MainApp
    {
        static void Main(string[] args)
        {
            Car[] cars =
            {
                new Car(){Cost = 56, MaxSpeed = 120 },
                new Car(){Cost = 70, MaxSpeed = 150 },
                new Car(){Cost = 45, MaxSpeed = 180 },
                new Car(){Cost = 32, MaxSpeed = 200 },
                new Car(){Cost = 82, MaxSpeed = 280 }
            };

            var selected = /* Cost 가 50 이상, MaxSpeed 는 150이상 */
                from car in cars
                where car.Cost >= 50 
                where car.MaxSpeed >= 150
                select car;

            foreach (var car in selected)
            {
                Console.WriteLine($"{car.Cost}, {car.MaxSpeed}");
            }  
        }
    }
}
