using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using TrollChat.BusinessLogic.Quartz.Jobs;

namespace TrollChat.BusinessLogic.Quartz
{
    public static class ShedulerConfig
    {
        public static void CreateScheduler(IScheduler scheduler, IApplicationBuilder app)
        {
            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;

            scheduler.JobFactory = (IJobFactory)app.ApplicationServices.GetService(typeof(IJobFactory));
            scheduler.Start().Wait();

            var userEmailsJob = JobBuilder.Create<EmailJob>()
                .WithIdentity("SendUserEmails")
                .Build();
            var userEmailsTrigger = TriggerBuilder.Create()
                .WithIdentity("UserEmailsCron")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
                .Build();

            scheduler.ScheduleJob(userEmailsJob, userEmailsTrigger).Wait();
        }
    }
}