using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using TrollChat.BusinessLogic.Quartz.Jobs.EmailUsers;

namespace TrollChat.BusinessLogic.Quartz
{
    public static class ShedulerCreator
    {
        public static IScheduler CreateScheduler(IApplicationBuilder app)
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;

            scheduler.JobFactory = app.ApplicationServices.GetService(typeof(IJobFactory)) as IJobFactory;

            var userEmailsConfig = new EmailUsersJobConfig();
            scheduler.ScheduleJob(userEmailsConfig.GetJobDetail(), userEmailsConfig.GetJobTrigger()).Wait();

            return scheduler;
        }
    }
}