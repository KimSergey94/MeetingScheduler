using MeetingScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Services
{
    class MeetingReminderTimer : System.Timers.Timer
    {
        public Meeting Meeting;
    }

    internal static class MeetingReminder
    {
        private static Dictionary<int, MeetingReminderTimer> MeetingReminders { get; set; } = new Dictionary<int, MeetingReminderTimer>();
        private static void OnElapsedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var meeting = ((MeetingReminderTimer)source).Meeting;
            PrintMeetingReminder(meeting);
            MeetingReminders.Remove(meeting.Id);
        }


        public static void AddAndStartMeetingReminder(Meeting meeting)
        {
            if (meeting.ReminderMinutes > 0)
            {
                MeetingReminderTimer timer = new MeetingReminderTimer();
                timer.Meeting = meeting;
                var test = (meeting.StartDate.AddMinutes(-1 * meeting.ReminderMinutes).Ticks - DateTime.Now.Ticks) / 10000;
                timer.Interval = test;
                timer.AutoReset = false;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnElapsedEvent);

                StopAndDeleteMeetingReminder(meeting);
                MeetingReminders.Add(meeting.Id, timer);
                MeetingReminders[meeting.Id].Start();
            }
        }
        public static void StopAndDeleteMeetingReminder(Meeting meeting)
        {
            if (MeetingReminders.ContainsKey(meeting.Id))
            {
                MeetingReminders[meeting.Id].Stop();
                MeetingReminders.Remove(meeting.Id);
            }
        }
        private static void PrintMeetingReminder(Meeting meeting)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("             ВНИМАНИЕ");
            Console.WriteLine("Напоминаем о запланированной встрече:");
            Console.WriteLine(meeting.ToString());
            Console.WriteLine("------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");
        }
    }
}
