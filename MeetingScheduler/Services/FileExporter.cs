using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Services
{
    internal static class FileExporter
    {
        public static void SaveToFile(string content) 
        {
            using (StreamWriter writetext = new StreamWriter("schedule.txt"))
            {
                writetext.WriteLine(content);
            }
        }
    }
}
