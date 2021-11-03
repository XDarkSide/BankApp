using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bank_Transaction;
using bankAppCore = BankAppCore.BankApp_Core;

namespace BankApp
{
    class Bank : IBankActions
    {
        public enum acc_Type { Depozit = 1, Business, Credit, Bank }

        public string Bank_name { get; private init; }
        public Bank_Account Bank_account { get; private init; }
        public int Number_of_customers { get; private set; }
        public Account Selected_Acc { get; private set; }
        public double Amount_of_commission { get; set; }

        private double Annual_percentage { get; init; }

        private List<Account> accounts = new List<Account>();

        public Bank(string bank_name, double amount_of_commission, double annual_percentage, decimal start_up_capital)
        {
            Bank_name = bank_name;
            Bank_account = new Bank_Account(Add_Account, Close_Account, start_up_capital, this);
            Amount_of_commission = amount_of_commission;
            Annual_percentage = annual_percentage;
            Number_of_customers = 0;

            Bank_Timer bt = new Bank_Timer();
            bt.Start_Timer(this);
        }

        private void Add_Account(Info_Blank info_Blank) // Добавляє рахунок до банкової системи даних
        {
            accounts.Add(info_Blank.Selected_account);
        }

        private void Close_Account(Info_Blank info_Blank) // Вилучає рахунок з банкової системи даних
        {
            accounts.Remove(info_Blank.Selected_account);
            Number_of_customers--;

            Selected_Acc = null;
        }

        /// <summary>
        /// Відкриває певний тип рахунку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        public void Open_Account<T>(acc_Type type) where T : Account
        {
            T account;
            switch (type)
            {
                case acc_Type.Depozit:
                    account = new Depozit_Account(Annual_percentage, Add_Account, Close_Account) as T;
                    account.Select_Event(new Info_Blank(this,  account), Account.Action.Open_Account);
                    break;

                case acc_Type.Business:

                    Console.Write("Введіть назву компанії: ");
                    string company = Console.ReadLine();

                    account = new Business_Account(company, Add_Account, Close_Account) as T;
                    account.Select_Event(new Info_Blank(this, account), Account.Action.Open_Account);
                    break;

                case acc_Type.Credit:
                    decimal summ = bankAppCore.Write_Summ();
                    string ToAcc = bankAppCore.Write_ID();

                    Account for_acc = SeachrAcc(ToAcc, out bool find);

                    if (find)
                    {
                        account = new Credit_Account(summ, 0.1, this, Add_Account, Close_Account) as T;
                        Bank_account.Select_Event(new Info_Blank(this, Bank_account, summ, for_acc, true), Account.Action.Transfer_Money);
                        account.Select_Event(new Info_Blank(this, account), Account.Action.Open_Account);
                    }
                    break;
            }

            Number_of_customers++;
        }

