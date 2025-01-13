using System;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Join
{

    class Profile
    {
        public string Name { get; set; }
        public int Height { get; set; }
    }

    class Product
    {
        public string GroupName { get; set; }
        public string Star { get; set; }
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

            Product[] arrProduct =
            {
                new Product(){GroupName = "아이브", Star="장원영"},
                new Product(){GroupName = "아이즈원", Star="장원영"},
                new Product(){GroupName = "에스파", Star="카리나" },
                new Product(){GroupName = "에스파", Star="윈터"},
                new Product(){GroupName = "있지", Star="유나"}
            };

            var listProfile = from profile in arrProfile
                              join product in arrProduct on profile.Name equals product.Star
                              select new
                              {
                                  Name = profile.Name,
                                  GroupName = product.GroupName,
                                  Height = profile.Height
                              };
            Console.WriteLine("--- 내부 조인 결과 ---");

            foreach ( var profile in listProfile)
            {
                Console.WriteLine("이름 : {0} , 그룹 : {1} , 키 : {2}cm", profile.Name, profile.GroupName, profile.Height);
            }

            listProfile = from profile in arrProfile
                          join product in arrProduct on profile.Name equals product.Star
                          into ps
                          from product in ps.DefaultIfEmpty(new Product(){ GroupName = "솔로"})
                          select new
                          {
                              Name = profile.Name,
                              GroupName = product.GroupName,
                              Height = profile.Height
                          };
            Console.WriteLine();
            Console.WriteLine("--- 외부 조인 결과 ---");

            foreach (var profile in listProfile)
            {
                Console.WriteLine("이름 : {0} , 그룹 : {1} , 키 : {2}cm", profile.Name, profile.GroupName, profile.Height);
            }

        }
    }
}
