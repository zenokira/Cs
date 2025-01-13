using System;
using System.Text.RegularExpressions;

namespace GroupBy 
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

            var listProfile = from profile in arrProfile
                              orderby profile.Height
                              group profile by profile.Height < 170 into g
                              select new { GroupKey = g.Key, Profiles = g };
                        
            foreach (var Group in listProfile)
            {
                Console.WriteLine($"- 170cm 미만 ? : {Group.GroupKey}");

                foreach (var profile in Group.Profiles)
                {
                    Console.WriteLine($">>>{profile.Name}, {profile.Height}");
                }
            }
        }
    }
}
