using System;

namespace BankApp
{
    class Credit_Account : Account
    {
        private Bank Lender { get; init; } // Банк, що видав кредит
        private decimal Amount_of_debt { get; set; }
        private double Interest { get; init; }
        private User User { get; init; }

        protected override void Put_Money(Info_Blank info_Blank)
        {
            base.Put_Money(info_Blank);
        }

        protected override void Take_Money(Info_Blank info_Blank)
        {
            if (info_Blank.To_Acc == Lender.Bank_account)
            {
                if (Amount_of_money >= info_Blank.Summ + Commission(info_Blank))
                {
                    Amount_of_money -= info_Blank.Summ;

                    Console.WriteLine($"\nЗ рахунку {info_Blank.Selected_account.Account_ID} знято {info_Blank.Summ:c}. Баланс: {info_Blank.Selected_account.Amount_of_money:c}. ");

                    if (info_Blank.To_Acc == Lender.Bank_account) Amount_of_debt -= info_Blank.Summ;

                    if (Amount_of_debt <= 0)
                    {
                        Console.WriteLine($"\nБорг рахунку {Account_ID} погашено. Скоро рахунок буде закрито.\n ");
                        Select_Base_Event(new Info_Blank(Lender, this), Action.Close_Account);
                    }
                }

                else
                {
                    Console.WriteLine($"Для здійснення операції недостатньо коштів на рахунку {info_Blank.Selected_account.Account_ID}");
                    info_Blank.Succes = false;
                }
            }
            else
            {
                 Console.WriteLine("\nВи не можете надсилати кошти на рахунок, не призначений для виплати боргу! ");
            }
        }

        public Credit_Account(decimal amount_of_debt, double interest, Bank leader, Financial_issues _account_opened, Financial_issues _account_closed) : base (_account_opened, _account_closed)
        {
            Amount_of_debt = amount_of_debt;
            Interest = interest;
            Lender = leader;

            User = User.Create_User();
        }

        /// <summary>
        /// Вибирає подію на основі enum Action
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
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу, , оскільки ТІЛЬКИ кредитор може закрити цей рахунок.\n ");
                    break;

                case Action.Add_Money:
                    goto default;

                case Action.Withdraw_Money:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    break;

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
        /// Збільшує суму боргу, враховуючи відсоток під який давався борг
        /// </summary>
        public void Calculate_Debt()
        {
            Amount_of_debt += (Amount_of_debt * (decimal)Interest);
            Console.WriteLine($"\nСума боргу {Account_ID} зросла на {Interest*100} %, і складає тепер {Amount_of_debt:c}. Очікуйте колекторів :D");
        }

        /// <summary>
        /// Виводить основні дані про рахунок
        /// </summary>
        public override void Show_Account()
        {
            Console.WriteLine($"Рахунок {Account_ID} для погашення боргу перед {Lender.Bank_name}: {Lender.Bank_account.Account_ID}. Власник: {User.First_Name} {User.Last_Name}.\nСума боргу: {Amount_of_debt:c} під {Interest*100} % на рік.  Баланс: {Amount_of_money:c}");
        }
    }
}
