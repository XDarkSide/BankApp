using System;
using Bank_Transaction;

namespace BankApp
{
    abstract class Account
    {
        public enum Action { Log_in_to_Account = 1, Exit_from_Account, Open_Account, Close_Account, Add_Money, Withdraw_Money, Transfer_Money, Redistribution_of_Funds, Add_Co_Owner, Back_To_Bank_Select, Pay_Commission };

        public string Account_ID { get; private init; }
        public bool Access { get; private set; }
        public decimal Amount_of_money { get; protected set; }
        protected bool IsFrozen { get; set; }
        private Login_Password Acc_Data { get; init; }


        public delegate void Organizational_issues();
        public delegate void Financial_issues (Info_Blank info_Blank);

        private event Organizational_issues login_completed;
        private event Organizational_issues exit_completed;
        private event Organizational_issues account_frozen;

        protected event Financial_issues account_opened;
        protected event Financial_issues account_closed;

        private event Financial_issues money_added;
        private event Financial_issues money_withdrawn;
        private event Financial_issues money_transferred;

        protected event Financial_issues commission_paid;

        protected void UseEvent(Organizational_issues ev) // Запуск події
        {
            if (ev != null) ev.Invoke();
        }

        protected void UseEvent(Financial_issues ev, Info_Blank info_Blank) // Перевантажений варіант запуску події
        {
            if (ev != null) ev.Invoke(info_Blank);
        }

        protected Account(Financial_issues _account_opened, Financial_issues _account_closed)
        {
            account_opened += _account_opened;
            account_opened += (Info_Blank Info_Blank) => Console.WriteLine($"Рахунок {Account_ID} відкрито. ");

            account_closed += _account_closed;
            account_closed += (Info_Blank Info_Blank) => Console.WriteLine($"Рахунок {Account_ID} закрито. ");

            account_frozen += Freeze;
            account_frozen += () => Console.WriteLine($"Кількість спроб вводу вичерпано. Рахунок {Account_ID} заморожено.\n ");

            login_completed += Log_in;
            login_completed += () =>
            {
                if (Access) Console.WriteLine($"Доступ до {Account_ID} дозволено.\n ");
            };

            exit_completed += Exit;
            exit_completed += () => Console.WriteLine($"Доступ до {Account_ID} припинено.\n ");

            money_added += Put_Money;
            money_withdrawn += Take_Money;
            money_transferred += Transfer_Money;
            money_transferred += Transaction.Write_Transaction;
            commission_paid += Pay_Commision;

            Account_ID = Create_ID();
            Acc_Data = new Login_Password("User", "1234");
        }

        private string Create_ID() // Створює унікальний шестизначний (3 англ. літери + 3 цифри) ідентифікатор
        {
            const int Lenght = 6;
            char[] charray = new char[Lenght];
            Random rnd = new Random();

            for (int i = 0; i < Lenght; i++)
            {
                charray[i] = i <= 2 ? (char)rnd.Next(65, 91) : (char)rnd.Next(48, 58);
            }

            return new string(charray);
        }

        protected virtual void Put_Money(Info_Blank info_Blank) // Добавляє кошти на рахунок
        {
            if (info_Blank.Summ > 0 && info_Blank.Succes)
            {
                Amount_of_money += info_Blank.Summ;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\nРахунок {info_Blank.To_Acc.Account_ID} поповнено на {info_Blank.Summ:c}. Баланс: {info_Blank.To_Acc.Amount_of_money:c}. \n");
                Console.ResetColor();
            }
        }
        protected virtual void Take_Money(Info_Blank info_Blank) // Знімає кошти з рахунку
        {
            if (Amount_of_money >= info_Blank.Summ + Commission(info_Blank))
            {
                Amount_of_money -= info_Blank.Summ;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nЗ рахунку {info_Blank.Selected_account.Account_ID} знято {info_Blank.Summ:c}. Баланс: {info_Blank.Selected_account.Amount_of_money:c}. ");
                Console.ResetColor();
            }

            else
            {
                Console.WriteLine($"Для здійснення операції недостатньо коштів на рахунку {info_Blank.Selected_account.Account_ID}");
                info_Blank.Succes = false;
            }
        }

