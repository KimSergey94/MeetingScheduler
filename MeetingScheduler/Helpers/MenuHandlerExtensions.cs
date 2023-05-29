using MeetingScheduler.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Helpers
{
    public static class MenuHandlerExtensions
    {
        public static string AdjustFullDateInputFormat(this string fullDateString)
        {
            var fullDateStringArray = fullDateString.Split(' ');
            var dateString = fullDateStringArray[0];
            var timeString = fullDateStringArray[1];
            dateString = dateString.AdjustDateInputFormat();
            timeString = timeString.AdjustTimeInputFormat();
            return $"{dateString} {timeString}";
        }
        public static string AdjustTimeInputFormat(this string timeString)
        {
            var dateStringArray = timeString.Split(':');
            dateStringArray[0] = dateStringArray[0].AdjustDateInputFormat(StringDatePartEnum.DayOfMonth);
            dateStringArray[1] = dateStringArray[1].AdjustDateInputFormat(StringDatePartEnum.Month);
            return string.Join(':', dateStringArray);
        }
        public static string AdjustDateInputFormat(this string timeString)
        {
            var timeStringArray = timeString.Split('.');
            timeStringArray[0] = timeStringArray[0].AdjustDateInputFormat(StringDatePartEnum.DayOfMonth);
            timeStringArray[1] = timeStringArray[1].AdjustDateInputFormat(StringDatePartEnum.Month);
            timeStringArray[2] = timeStringArray[2].AdjustDateInputFormat(StringDatePartEnum.Year);
            return string.Join('.', timeStringArray);
        }
        public static string AdjustDateInputFormat(this string input, StringDatePartEnum dateChangeOption)
        {
            if (StringDatePartEnum.Year == dateChangeOption)
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
        public static DateTime ChangeDatePartByInput(this DateTime date, string input, StringDatePartEnum dateChangeOption)
        {
            var dateString = date.ToString("dd.MM.yyyy");
            var dateArray = dateString.Split('.');

            if (dateChangeOption == StringDatePartEnum.DayOfMonth)
                dateArray[0] = input;
            else if (dateChangeOption == StringDatePartEnum.Month)
                dateArray[1] = input;
            else if (dateChangeOption == StringDatePartEnum.Year)
                dateArray[2] = input;

            dateString = string.Join('.', dateArray);
            return dateString.TryParseToDate();
        }
    }
}
