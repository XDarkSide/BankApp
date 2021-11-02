namespace BankApp
{
    interface IBankActions // описує основний функціонал банків
    {
        void Open_Account<T>(Bank.acc_Type type) where T : Account;
        void Close_Account();

        void Add_Money(decimal summ);
        void Withdraw_Money(decimal summ);
        void Transfer_money(string To, decimal summ);
    }
}
