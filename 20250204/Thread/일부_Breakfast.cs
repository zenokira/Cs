using System.Diagnostics;
using System.Threading.Tasks;

namespace ex_breakfast
{
    class Breakfast
    {
        bool cookingFlag = true;
        static void Main(string[] args)
        {
            Breakfast bf = new Breakfast();
            bf.StartAsync();

            while(bf.cookingFlag)
            {
                Task.Delay(1000).Wait();
                Console.WriteLine("==>전체를보는중");
            }
        }
        async Task StartAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("커피 준비됨");

            Task<Egg> eggsTask = FryEggsAsync(2);
            Task<Bacon> baconTask = FryBaconAsync(3);
            Task<Toast> toastTask = ToastBreadAsync(2);

        
            Toast toast =  await toastTask;
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("토스트 준비됨");

            Juice oj = PourOJ();
            Console.WriteLine("오렌지쥬스 준비됨");

            Egg eggs = await eggsTask;
            Console.WriteLine("달걀 준비됨");

            Bacon bacon = await baconTask;
            Console.WriteLine("베이컨 준비됨");


            sw.Stop();
            Console.Write((sw.ElapsedMilliseconds / 1000) + "초만에 ");
            Console.WriteLine("아침식사 준비됨!");
            cookingFlag = false;
        }

        private Juice PourOJ()
        {
            Console.WriteLine("오렌지쥬스 따르기");
            return new Juice();
        }

        private void ApplyJam(Toast toast) => Console.WriteLine("토스트에 잼 바르기");
        private void ApplyButter(Toast toast) => Console.WriteLine("토스트에 버터 바르기");

        private async Task<Toast> ToastBreadAsync(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
                Console.WriteLine("토스트기에 빵 넣기");
            Console.WriteLine("빵 굽기");
            await Task.Delay(3000);
            Console.WriteLine("토스트기에서 빵 빼기");

            return new Toast();
        }

        private async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine($"후라이팬에 베이컨 {slices} 조각 넣기");
            Console.WriteLine("베이컨 앞면 굽기");
            await Task.Delay(3000);

            for (int slice = 0; slice < slices; slice++)
                Console.WriteLine((slice + 1) + "번째 베이컨조가 뒤집기");
            Console.WriteLine("베이컨 뒷면 굽기");
            await Task.Delay(3000);
            Console.WriteLine("접시에 베이컨 담기");

            return new Bacon();
        }

        private async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("달걀용 후라이팬 달구기");
            await Task.Delay(3000);
            Console.WriteLine($"{howMany}개 달걀 깨기");
            Console.WriteLine("달걀 요리하기");
            await Task.Delay(3000);
            Console.WriteLine("접시에 계란후라이 놓기");

            return new Egg();
        }

        private Coffee PourCoffee()
        {
            Console.WriteLine("커피 따르기");
            return new Coffee();
        }
    }
    internal class Juice { }
    internal class Toast { }
    internal class Bacon { }
    internal class Egg { }
    internal class Coffee { }
}
