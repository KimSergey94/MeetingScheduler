using MeetingScheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Models
{
    internal class Meeting
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
    }
}
