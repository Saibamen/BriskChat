using BriskChat.BusinessLogic.Quartz.Jobs.EmailUsers;
using Microsoft.AspNetCore.Builder;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BriskChat.BusinessLogic.Quartz
{
    public static class SchedulerCreator
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