        /// <summary>
        /// Закриває вибраний рахунок
        /// </summary>
        public void Close_Account()
        {
            if (Selected_Acc != null) Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc), Account.Action.Close_Account);
            else 
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Поповнює рахунок на певну кількість валюти
        /// </summary>
        /// <param name="summ"></param>
        public void Add_Money(decimal summ)
        {
            if (Selected_Acc != null) Selected_Acc.Select_Event(new Info_Blank(this, null, summ, Selected_Acc), Account.Action.Add_Money);
            else
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Знміє певну суму валюти з рахунку
        /// </summary>
        /// <param name="summ"></param>
        public void Withdraw_Money(decimal summ)
        {
            if (Selected_Acc != null)  Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc, summ, null, true), Account.Action.Withdraw_Money);
            else 
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Переводить валюту з одного рахунку на інший
        /// </summary>
        /// <param name="To"></param>
        /// <param name="summ"></param>
        public void Transfer_money(string To, decimal summ)
        {
            Account acc = SeachrAcc(To, out bool find);

            if (find && Selected_Acc != null) Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc, summ, acc, false), Account.Action.Transfer_Money);
            else
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Добавляєє співвласника до бізнес-рахунку
        /// </summary>
        /// <param name="co_owner"></param>
        /// <param name="share"></param>
        public void Add_Co_Owner(string co_owner, double share)
        {
            Account acc = SeachrAcc(co_owner, out bool find);

            if (find && Selected_Acc !=null) Selected_Acc.Select_Event(new Info_Blank(this, acc, share:share), Account.Action.Add_Co_Owner);
            else
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Реалізує разову виплату коштів з бізнес-рахунку співвласникам
        /// </summary>
        /// <param name="ac"></param>
        public void Redistribution_of_Funds(string ac)
        {
            if (Selected_Acc != null && Selected_Acc is Business_Account)
            {
                Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc, is_null_Commision: true), Account.Action.Redistribution_of_Funds);
            }

            else
            {
                Console.WriteLine($"Ви не ввійшли в свій рахунок!");
            }
        }

        /// <summary>
        /// Вибирає рахунок з банківської системи даних
        /// </summary>
        /// <param name="ID"></param>
        public void Select_Account(string ID)
        {
            Account acc = SeachrAcc(ID, out bool find);

            if (find)
            {
                Selected_Acc = acc;
                Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc), Account.Action.Log_in_to_Account);
            }
        }

        /// <summary>
        /// Припиняє доступ до вибраного рахунку
        /// </summary>
        public void Exit_Account()
        {
            Selected_Acc.Select_Event(new Info_Blank(this, Selected_Acc), Account.Action.Exit_from_Account);
            Selected_Acc = null;
        }

        private Account SeachrAcc(string ID, out bool find) /// Пошук рахунку в системі даних банку
        {
            try
            {
                foreach (Account acc in accounts)
                {
                    if (acc.Account_ID == ID)
                    {
                        find = true;
                        return acc;
                    }
                }
                throw new Exception($"рахунок {ID} не знайдено в банку {Bank_name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                find = false;
                return null;
            }
        }

        public void Control() /// Цей метод контролює виплату відсотків, збільшення суми боргу та запис транзакцій; списку рахунків - Запускається щохвилини
        {
            foreach (Account acc in accounts)
            {
                if (acc is Depozit_Account da)
                {
                    Bank_account.Select_Event(new Info_Blank(this, Bank_account, da.Calculate_profit(), da), Account.Action.Transfer_Money);
                }

                if (acc is Credit_Account ca)
                {
                    ca.Calculate_Debt();
                }
            }

            Transaction.Write_Accounts(accounts, this);
        }
    }

    class Info_Blank // Бланк даних для фінансових та організаційних операцій 
    {
        public Bank Bank_address { get; private init; }
        public Account Selected_account { get; private init; }
        public Account To_Acc { get; private init; }
        public decimal Summ { get; private init; }
        public bool NullCommission { get; private init; }
        public double Share { get; private init; }

        public bool Succes;
        

        public Info_Blank(Bank bank, Account selected_account, decimal summ = 0, Account to_account = null, bool is_null_Commision = false, double share = 0)
        {
            Bank_address = bank;
            Selected_account = selected_account;
            To_Acc = to_account;
            Summ = summ;
            Succes = true;
            NullCommission = is_null_Commision;
            Share = share;
        }
    }

    class Bank_Timer // Внутрішній таймер банку, асинхронно запускає Bank.Control() раз в хвилину
    {
        Time local_time = new Time();
        Bank bank;

        public void Start_Timer(Bank b)
        {
            bank = b;
            TimerCallback tc = new TimerCallback(Counter);
            Timer tm = new Timer(tc, local_time, 0, 1000);
        }

        private async void Counter(object time)
        {
            Time temp_time = (Time)time;

            temp_time.AddSeconds(1);

            if (temp_time.Seconds == 60)
            {
                temp_time.AddMinutes(1);
                temp_time.AddSeconds(-60);

                await Task.Run(() => bank.Control());

            }

            if (temp_time.Minutes == 60)
            {
                temp_time.AddHours(1);
                temp_time.AddMinutes(-60);
            }
        }

        class Time
        {
            public int Seconds;
            public int Minutes;
            public int Hours;

            public Time(int hours, int minuter, int seconds)
            {
                Hours = hours;
                Minutes = minuter;
                Seconds = seconds;
            }

            public Time() { }

            public void AddHours(int hours)
            {
                Hours += hours;
            }

            public void AddMinutes(int minuter)
            {
                Minutes += minuter;
            }

            public void AddSeconds(int seconds)
            {
                Seconds += seconds;
            }

            public void AddTime(Time time)
            {
                Hours += time.Hours;
                Minutes += time.Minutes;
                Seconds += time.Seconds;
            }

            public void Show_Time()
            {
                Console.Write($"\r{Hours:0#}:{Minutes:0#}:{Seconds:0#}");
            }
        }
    }
}
