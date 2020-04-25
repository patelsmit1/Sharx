using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace DataManagement
{
    public class InitTasks
    {
        public static Timer timer;
        static void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 8, 42, 0, 0); //Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Timer Stopped
            timer.Stop();
            //### Scheduled Task Started ###

            //Performing scheduled task

            // Task Finished ### 
            schedule_Timer();
        }
        async void Index()
        {

        }
        
    }
}