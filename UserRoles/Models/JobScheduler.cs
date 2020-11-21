using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using UserRoles.Controllers;

namespace UserRoles.Models
{
    public class JobScheduler
    {
        public static void Start()
        {
            ISchedulerFactory scheduler = new StdSchedulerFactory();
            IScheduler scheduler1 = (IScheduler)scheduler.GetScheduler().GetAwaiter().GetResult();
            scheduler1.Start();
           

            IJobDetail job = JobBuilder.Create<EmailJobController>().Build();
           

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                (s => s.WithIntervalInHours(24)
                .OnEveryDay()
                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(22, 19))
                )
                .Build();
            //ITrigger trigger = TriggerBuilder.Create()
            //  .WithIdentity("trigger1", "group1")
            //  .StartNow()
            //  .WithSimpleSchedule(x => x.WithIntervalInHours(24)
            //  .RepeatForever())
            //  .Build();

            scheduler1.ScheduleJob(job, trigger);
        }
    }
}