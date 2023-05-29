using MeetingScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Services
{
    internal static class MeetingManager
    {
        private static List<Meeting> meetings { get; set; } = new List<Meeting>();
        private static int indexCounter = 0;

        public static void AddMeeting(Meeting meeting) 
        { 
            meeting.Id = ++indexCounter; 
            meetings.Add(meeting);
        }

        /// <returns>
        /// 0 если произошла ошибка при удалении со списка
        /// 1 если удаление успешно
        /// </returns>
        public static int RemoveMeeting(Meeting meeting) 
        {
            var result = 0;
            try
            {
                meetings.Remove(meeting);
                result++;
            }
            catch { }
            return result;
        }

        /// <returns>
        /// 0 если не найдена встреча с id объекта параметра
        /// 1 если редактирование успешно
        /// </returns>
        public static int EditMeeting(Meeting meeting) 
        { 
            var result = 0;
            var originalMeetingObj = meetings.FirstOrDefault(x => x.Id == meeting.Id);
            if (originalMeetingObj != null)
            {
                originalMeetingObj.ReminderMinutes = meeting.ReminderMinutes;
                originalMeetingObj.Name = meeting.Name;
                originalMeetingObj.StartDate = meeting.StartDate;
                originalMeetingObj.EndDate = meeting.EndDate;
                result = 1;
            }
            return result;
        }

        public static List<Meeting> GetMeetingsByDate(DateTime meetingsDate)
        {
            return meetings.Where(x => x.StartDate.Date == meetingsDate.Date).ToList();
        }
        public static Meeting GetMeetingById(int meetingId)
        {
            return meetings.FirstOrDefault(x => x.Id == meetingId);
        }

        public static bool DoesMeetingFitSchedule(Meeting meeting)
        {
            return !meetings.Any(x =>
                               (x.StartDate < meeting.StartDate && meeting.StartDate < x.EndDate)
                               ||
                               (x.StartDate < meeting.EndDate && meeting.EndDate <= x.EndDate)
                               ||
                               (meeting.StartDate < x.StartDate && x.StartDate < meeting.EndDate)
                               ||
                               (meeting.StartDate < x.EndDate && x.EndDate <= meeting.EndDate)
                               );
        }
        public static void ExportAsTextFile(List<Meeting> meetings)
        {
            var content = "";
            meetings.ForEach(x =>
            {
                content += x.ToString();
            });
            FileExporter.SaveToFile(content);
        }
    }
}
