using System;

namespace BankApp
{
    class Depozit_Account : Account
    {
        private double Annual_percentage { get; init; } // Річний відсоток, який сплачує банк депозитним рахункам
        private decimal Profit_per_year { get; set; } // Прибуток за рік (В цілях демонстрації річний прибуток виплачується щохвилини)
        public User User { get; private init; }
         
        public Depozit_Account(double annual_percentage, Financial_issues _account_opened, Financial_issues _account_closed) : base(_account_opened, _account_closed)
        {
            User = User.Create_User();

            Annual_percentage += annual_percentage;

        }

        protected override void Put_Money(Info_Blank info_Blank)
        {
            base.Put_Money(info_Blank);
        }

        protected override void Take_Money(Info_Blank info_Blank)
        {
            base.Take_Money(info_Blank);
        }

        /// <summary>
        /// Робить спробу застросувати одну з дій, доступних цьому виду рахунку
        /// </summary>
        /// <param name="info_Blank"></param>
        /// <param name="action"></param>
        public override void Select_Event(Info_Blank info_Blank, Action action)
        {
            switch (action)
            {
                case Action.Log_in_to_Account:
                    goto default;

                case Action.Exit_from_Account:
                    goto default;

                case Action.Open_Account:
                    goto default;

                case Action.Close_Account:
                    goto default;

                case Action.Add_Money:
                    goto default;

                case Action.Withdraw_Money:
                    goto default;

                case Action.Transfer_Money:
                    goto default;

                case Action.Add_Co_Owner:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Redistribution_of_Funds:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

                case Action.Pay_Commission:
                    goto default;

                default:
                    Select_Base_Event(info_Blank, action);
                    break;
            }
        }

        /// <summary>
        /// Виводить основні дані про рахунок
        /// </summary>
        public override void Show_Account()
        {
            Console.WriteLine($"Депозитний рахунок під {Annual_percentage} % на рік: {Account_ID}. Власник: {User.First_Name} {User.Last_Name}. Баланс: {Amount_of_money:c}.");
        }

        /// <summary>
        /// розраховує річний прибуток на основі суми наявних грошей та річного відсотка банку
        /// </summary>
        /// <returns></returns>
        public decimal Calculate_profit()
        {
            Profit_per_year = Amount_of_money * (decimal) Annual_percentage;
            return Profit_per_year;
        }
    }
}