        protected static void Transfer_Money(Info_Blank info_Blank) // Переводить кошти з одного рахунку на інший, інкапсулює Put_Money() і Take_Money()
        {
            info_Blank.Selected_account.Take_Money(info_Blank);

            if (info_Blank.Succes)
            {
                info_Blank.To_Acc.Put_Money(info_Blank);
                if(!info_Blank.NullCommission) info_Blank.Selected_account.Select_Event(new Info_Blank(info_Blank.Bank_address, info_Blank.Selected_account, info_Blank.Selected_account.Commission(info_Blank), info_Blank.Bank_address.Bank_account ), Action.Pay_Commission);
            }
        }
        public abstract void Select_Event(Info_Blank info_Blank, Action action);
        public abstract void Show_Account();

        private void Freeze() => IsFrozen = true; // Заморожує рахунок, коли спроби вводу вичерпано

        private void Log_in() // Дозволяє взаємодію з рахунком після авторизації
        {
            string input_login, input_password;

            while (!IsFrozen && !Access)
            {

                Console.Write("Введіть логін: ");
                input_login = Console.ReadLine();

                Console.Write("Введіть пароль: ");
                input_password = Console.ReadLine();

                Acc_Data.Authorization(this, input_login, input_password);
            }

            if (IsFrozen) Console.WriteLine($"Рахунок {Account_ID} заморожено. Спроби авторизації неможливі. ");
        }

        private void Exit() => Access = false; // Припиняє доступ до рахунку

        protected void Pay_Commision(Info_Blank info_Blank) // Переводить суму комісії банку
        {
            info_Blank.To_Acc.Put_Money(info_Blank);
            info_Blank.Selected_account.Take_Money(info_Blank);
        }

        protected void Select_Base_Event(Info_Blank info_Blank, Action action) // Вибирає подію, інкапсулюється в Select_Event()
        {
            try
            {
                switch (action)
                {
                    case Action.Log_in_to_Account:
                        UseEvent(login_completed);
                        break;

                    case Action.Exit_from_Account:
                        UseEvent(exit_completed);
                        break;

                    case Action.Open_Account:
                        UseEvent(account_opened, info_Blank);
                        break;

                    case Action.Close_Account:
                        if (Access && !IsFrozen) UseEvent(account_closed, info_Blank);
                        else throw new Exception($"Немає доступу до рахунку {Account_ID}");
                        break;

                    case Action.Add_Money:
                        UseEvent(money_added, info_Blank);
                        break;

                    case Action.Withdraw_Money:
                        if (Access && !IsFrozen) UseEvent(money_withdrawn, info_Blank);
                        else throw new Exception($"Немає доступу до рахунку {Account_ID}");
                        break;

                    case Action.Transfer_Money:
                        if ((Access && !IsFrozen) || info_Blank.Selected_account is Bank_Account) UseEvent(money_transferred, info_Blank);
                        else throw new Exception($"Немає доступу до рахунку {Account_ID}");
                        break;

                    case Action.Pay_Commission:
                        UseEvent(commission_paid, info_Blank);
                        break;

                    default:  throw new Exception("Рахунок не може обробити дію такого типу. ");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected decimal Commission(Info_Blank info_Blank) // Розраховує суму комісії, якщо info_Blank.NullCommission істинно - комісія не сплачується
        {
            decimal commision;

            commision = !info_Blank.NullCommission ? (info_Blank.Summ * (decimal)info_Blank.Bank_address.Amount_of_commission) : 0;

            return commision;
        }

        ~Account() // автоматичне управління пам'яттю, збиральник сміття
        {
            Console.WriteLine($"Доступ до {Account_ID} остаточно втрачено. ");
        }

        private class Login_Password
        {
            private string Login { get; set; }
            private string Password { get; set; }
            public int Before_freezing { get; private set; }

            public Login_Password(string login, string password)
            {
                Login = login;
                Password = password;

                Before_freezing = 3;
            }

            /// <summary>
            /// Авторизує користувача в його рахунку
            /// </summary>
            /// <param name="acc"></param>
            /// <param name="input_login"></param>
            /// <param name="input_password"></param>
            public void Authorization(Account acc, string input_login, string input_password)
            {
                if (input_login != acc.Acc_Data.Login || input_password != acc.Acc_Data.Password)
                {
                    acc.Access = false;
                    acc.Acc_Data.Before_freezing--;

                    if (acc.Acc_Data.Before_freezing <= 0) acc.UseEvent(acc.account_frozen);
                }

                else
                {
                    acc.Access = true;
                }
            }
        }
    }
}
