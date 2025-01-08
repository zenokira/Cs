using System;
using System.Data;

namespace EX13_2
{
    delegate void MyDelegate(int a);
    class Market
    {
        public event MyDelegate CustomerEvent;

        public void BuySomething(int CustomerNo)
        {
            if (CustomerNo == 30)
            {
                CustomerEvent(CustomerNo);
            }
        }
    }
    class MainApp
    {
        static void Main(string[] args)
        {
            Market market = new Market();
            market.CustomerEvent += new MyDelegate(delegate(int a)
            {
                Console.WriteLine($"축하합니다! {a}번째 고객 이벤트에 당첨되셨습니다.");
            });

            for (int customerNo = 0; customerNo < 100; customerNo++)
            {
                market.BuySomething(customerNo);
            }

        }
    }
}
