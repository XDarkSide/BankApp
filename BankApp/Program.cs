using System;

namespace BankApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            Bank bank1 = new Bank("PrivatBank", 0.1, 0.01, 10000);
            Bank bank2 = new Bank("MonoBank", 0.2, 0.02, 50000);
            Bank bank3 = new Bank("AlphaBank", 0.3, 0.03, 20000);

            BankAppCore.BankApp_Core.Session();
        }
    }
}

/*
 *   Для всіх новостворених рахунків логін і пароль встановлюється автоматично на <User> <1234> відповідно.
 */