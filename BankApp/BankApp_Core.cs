using System;
using System.Collections.Generic;
using BankApp;

namespace BankAppCore
{
    static class BankApp_Core
    {
        static List<Bank> banks = new List<Bank>();
        static Bank select_Bank;
        static Bank.acc_Type select_type;
        static Account.Action select_action;

        public static void Add_Bank(Info_Blank info_Blank)
        {
            banks.Add(info_Blank.Bank_address);
        }

        public static void Remove_Bank(Info_Blank info_Blank)
        {
            banks.Remove(info_Blank.Bank_address);
        }

        private static void Select_Action()
        {
            bool correctly = false;
            int select = 0;

            Console.WriteLine();

            for (Account.Action i = (Account.Action)1; i <= Account.Action.Back_To_Bank_Select; i++)
            {
                Console.WriteLine((int)i + ". " + i);
            }

            Console.Write("\nВиберіть дію: ");

            while (!correctly)
            {
                string input = Console.ReadLine();
                correctly = int.TryParse(input, out select);

                if (select < (int)Account.Action.Log_in_to_Account || select > (int)Account.Action.Back_To_Bank_Select)
                {
                    correctly = false;
                    Console.Write("Введно некоректний ID дії. Спробуйте ще раз: ");
                }
            }

            select_action = (Account.Action) select;
        }

        private static void Select_Type()
        {
            bool correctly = false;
            int select = 0;

            Console.WriteLine();

            for (Bank.acc_Type i = (Bank.acc_Type)1; i <= Bank.acc_Type.Credit; i++)
            {
                Console.WriteLine((int)i + ". " + i);
            }

            Console.Write("\nВиберіть тип рахунку: ");

            while (!correctly)
            {
                string input = Console.ReadLine();
                correctly = int.TryParse(input, out select);

                if (select < (int)Bank.acc_Type.Depozit || select > (int)Bank.acc_Type.Credit)
                {
                    correctly = false;
                    Console.Write("Введно некоректний ID рахунку. Спробуйте ще раз: ");
                }
            }

            select_type = (Bank.acc_Type) select;
        }

        private static void Select_Bank()
        {
            int i = 1;
            bool correctly = false;
            int select = 0;

            Console.WriteLine();

            foreach (Bank b in banks)
            {
                Console.WriteLine(i + ". " + b.Bank_name);
                i++;
            }

            Console.Write("\nВиберіть банк: ");

            while (!correctly)
            {
                string input = Console.ReadLine();
                correctly = int.TryParse(input, out select);

                if (select < 0 || select > banks.Count)
                {
                    correctly = false;
                    Console.Write("Введно некоректний ID банку. Спробуйте ще раз: ");
                }
            }

            select_Bank = banks[select - 1];
            Console.WriteLine($"Вибрано {select_Bank.Bank_name}. Використовують осіб : {select_Bank.Number_of_customers}.\n");
        }

        public static decimal Write_Summ()
        {
            decimal summ = 0;
            bool correctly = false;

            Console.Write($"Введіть суму: ");

            while (!correctly)
            {
                string input = Console.ReadLine();
                correctly = decimal.TryParse(input, out summ);

                if (summ <= 0) correctly = false;

                if (!correctly)
                {
                    Console.Write("Введене вами значення, некоректне. Спробуйте ще раз: ");
                }
            }

            return summ;
        }

        private static double Write_Share()
        {
            double share = 0;
            bool correctly = false;

            Console.Write($"Введіть долю відповідальності: ");

            while (!correctly)
            {
                string input = Console.ReadLine();
                correctly = double.TryParse(input, out share);

                if (share > 1 || share <= 0) correctly = false;

                if (!correctly)
                {
                    Console.Write("Введене вами значення, некоректне. Доля відповідальності має міститися в діапазоні (0;1]. Спробуйте ще раз: ");
                }
            }

            return share;
        }

        public static string Write_ID()
        {
            Console.Write("Введіть ID рахунку: ");

            string ID = Console.ReadLine();

            return ID;
        }

        public static void Session()
        {
            loop1:
            Select_Bank();

            while (true)
            {
                Select_Action();

                switch (select_action)
                {
                    case Account.Action.Log_in_to_Account:
                        select_Bank.Select_Account(Write_ID());
                        break;
                    case Account.Action.Exit_from_Account:
                        select_Bank.Exit_Account();
                        break;
                    case Account.Action.Open_Account:
                        Select_Type();
                        select_Bank.Open_Account<Account>(select_type);
                        break;
                    case Account.Action.Close_Account:
                        select_Bank.Close_Account();
                        break;
                    case Account.Action.Add_Money:
                        select_Bank.Add_Money(Write_Summ());
                        break;
                    case Account.Action.Withdraw_Money:
                        select_Bank.Withdraw_Money(Write_Summ());
                        break;
                    case Account.Action.Transfer_Money:
                        select_Bank.Transfer_money(Write_ID(), Write_Summ());
                        break;
                    case Account.Action.Add_Co_Owner:
                        select_Bank.Add_Co_Owner(Write_ID(), Write_Share());
                        break;
                    case Account.Action.Redistribution_of_Funds:
                        select_Bank.Redistribution_of_Funds(Write_ID());
                        break;
                    case Account.Action.Back_To_Bank_Select:
                        goto loop1;
                }

                if (select_Bank.Selected_Acc != null && select_Bank.Selected_Acc.Access)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\nВибраний рахунок: ");
                    select_Bank.Selected_Acc.Show_Account();
                    Console.Write("\n");
                    Console.ResetColor();
                }
            }
        }
    }
}
