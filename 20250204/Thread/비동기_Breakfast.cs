using System.Diagnostics;
using System.Threading.Tasks;

namespace ex_breakfast
{
    class Breakfast
    {
        static void Main(string[] args)
        {
            Breakfast bf = new Breakfast();
            bf.StartAsync();
        }
        async Task StartAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("커피 준비됨");

            Egg eggs = await  FryEggsAsync(2);
            Console.WriteLine("달걀 준비됨");

            Bacon bacon = await FryBaconAsync(3);
            Console.WriteLine("베이컨 준비됨");

            Toast toast =  await ToastBreadAsync(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("토스트 준비됨");

            Juice oj = PourOJ();
            Console.WriteLine("오렌지쥬스 준비됨");

            sw.Stop();
            Console.Write((sw.ElapsedMilliseconds / 1000) + "초만에 ");
            Console.WriteLine("아침식사 준비됨!");
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
            Task.Delay(3000).Wait();
            Console.WriteLine("토스트기에서 빵 빼기");

            return new Toast();
        }

        private async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine($"후라이팬에 베이컨 {slices} 조각 넣기");
            Console.WriteLine("베이컨 앞면 굽기");
            Task.Delay(3000).Wait();

            for (int slice = 0; slice < slices; slice++)
                Console.WriteLine((slice + 1) + "번째 베이컨조가 뒤집기");
            Console.WriteLine("베이컨 뒷면 굽기");
            Task.Delay(3000).Wait();
            Console.WriteLine("접시에 베이컨 담기");

            return new Bacon();
        }

        private async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("달걀용 후라이팬 달구기");
            Task.Delay(3000).Wait();
            Console.WriteLine($"{howMany}개 달걀 깨기");
            Console.WriteLine("달걀 요리하기");
            Task.Delay(3000).Wait();
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
