using System;

namespace BankApp
{
    class Bank_Account : Account
    {
        private Bank bank { get; set; }

        public Bank_Account(Financial_issues _account_opened, Financial_issues _account_closed, decimal start_up_capital, Bank bank) : base(_account_opened, _account_closed)
        {
            Amount_of_money = start_up_capital;

            Amount_of_money = start_up_capital;

            account_opened += BankAppCore.BankApp_Core.Add_Bank;
            account_closed += BankAppCore.BankApp_Core.Remove_Bank;

            Select_Event(new Info_Blank(bank, this), Action.Open_Account);

        }

        protected override void Put_Money(Info_Blank info_Blank)
        {
            if (info_Blank.Summ > 0 && info_Blank.Succes)
            {
                Amount_of_money += info_Blank.Summ;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\nБанківський рахунок {info_Blank.To_Acc.Account_ID} поповнено на {info_Blank.Summ:c}. Баланс: {info_Blank.To_Acc.Amount_of_money:c}. ");
                Console.ResetColor();
            }
        }

        protected override void Take_Money(Info_Blank info_Blank)
        {
            if (Amount_of_money >= info_Blank.Summ && info_Blank.Summ > 0)
            {
                Amount_of_money -= info_Blank.Summ;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"\nЗ банківського рахунку {info_Blank.Selected_account.Account_ID} знято {info_Blank.Summ:c}. Баланс: {info_Blank.Selected_account.Amount_of_money:c}. ");
                Console.ResetColor();
            }

            else
            {
                info_Blank.Succes = false;
            }
        }

        /// <summary>
        /// Виводить основні дані про рахунок
        /// </summary>
        public override void Show_Account()
        {
            Console.WriteLine($" \nБанківський рахунок : {Account_ID}. Власник: {bank.Bank_name}. Баланс: {Amount_of_money:c}.");
        }

        /// <summary>
        ///  Вибирає подію на основі enum Action
        /// </summary>
        /// <param name="info_Blank"></param>
        /// <param name="action"></param>
        public override void Select_Event(Info_Blank info_Blank, Action action)
        {
            switch (action)
            {
                case Action.Log_in_to_Account:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Exit_from_Account:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Open_Account:
                    goto default;

                case Action.Close_Account:
                    goto default;

                case Action.Add_Money:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Withdraw_Money:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Transfer_Money:
                    goto default;

                case Action.Add_Co_Owner:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Pay_Commission:
                    break;

                case Action.Redistribution_of_Funds:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                default:
                    Select_Base_Event(info_Blank, action);
                    break;
            }
        }
    }
}
