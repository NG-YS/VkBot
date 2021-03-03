using System;

namespace VkBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Supported Commands: \n" +
                "AddUserID [UserGroup] [UserID]\n" +
                "ReloadCalendar\n" +
                "Tnks for use YoungSecretar");
            vkBot = new VkBot();
            reminders.NewRemind += Reminders_NewRemind;
            Celendar.Initialize();
            while(true)
            {
                string command = Console.ReadLine();
                var comparms = command.Split(' ');
                switch (comparms[0])
                {
                    case "AddUserID":
                        try
                        {
                            vkBot.AddUser(comparms[1], int.Parse(comparms[2]));
                            Console.WriteLine("Successful");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                        break;
                    case "ReloadCalendar":
                        reminders = new Reminders();
                        Celendar.Initialize();
                        Console.WriteLine("Successful");
                        break;
                    default:
                        Console.WriteLine("Error, Incorrect command");
                        break;
                }
            }
        }

        private static void Reminders_NewRemind(object sender, RemindEventArgs e)
        {
            vkBot.SendMessage(e.sender.Summary + "\n" + e.sender.Description + "\n" + e.sender.Start.DateTime.ToString() + " — " + e.sender.End.DateTime.ToString() + "\n", e.group);
        }
        static public Reminders reminders = new Reminders();
        static public VkBot vkBot;
    }
}
