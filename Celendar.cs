using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace VkBot
{
    static class Celendar
    {
        static List<string> CelendarIds = new List<string>
        {
            //"7odtos4j6q45hannsd683ocebc@group.calendar.google.com",///9361
            //"br5luc5j1rj1p50u5h3cdb50p0@group.calendar.google.com",///9362
            //"hpg8lr9gcbni8k9n0s90b48rn4@group.calendar.google.com",///9363
            //"04vtqutvskn60n8o115q67b0v0@group.calendar.google.com",///Партия
            "crdsbrmk5e00qd1torsrqbq1ls@group.calendar.google.com" ///Ярик
        };
        public static void Initialize()
        {
            foreach (var item in CelendarIds) LoadCelendar(item);
        }
        public static void Writeline(string message)
        {
            if (File.Exists("Log.txt")) File.AppendAllText("Log.txt", DateTime.Now.ToString() + " " + message);
            else File.Create("Log.txt");
        }
        static void LoadCelendar(string Id)
        {
            try
            {
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyCVJfIBE2dpnhGCqGQWlyQoupQtZu-LDrs",
                    ApplicationName = "xyz",
                });
                var events = service.Events.List(Id).Execute();
                Program.reminders.ADD(events);
            }
            catch(Exception e)
            {
                Celendar.Writeline("CelenderGoogleError:" + e.Message + "\n");
            }
        }
    }
    public class Reminders : List<Events>
    {
        DateTime StartTime;
        public Reminders()
        {
            timer = new Timer { Interval = 1000 };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            StartTime = DateTime.Now;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        { 
            if (DateTime.Now.Day != StartTime.Day)
            {
                Clear();
                Celendar.Initialize();
            }
            for (int i = Count - 1; i >= 0; i--)
            {
                foreach (var item in this[i].Items.ToList())
                {
                    if (item.Start == null)
                    {
                        this[i].Items.Remove(item);
                        continue;
                    }
                    if (item.Start.DateTime.Value.TimeOfDay <= DateTime.Now.AddMinutes(15).TimeOfDay)
                    {
                        if (item.Start.DateTime.Value.TimeOfDay <= DateTime.Now.AddMinutes(-30).TimeOfDay) continue;
                        if (item.Recurrence == null) { if (item.Start.DateTime.Value.DayOfYear != DateTime.Now.DayOfYear) continue; }
                        else if (item.Start.DateTime.Value.DayOfWeek != DateTime.Now.DayOfWeek) continue;
                        NewRemind?.Invoke(null, new RemindEventArgs(item, this[i].Summary));
                        this[i].Items.Remove(item);
                    }
                }
            }
        }
        public event EventHandler<RemindEventArgs> NewRemind;
        private Timer timer;
        public void ADD(Events group) => Add(group);
    }
    public class RemindEventArgs : EventArgs
    {
        public RemindEventArgs(Event sender, string group) { this.sender = sender; this.group = group; }
        public Event sender;
        public string group;
    }
}
