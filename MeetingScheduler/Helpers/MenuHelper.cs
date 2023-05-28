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
        #region Main menu
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

            if (input == "1") PrintSchedulerMenu(DateTime.Now);
            else if (input == "2") ShowParticularDayMeetings();
            else if (input == "3") Environment.Exit(0);
            else
            {
                ShowMessage($"Произошла ошибка. Опция '{input}' не валидна.");
                PrintMainMenu();
            }
        }
        private static void PrintMainMenuOptions()
        {
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine("[1] Запланировать встречу");
            Console.WriteLine("[2] Просмотр запланированных встреч на определенный день");
            Console.WriteLine("[3] Выход");
        }
        #endregion

        #region Show meetings
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
                Console.WriteLine();
                AskIfChangesNeeded();
            }
            catch
            {
                ShowMessage("Неправильный формат даты.");
                ShowParticularDayMeetings();
            }
        }
        private static void AskIfChangesNeeded()
        {
            Console.WriteLine("Введите номер встречи если требуется внести изменения или удалить встречу. Введите 0 чтобы вернуться в главное меню.");
            var input = GetUserInput();
            if (input == "0") PrintMainMenu();
            try
            {
                var meetingNumber = int.Parse(input);
                if (meetingNumber > 0)
                {
                    try
                    {
                        StartMeetingEditing(meetingNumber);
                    }
                    catch
                    {
                        ShowMessage($"Произошла ошибка. Не удалось редактировать встречу с номером '{meetingNumber}'. Проверьте правильность данных.");
                        AskIfChangesNeeded();
                    }
                }
                else PrintMainMenu();
            }
            catch 
            {
                ShowMessage($"Произошла ошибка. Опция '{input}' не валидна.");
                AskIfChangesNeeded();
            }
        }
        private static void StartMeetingEditing(int meetingNumber)
        {
            var meeting = MeetingManager.GetMeetingById(meetingNumber);
            if (meeting != null) ShowMeetingEditingOptions(meeting);
            else throw new Exception($"не найдена встреча с номером {meetingNumber}");
        }
        private static void ShowMeetingEditingOptions(Meeting meeting)
        {
            PrintMeetingDetails(meeting);
            Console.WriteLine("Пожалуйста выберите нужную опцию:");
            Console.WriteLine("[1] Редактировать название встречи");
            Console.WriteLine("[2] Редактировать дату начала и окончания встречи");
            Console.WriteLine("[3] Редактировать за сколько минут до встречи отправить напоминание о встрече");
            Console.WriteLine("[4] Удалить встречу");
            Console.WriteLine("[5] Вернуться в главное меню");
            var input = GetUserInput();

            if (input == "1") HandleMeetingNameChange(meeting);
            else if (input == "2")
            {
                var meetingCopy = meeting.CreateDeepCopy();
                HandleMeetingDateChange(meeting, MeetingDateChangeOptionEnum.StartDate);
                HandleMeetingDateChange(meeting, MeetingDateChangeOptionEnum.EndDate);
                if (MeetingManager.DoesMeetingFitSchedule(meeting))
                {
                    ShowMessage($"Время встречи не пересекается с другими встречами и было успешно сохранено.");
                    ShowMeetingEditingOptions(meeting);
                }
                else
                {
                    meeting = meetingCopy;
                    ShowMessage($"Произошла ошибка. Время встречи пересекается с другой/другими встречами. Произведен откат изменений дат начала и окончания встречи. Попробуйте еще раз.");
                    ShowMeetingEditingOptions(meeting);
                }
            }
            else if (input == "3") HandleMeetingReminderChange(meeting);
            else if (input == "4")
            {
                if (MeetingManager.RemoveMeeting(meeting) > 0) ShowMessageAndAskToExitOrMainMenu("Встреча успешно удалена.");
                else ShowMessageAndAskToExitOrMainMenu($"Произошла ошибка. Не удалось удалить встречу с номером {meeting.Id}. Проверьте правильность данных.");
            }
            else if (input == "5") PrintMainMenu();
            else
            {
                ShowMessage($"Произошла ошибка. Опция '{input}' не валидна.");
                ShowMeetingEditingOptions(meeting);
            }
        }
        private static void HandleMeetingReminderChange(Meeting meeting)
        {
            Console.WriteLine("Пожалуйста введите за сколько минут до встречи нужно отправить напоминание:");
            var input = GetUserInput();

            try
            {
                var reminderMinutes = int.Parse(input);

                if(meeting.StartDate.AddMinutes(Math.Abs(reminderMinutes)) >= DateTime.Now.AddMinutes(1))
                {
                    meeting.ReminderMinutes = reminderMinutes;
                    ShowMessage("Дата напоминания успешно отредактирована.");
                    ShowMeetingEditingOptions(meeting);
                }
                else
                {
                    ShowMessage("Дата напоминания должна быть позже текущего времени системы минимум на одну минуту.");
                    HandleMeetingReminderChange(meeting);
                }
                
            }
            catch
            {
                ShowMessage("Неправильный формат ввода.");
                HandleMeetingReminderChange( meeting);
            }
        }
        private static void HandleMeetingDateChange(Meeting meeting, MeetingDateChangeOptionEnum meetingDateChangeOptionEnum)
        {
            var meetingChangeDateTitle = meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.StartDate ? "дату начала" : meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.EndDate ? "примерную дату окончания" : "";
            Console.WriteLine($"Пожалуйста введите новую {meetingChangeDateTitle} встречи. Формат: 27.05.2023 13:12");
            var dateString = GetUserInput();
            try
            {
                var dateTime = dateString.TryParseToDateTime();
                if(meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.StartDate)
                {
                    if(dateTime >= DateTime.Now.AddMinutes(10)) meeting.StartDate = dateTime;
                    else
                    {
                        ShowMessage("Дата начала встречи должна быть позже текущего времени системы минимум на 10 минут.");
                        HandleMeetingDateChange(meeting, meetingDateChangeOptionEnum);
                        return;
                    }
                }
                else if (meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.EndDate)
                {
                    if (dateTime >= meeting.StartDate.AddMinutes(1)) meeting.EndDate = dateTime;
                    else
                    {
                        ShowMessage("Примерная дата окончания встречи должна быть позже даты начала встречи минимум на одну минуту.");
                        HandleMeetingDateChange(meeting, meetingDateChangeOptionEnum);
                        return;
                    }
                }
                var messageChangeDateTitle = "";
                if (meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.StartDate) messageChangeDateTitle = "Дата начала";
                else if(meetingDateChangeOptionEnum == MeetingDateChangeOptionEnum.EndDate) messageChangeDateTitle = "Примерная дата окончания";
                ShowMessage($"{messageChangeDateTitle} встречи успешно отредактирована.");
            }
            catch
            {
                ShowMessage("Неправильный формат даты.");
                HandleMeetingDateChange(meeting, meetingDateChangeOptionEnum);
            }
        }
        private static void HandleMeetingNameChange(Meeting meeting)
        {
            Console.WriteLine("Пожалуйста введите новое название:");
            var input = GetUserInput();
            meeting.Name = input;

            ShowMessage("Название встречи успешно отредактировано.");
            ShowMeetingEditingOptions(meeting);
                
            //if (MeetingManager.EditMeeting(meeting) > 0)
            //{
            //    ShowMessageAndAskToExitOrMainMenu("Встреча успешно отредактирована.");
            //}
            //else
            //{
            //    ShowMessageAndAskToExitOrMainMenu($"Произошла непредвиденная ошибка. Не удалось отредактирова встречу под номером {meeting.Id}.");
            //}
        }
        private static void PrintMeetingDetails(Meeting meeting)
        {
            Console.WriteLine(meeting.ToString());
        }
        #endregion

        #region Scheduling meeting
        private static void PrintSchedulerMenu(DateTime date)
        {
            PrintSchedulerMenuOptions(date);
            var input = GetUserInput();

            if (input == "1") StartMeetingCreation(date);
            else if (input == "2") PrintSchedulerMenu(ChangeSchedulerDate(date, DatePartEnum.DayOfMonth));
            else if (input == "3") PrintSchedulerMenu(ChangeSchedulerDate(date, DatePartEnum.Month));
            else if (input == "4") PrintSchedulerMenu(ChangeSchedulerDate(date, DatePartEnum.Year));
            else if (input == "5") PrintMainMenu();
            else
            {
                ShowMessage($"Произошла ошибка. Опция '{input}' не валидна.");
                PrintSchedulerMenu(date);
            }
        }
        private static void StartMeetingCreation(DateTime startDate)
        {
            startDate = InitMeetingDateAndTime(startDate);
            var endDate = InitMeetingEndDateAndTime();
            var newMeeting = new Meeting("temp", startDate, endDate, 0);
            if (MeetingManager.DoesMeetingFitSchedule(newMeeting))
            {
                var name = AskMeetingName();
                var reminderMinutes = AskToSetReminder();
                newMeeting.ReminderMinutes = reminderMinutes;
                newMeeting.Name = name;
                MeetingManager.AddMeeting(newMeeting);
                ShowMessageAndAskToExitOrMainMenu("Встреча успешно создана.");
            }
            else
            {
                ShowMessage($"Произошла ошибка. Время встречи пересекается с другой/другими встречами. Попробуйте еще раз.");
                StartMeetingCreation(startDate);
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
                Console.WriteLine("Неправильный формат ввода.");
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
        private static DateTime ChangeSchedulerDate(DateTime date, DatePartEnum dateChangeOption)
        {
            Console.WriteLine("Пожалуйста введите нужное значение:");
            var input = GetUserInput().AdjustDateInputFormat(dateChangeOption);
          
            try
            {
                var dateString = date.ToString("dd.MM.yyyy");
                var dateArray = dateString.Split('.');

                if (dateChangeOption == DatePartEnum.DayOfMonth)
                    dateArray[0] = input;
                else if (dateChangeOption == DatePartEnum.Month)
                    dateArray[1] = input;
                else if (dateChangeOption == DatePartEnum.Year)
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
        #endregion

        #region Common
        public static string GetUserInput()
        {
            var input = Console.ReadLine();
            Console.WriteLine();
            return input.TrimConsoleInput();
        }
        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
            Console.WriteLine();
        }
        private static void ShowMessageAndAskToExitOrMainMenu(string message)
        {
            Console.WriteLine(message);
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


            if (input == "1") PrintMainMenu();
            else if (input == "2") Environment.Exit(0);
            else
            {
                ShowMessageAndAskToExitOrMainMenu($"Произошла ошибка. Опция '{input}' не валидна.");
            }
        }
        #endregion
    }

    public enum DatePartEnum
    {
        DayOfMonth = 0, Month = 1, Year = 2,
    }
    public enum MeetingDateChangeOptionEnum
    {
        StartDate = 0, EndDate = 1,
    }

    public static class MenuHelperExtensions
    {
        public static string AdjustDateInputFormat(this string input, DatePartEnum dateChangeOption)
        {
            if (DatePartEnum.Year == dateChangeOption)
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
