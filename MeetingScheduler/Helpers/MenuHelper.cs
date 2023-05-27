using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Helpers
{
    internal static class MenuHelper
    {
        public static void PrintGreeting()
        {
            Console.WriteLine("Добро пожаловать.");
        }
        public static void PrintMainMenu()
        {
            PrintMainMenuOptions();
            var input = Console.ReadLine().Trim();
            Console.WriteLine();
          
            if (input == "1")
                PrintSchedulerMenu(DateTime.Now);
            else if(input == "2")
            {
            }
            else
            {
                Console.WriteLine($"Произошла ошибка. Опция '{input}' не валидна.");
                Console.ReadKey();
                Console.WriteLine();
                PrintMainMenu();
            }
        }
        public static void PrintSchedulerMenu(DateTime date)
        {
            PrintSchedulerMenuOptions(date);
            var input = Console.ReadLine().Trim();
            Console.WriteLine();

            if (input == "1") Console.WriteLine("asd");
            else if (input == "2") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.DayOfMonth));
            else if (input == "3") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.Month));
            else if (input == "4") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.Year));
        }
        public static DateTime ChangeSchedulerDate(DateTime date, DateChangeOptionEnum dateChangeOption)
        {
            Console.WriteLine("Пожалуйста введите нужное значение:");
            var input = Console.ReadLine().Trim();
            input = input.AdjustDateInputFormat(dateChangeOption);
          
            try
            {
                var dateString = date.ToString("dd.MM.yyyy");
                var dateArray = dateString.Split('.');

                if (dateChangeOption == DateChangeOptionEnum.DayOfMonth)
                    dateArray[0] = input;
                else if (dateChangeOption == DateChangeOptionEnum.Month)
                    dateArray[1] = input;
                else if (dateChangeOption == DateChangeOptionEnum.Year)
                    dateArray[2] = input;

                dateString = string.Join('.', dateArray);

                CultureInfo provider = CultureInfo.InvariantCulture;
                date = DateTime.ParseExact(dateString, "dd.MM.yyyy", provider);
                Console.WriteLine();
                return date;
            }
            catch {
                Console.WriteLine("Произошла ошибка при редактировании даты.");
                Console.WriteLine();
                return date;
            }
        }
        private static void PrintSchedulerMenuOptions(DateTime date)
        {
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine($"Дата для планирования встречи: {date.ToString("dd.MM.yyyy")} (день.месяц.год).");
            Console.WriteLine("[1] Выбрать указанную дату");
            Console.WriteLine("[2] Изменить день даты");
            Console.WriteLine("[3] Изменить месяц даты");
            Console.WriteLine("[4] Изменить год даты");
        }
        private static void PrintMainMenuOptions()
        {
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine("[1] Запланировать встречу");
            Console.WriteLine("[2] Просмотр запланированных встреч на определенный день");
        }
    }

    public enum DateChangeOptionEnum
    {
        DayOfMonth = 0, Month = 1, Year = 2,
    }

    public static class MenuHelperExtensions
    {
        public static string AdjustDateInputFormat(this string input, DateChangeOptionEnum dateChangeOption)
        {
            if (DateChangeOptionEnum.Year == dateChangeOption)
            {
                while (input.Length < 4)
                {
                    input = input.Insert(0, "0");
                }
            }
            else if (input.Length < 2)
            {
                input = input.Insert(0, "0");
            }
            return input;
        }
    }
}
