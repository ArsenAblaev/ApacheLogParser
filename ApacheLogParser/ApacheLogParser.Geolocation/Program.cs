using System;
using ApacheLogParser.BL.Services;
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;
using Timer = System.Threading.Timer;

//using Microsoft.Win32.TaskScheduler;

namespace ApacheLogParser.Geolocation
{
    class Program
    {
        static void Main()
        {

            IApacheLogRepository repository = new ApacheLogRepository();
            ILogger logger = new ConsoleLogger(); //example
            var geolocationService = new ApacheLogGeolocationService(repository, logger);

            //  geolocationService.Geolocate();



            //todo change it
            new Timer(state => geolocationService.Geolocate(),"test",0,1000);

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            //using (TaskService ts = new TaskService())
            //{
            //    // Create a new task definition and assign properties
            //    TaskDefinition td = ts.NewTask();
            //    td.RegistrationInfo.Description = "Does something";

            //    // Create a trigger that will fire the task at this time every other day
            //    td.Triggers.Add(new DailyTrigger { DaysInterval = 2 });

            //    // Create an action that will launch Notepad whenever the trigger fires


            //    // Register the task in the root folder
            //    ts.RootFolder.RegisterTaskDefinition(@"Test", td);

            //    // Remove the task we just created
            //    ts.RootFolder.DeleteTask("Test");
            //}
            //   GC.KeepAlive(_timer);
            //  Console.ReadKey();
        }


    }
}
