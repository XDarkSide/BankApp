using System;
using System.Collections.Generic;

namespace BankApp
{
    class Business_Account : Account
    {
        private List<Businessman> co_owners = new List<Businessman>(); // список співвласників бізнесу
        public string Organization_Name { get; private init; }
        public double Balance_of_Liability { get; private set; }

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
                    goto default;

                case Action.Add_Money:
                    goto default;

                case Action.Withdraw_Money:
                    Console.WriteLine($"Дія <{action}> недоступна для рахунку цього типу.\n ");
                    goto default;

                case Action.Transfer_Money:
                    goto default;

                case Action.Add_Co_Owner:
                    if (Access && !IsFrozen) Add_Co_Owner(info_Blank.Selected_account, info_Blank.Share);
                    else throw new Exception($"Немає доступу до рахунку {Account_ID}");
                    break;

                case Action.Pay_Commission:
                    break;

                case Action.Redistribution_of_Funds:
                    if (Access && !IsFrozen) Redistribution_of_Funds(info_Blank);
                    else throw new Exception($"Немає доступу до рахунку {Account_ID}");
                    break;

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
            Console.WriteLine($"Бізнес-рахунок {Account_ID}. Власник: {Organization_Name}. Баланс: {Amount_of_money:c}.");

            if(co_owners.Capacity > 0) Console.WriteLine("\nСписок співвласників: ");
            foreach (var res in co_owners)
            {
                Console.WriteLine($"{res.User.First_Name} {res.User.Last_Name}.\t Доля відповідальності: {res.Share_of_Responsibility * 100} %");
            }
        }

        protected override void Put_Money(Info_Blank info_Blank)
        {
            base.Put_Money(info_Blank);
        }

        protected override void Take_Money(Info_Blank info_Blank)
        {
            base.Take_Money(info_Blank);
        }

        public Business_Account(string name, Financial_issues _account_opened, Financial_issues _account_closed) : base(_account_opened, _account_closed)
        {
            Organization_Name = name;
            Balance_of_Liability = 1;
        }

        private void Add_Co_Owner(Account account, double share) // Робить спробу добавити співвласника з певною долею відповідальності, що визначає частку прибутку співвласника
        {
            try
            {
                if (Balance_of_Liability >= share)
                {
                    if (account is Depozit_Account depozit)
                    {
                        co_owners.Add(new Businessman(depozit.User, depozit, share));
                        Console.WriteLine($"Рахунок {depozit.Account_ID} став співвласником {Organization_Name} з долею відповідальності {share}.");
                        Balance_of_Liability -= share;
                    }
                    else
                    {
                        throw new Exception($"НЕ-депозитний рахунок {account.Account_ID} не може стати частиною бізнес-рахунку компанії {Organization_Name}.");
                    }
                }

                else
                {
                    if (Balance_of_Liability == 0) throw new Exception("Доля відповідальності вже розподілена між учасниками. Ви не можете добавити нового співвласника. ");
                    else throw new Exception($"Доля відповідальності нового учасника не може перевищувати {Balance_of_Liability}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Redistribution_of_Funds(Info_Blank info_Blank) // Перерозподіляє кошти рахунку між співвласниками, враховуючи їхні долі відповідальності
        {
            decimal Summ_for_Redistribution = Amount_of_money;

            foreach (Businessman acc in co_owners)
            {
                Select_Event(new Info_Blank(info_Blank.Bank_address, this, Summ_for_Redistribution * (decimal) acc.Share_of_Responsibility, is_null_Commision:info_Blank.NullCommission, to_account:acc.Account), Action.Transfer_Money);
            }
        }
        private struct Businessman
        {
            public User User { get; private init; }
            public Account Account { get; private init; }
            public double Share_of_Responsibility { get; private set; }

            /// <summary>
            /// Створює сутність <Businessman> на основі сутності <User>
            /// </summary>
            /// <param name="user"></param>
            /// <param name="account"></param>
            /// <param name="share"></param>
            public Businessman(User user, Account account, double share)
            {
                User = user;
                Account = account;
                Share_of_Responsibility = share;
            }
        }
    }
}
