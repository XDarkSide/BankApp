using System;

namespace BankApp
{
    struct User
    {
        public string First_Name { get; private init; }
        public string Last_Name { get; private init; }

        private User(string first_name, string last_name)
        {
            First_Name = first_name;
            Last_Name = last_name;
        }

        /// <summary>
        /// Динамічно створює нового користувача
        /// </summary>
        /// <returns></returns>
        public static User Create_User()
        {
            Console.Write("Введіть ваше ім'я: ");
            string input_first_name = Console.ReadLine();

            Console.Write("Введіть ваше прізвище: ");
            string input_last_name = Console.ReadLine();

            return new User(input_first_name, input_last_name);
        }
    }
}
