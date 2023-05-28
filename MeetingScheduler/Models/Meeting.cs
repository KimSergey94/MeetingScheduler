using MeetingScheduler.Interfaces;
using MeetingScheduler.Services;
using MeetingScheduler.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Models
{
    internal class Meeting : IPrototype<Meeting>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ReminderMinutes { get; set; }


        public Meeting(string name, DateTime startDate, DateTime endDate, int reminderMinutes)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            ReminderMinutes = reminderMinutes;
        }
        public bool IsCorrect()
        {
            return StartDate < EndDate;
        }
        public bool DoesFitSchedule()
        {
            return MeetingManager.DoesMeetingFitSchedule(this);
        }

        public Meeting CreateDeepCopy()
        {
            var meeting = (Meeting)MemberwiseClone();
            return meeting;
        }

        public override string ToString()
        {
            var result = $"*********************************************\n" +
                $"Встреча #{Id}. {Name}\n" +
                $"Время начала: {StartDate.ParseToString()}\n" +
                $"Примерно время окончания: {EndDate.ParseToString()}\n";
            if (ReminderMinutes > 0) result += $"Напоминание за {ReminderMinutes} минут\n";
            result += "*********************************************\n";
            return result;
        }
    }
}
