using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UserRoles.Models;
using Quartz;
using Quartz.Impl;
using UserRoles.Controllers;

namespace UserRoles
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JobScheduler.Start();
            //ISchedulerFactory scheduler = new StdSchedulerFactory();
            //IScheduler scheduler1 = (IScheduler)scheduler.GetScheduler().GetAwaiter().GetResult();
            //scheduler1.Start();


            //IJobDetail job = JobBuilder.Create<EmailJobController>().Build();

            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithDailyTimeIntervalSchedule
            //    (s => s.WithIntervalInHours(24)
            //    .OnEveryDay()
            //    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(14, 0))
            //    )
            //    .Build();
            ////ITrigger trigger = TriggerBuilder.Create()
            ////  .WithIdentity("trigger1", "group1")
            ////  .StartNow()
            ////  .WithSimpleSchedule(x => x.WithIntervalInHours(24)
            ////  .RepeatForever())
            ////  .Build();

            //scheduler1.ScheduleJob(job, trigger);
        }
    }
}
