using System;
using BankApp;
using System.IO;
using System.Collections.Generic;

namespace Bank_Transaction
{
    static class Transaction
    {
        private static int counter;
        static Transaction()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Bank_info");
            directoryInfo.Create();

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            counter = 0;
        }

        /// <summary>
        /// записує дані про транзакцію в файлову систему
        /// </summary>
        /// <param name="info_Blank"></param>
        public static void Write_Transaction(Info_Blank info_Blank)
        {
            if (info_Blank.Succes || info_Blank.Summ != 0)
            {
                using (StreamWriter streamWriter = new StreamWriter(@"C:\Bank_info\Bank_Transactions.txt", true, System.Text.Encoding.Default))
                {
                    streamWriter.WriteLine($"З {info_Blank.Selected_account.Account_ID} до {info_Blank.To_Acc.Account_ID} переведено {info_Blank.Summ:c}. Успішно: {info_Blank.Succes}.");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"\nТранзакція {counter} записана в файлову систему. ");
                    Console.ResetColor();

                    counter++;
                }
            }
        }

        /// <summary>
        /// записує дані про рахунки в файлову систему для кожного банку окремо
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="bank"></param>
        public static void Write_Accounts(List<Account> accounts, Bank bank)
        {
            using (StreamWriter streamWriter = new StreamWriter($"C:/Bank_info/{bank.Bank_name}_Account.txt", false, System.Text.Encoding.Default))
            {
                foreach (Account account in accounts)
                {
                    streamWriter.WriteLine($"Рахунок : {account.Account_ID}. Баланс : {account.Amount_of_money:c}.");
                }
            }
        }
    }
}
