using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace test
{


    internal class Program
    {
        static void Main(string[] args)
        {
            MyClass obj = new MyClass();
            
            Console.ReadLine();
        }
        static void reple()
        {
            Type type = Type.GetType("test.Profile");
            MethodInfo methodInfo = type.GetMethod("Print");
            PropertyInfo nameProperty = type.GetProperty("Name");
            PropertyInfo phoneProperty = type.GetProperty("Phone");

            object profile = Activator.CreateInstance(type, "박상현", "512-1234");
            methodInfo.Invoke(profile, null);
            profile = Activator.CreateInstance(type);


            nameProperty.SetValue(profile, "박찬호", null);
            phoneProperty.SetValue(profile, "997-5511", null);

            Console.WriteLine("{0}, {1}", nameProperty.GetValue(profile, null), phoneProperty.GetValue(profile, null));


        }
        static void PrintInterfaces(Type type)
        {
            Console.WriteLine("------ Interfaces ------");

            Type[] interfaces = type.GetInterfaces();
            foreach (Type i in interfaces)
            {
                Console.WriteLine("Name : {0}", i.Name);
            }
            Console.WriteLine();
        }

        static void PrintFields(Type type)
        {
            Console.WriteLine("------ Fields ------");

            FieldInfo[] fields = type.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                String accessLevel = "protected";
                if (field.IsPublic) accessLevel = "public";
                else if (field.IsPrivate) accessLevel = "private";

                Console.WriteLine("Access:{0}, Type:{1}, Name:{2}",
                    accessLevel, field.FieldType.Name, field.Name);
            }
            Console.WriteLine();
        }

        static void PrintMethods(Type type)
        {
            Console.WriteLine("------ Methods ------");

            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                Console.WriteLine("Type:{0}, Name:{1}, Parameter:",
                   method.ReturnType.Name, method.Name);

                ParameterInfo[] args = method.GetParameters();

                for (int i = 0; i < args.Length; i++)
                {
                    Console.Write("{0}", args[i].ParameterType.Name);
                    if (i < args.Length - 1)
                        Console.Write(", ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void PrintProperties(Type type)
        {
            Console.WriteLine("------ Properties ------");

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine("Type:{0}, Name:{1}",
                   property.PropertyType.Name, property.Name);

                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]    
    class History : System.Attribute
    {
        private string programmer;
        public double Version;
        public string Changes;

        public History(string programmer)
        {
            this.programmer = programmer;
            Version = 1.0;
            Changes = "First release";
        }

        public string GetProgrammer()
        {
            return programmer;
        }
    }
    [History("sean", Version = 0.1, Changes = "2024-10-25 Created class stub")]

    [History("bob", Version = 0.2, Changes = "2024-10-26 Created class stub")]
        class MyClass
        {
        public void Func()
        {
            Console.WriteLine("Func()");
        }
            
        }

        class Profile
        {
            public string name;
            public string phone;
            //public int Height { get; set; }

            public Profile()
            {
                name = ""; phone = "";
            }
            public Profile(string name, string phone)
            {
                this.name = name; this.phone = phone;
            }

            public void Print()
            {
                Console.WriteLine("{0}, {1}", name, phone);
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string Phone
            {
                get { return phone; }
                set { phone = value; }
            }

        }
        class Class
        {
            public string name { get; set; }
            public int[] Score { get; set; }
        }

        /*         void test2()
                 {
                     Class[] arrClass =
                 {
                     new Class() { Name = "연두반", Score = new int[] { 99, 80, 70, 24 } },
                     new Class() { Name = "분홍반", Score = new int[] { 60, 45, 87, 72 } },
                     new Class() { Name = "파랑반", Score = new int[] { 92, 30, 85, 94 } },
                     new Class() { Name = "노랑반", Score = new int[] { 90, 88, 0, 17 } }
                 };

                     var classes = from c in arrClass
                                   from s in c.Score
                                   where s < 60
                                   select new { c.Name, lowest = s };
                     foreach (var c in classes)
                     {
                         Console.WriteLine("낙제 : {0} ({1})", c.Name, c.lowest);
                     }
                 }
                 */
        /*       void test3()
               {
                   Profile[] arrProfile =
                  {
                       new Profile() { Name ="정우성", Height=186},
                       new Profile() { Name ="김태희", Height=158},
                       new Profile() { Name ="고현정", Height=172},
                       new Profile() { Name ="이문세", Height=178},
                       new Profile() { Name ="하동훈", Height=171},
                   };

                   var heightstat = from profile in arrProfile
                                    group profile by profile.Height < 175 into g
                                    select new
                                    {
                                        Group = g.Key == true ? "175미만" : "175이상",
                                        Count = g.Count(),
                                        Max = g.Max(profile => profile.Height),
                                        Min = g.Min(profile => profile.Height),
                                        Average = g.Average(profile => profile.Height)

                                    };

                   foreach (var stat in heightstat)
                   {
                       Console.WriteLine("{0} - Count : {1}, Max : {2}, ",
                           stat.Group, stat.Count, stat.Max);
                       Console.WriteLine("Min : {0}, Average : {1}, ",
                           stat.Min, stat.Average);
                   }
               }
           }*/
        class ExpressionTressTest
        {
            /* void test1()
             {
                 Expression const1 = Expression.Constant(1);
                 Expression const2 = Expression.Constant(2);

                 Expression leftExp = Expression.Multiply(const1, const2);

                 Expression param1 = Expression.Parameter(typeof(int));
                 Expression param2 = Expression.Parameter(typeof(int));

                 Expression rightExp = Expression.Subtract(param1, param2);
                 Expression exp = Expression.Add(leftExp, rightExp);
                 Expression<Func<int, int, int>> expression =
                      Expression<Func<int, int, int>>.Lambda<Func<int, int, int>>(
                          exp, new ParameterExpression[]
                          {
                          (ParameterExpression)param1,
                          (ParameterExpression)param2 });
                 Func<int, int, int> func = expression.Compile();

                 Console.WriteLine("1*2+({0}-{1}) = {2}", 7, 8, func(7, 8));
             }

             void test2()
             {
                 Expression<Func<int, int, int>> expression =
                        (a, b) => 1 * 2 + (a - b);
                 Func<int, int, int> func = expression.Compile();

                 Console.WriteLine("1*2+({0}-{1}) = {2}", 7, 8, func(7, 8));

             }*/
        }

    
}
