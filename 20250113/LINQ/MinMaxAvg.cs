using System;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace MinMaxAvg
{

    class Profile
    {
        public string Name { get; set; }
        public int Height { get; set; }
    }

    class MainApp
    {
        static void Main(string[] args)
        {
            Profile[] arrProfile =
            {
                new Profile(){Name="장원영", Height=173},
                new Profile(){Name="카리나", Height=168},
                new Profile(){Name="윈터", Height=164},
                new Profile(){Name="아이유", Height=162},
                new Profile(){Name="유나", Height=170}
            };

            var heightStat = from profile in arrProfile
                             group profile by profile.Height < 165 into g
                             select new
                             {
                                 Group = g.Key == true ? "165미만" : "165이상",
                                 Count = g.Count(),
                                 Max = g.Max(profile => profile.Height),
                                 Min = g.Min(profile => profile.Height),
                                 Average = g.Average(profile => profile.Height)
                             };
            
            foreach (var stat in heightStat)
            {
                Console.WriteLine("{0} - Count : {1}, Max : {2}, Min : {3}, Average : {4}", stat.Group, stat.Count, stat.Max, stat.Min, stat.Average);
            }
        }
    }
}
