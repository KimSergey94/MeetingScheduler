using MeetingScheduler.Models;
using MeetingScheduler.Services;
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
        public static void StartMenu()
        {
            PrintGreeting();
            PrintMainMenu();
        }
        private static void PrintGreeting()
        {
            Console.WriteLine("Добро пожаловать в персональный планировщик встреч.");
        }
        private static void PrintMainMenu()
        {
            PrintMainMenuOptions();
            var input = GetUserInput();
            Console.WriteLine();

            if (input == "1")
                PrintSchedulerMenu(DateTime.Now);
            else if (input == "2")
                ShowParticularDayMeetings();
            else if (input == "3")
                Environment.Exit(0);
            else
            {
                Console.WriteLine($"Произошла ошибка. Опция '{input}' не валидна.");
                Console.ReadKey();
                Console.WriteLine();
                PrintMainMenu();
            }
        }
        private static void ShowParticularDayMeetings()
        {
            Console.WriteLine("Пожалуйста введите день за который хотите посмотреть встречи. Формат: 27.05.2023");
            var meetingsDateString = GetUserInput();
            try
            {
                var meetingsDateTime = meetingsDateString.TryParseToDate();
                var filteredMeetings = MeetingManager.GetMeetingsByDate(meetingsDateTime);
                filteredMeetings.ForEach(meeting => {
                    PrintMeetingDetails(meeting);
                });
                Console.ReadKey();
                Console.WriteLine();
                AskToExitOrMainMenu();
            }
            catch
            {
                Console.WriteLine("Неправильный формат даты.");
                Console.ReadKey();
                Console.WriteLine();
                ShowParticularDayMeetings();
            }
        }
        private static void PrintMeetingDetails(Meeting meeting)
        {
            Console.WriteLine("*******************************");
            Console.WriteLine($"Встреча #{meeting.Id}. {meeting.Name}");
            Console.WriteLine($"Время начала: {meeting.StartDate.ParseToString()}");
            Console.WriteLine($"Примерно время окончания: {meeting.EndDate.ParseToString()}");
            if(meeting.ReminderMinutes > 0) Console.WriteLine($"Напоминание за {meeting.ReminderMinutes} минут");
            Console.WriteLine("*******************************");
        }
        private static void PrintSchedulerMenu(DateTime date)
        {
            PrintSchedulerMenuOptions(date);
            var input = GetUserInput();
            Console.WriteLine();

            if (input == "1") StartMeetingCreation(date);
            else if (input == "2") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.DayOfMonth));
            else if (input == "3") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.Month));
            else if (input == "4") PrintSchedulerMenu(ChangeSchedulerDate(date, DateChangeOptionEnum.Year));
            else if (input == "5") PrintMainMenu();
            else
            {
                Console.WriteLine($"Произошла ошибка. Опция '{input}' не валидна.");
                Console.ReadKey();
                Console.WriteLine();
                PrintSchedulerMenu(date);
            }
        }
        private static void StartMeetingCreation(DateTime startDate)
        {
            var name = AskMeetingName();
            startDate = InitMeetingDateAndTime(startDate);
            var endDate = InitMeetingEndDateAndTime();
            var reminderMinutes = AskToSetReminder();
            if (reminderMinutes > 0) Console.WriteLine("Reminder is created succsueussuffultlltlyy");

            var newMeeting = new Meeting(name, startDate, endDate, reminderMinutes);
            MeetingManager.AddMeeting(newMeeting);
            Console.WriteLine("Встреча успешно создана.");
            Console.ReadKey();
            Console.WriteLine();

            AskToExitOrMainMenu();
        }
        private static void AskToExitOrMainMenu()
        {
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine("[1] Вернуться в главное меню");
            Console.WriteLine("[2] Выход");
            var input = GetUserInput();
            Console.WriteLine();

            if (input == "1") PrintMainMenu();
            else if (input == "2") Environment.Exit(0);
            else
            {
                Console.WriteLine($"Произошла ошибка. Опция '{input}' не валидна.");
                Console.ReadKey();
                Console.WriteLine();
                AskToExitOrMainMenu();
            }
        }
        private static int AskToSetReminder()
        {
            var result = 0;
            Console.WriteLine("Пожалуйста введите за сколько минут до встречи вам отправить напоминание. Введите 0 если напоминание не нужно.");
            var input = GetUserInput();
            if (input == "0") return result;
            try
            {
                result = int.Parse(input);
            }
            catch
            {
                Console.WriteLine("Неправильный формат даты.");
                Console.WriteLine();
                result = AskToSetReminder();
            }
            return result;
        }
        private static DateTime InitMeetingEndDateAndTime()
        {
            Console.WriteLine("Пожалуйста введите примерное время окончания встречи. Формат: 27.05.2023 13:12");
            var endDateTimeString = GetUserInput();
            try
            {
                var endDateTime = endDateTimeString.TryParseToDateTime();
                return endDateTime;
            }
            catch
            {
                Console.WriteLine("Неправильный формат даты.");
                Console.WriteLine();
                InitMeetingEndDateAndTime();
            }
            return DateTime.Now;
        }
        private static DateTime InitMeetingDateAndTime(DateTime startDate)
        {
            Console.WriteLine("Пожалуйста введите планируемое время встречи. Формат: 13:12");
            var startTimeString = GetUserInput();
            try
            {
                startDate = startDate.AddTimeToDate(startTimeString);
            }
            catch
            {
                Console.WriteLine("Неправильный формат времени.");
                Console.WriteLine();
                InitMeetingDateAndTime(startDate);
            }
            return startDate;
        }
        private static string AskMeetingName()
        {
            Console.WriteLine("Пожалуйста введите название встречи:");
            return GetUserInput();
        }
        public static string GetUserInput()
        {
            var input = Console.ReadLine();
            return input.TrimConsoleInput();
        }
        private static DateTime ChangeSchedulerDate(DateTime date, DateChangeOptionEnum dateChangeOption)
        {
            Console.WriteLine("Пожалуйста введите нужное значение:");
            var input = GetUserInput().AdjustDateInputFormat(dateChangeOption);
          
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
                date = dateString.TryParseToDate();
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
            Console.WriteLine("[5] Вернуться в главное меню");
        }
        private static void PrintMainMenuOptions()
        {
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine("[1] Запланировать встречу");
            Console.WriteLine("[2] Просмотр запланированных встреч на определенный день");
            Console.WriteLine("[3] Выход");
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
        public static string TrimConsoleInput(this string? input)
        {
            return input != null ? input.Trim() : "";
        }
        public static DateTime TryParseToDate(this string dateString) 
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateString, "dd.MM.yyyy", provider);
        }
        public static DateTime AddTimeToDate(this DateTime date, string time)
        {
            var dateString = date.ToString("dd.MM.yyyy") + $" {time}";
            CultureInfo provider = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateString, "dd.MM.yyyy H:mm", provider);
        }
        public static DateTime TryParseToDateTime(this string dateTime)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            return DateTime.ParseExact(dateTime, "dd.MM.yyyy H:mm", provider);
        }
        public static string ParseToString(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy H:mm");
        }
    }
}
