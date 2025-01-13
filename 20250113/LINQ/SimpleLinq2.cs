using System;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace SimpleLinq2
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

            var profiles = arrProfile
                .Where(profile => profile.Height < 175)
                .OrderBy(profile => profile.Height)
                .Select(profile => new { Name = profile.Name, InchHeight = profile.Height * 0.393 });

            foreach (var profile in profiles)
            {
                Console.WriteLine($"{profile.Name}, {profile.InchHeight}");
            }
        }
    }
}
