using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Spi;
using TrollChat.BusinessLogic.Actions.Email.Implementations;
using TrollChat.BusinessLogic.Actions.Email.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.Web.QuartzJobs;

namespace TrollChat.Web
{
    public class Program
    {
        private static IScheduler _scheduler;

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                .Build();

            StartScheduler();
            host.Run();
        }

        public static void StartScheduler()
        {
            var services = new ServiceCollection();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGetEmailLog, GetEmailLog>();
            var container = services.BuildServiceProvider();
            var jobFactory = new MyJobFactory(container);

            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;
            _scheduler.JobFactory = jobFactory;

            var userEmailsJob = JobBuilder.Create<EmailJob>()
                .Build();
            var userEmailsTrigger = TriggerBuilder.Create()
               .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
               .Build();

            _scheduler.ScheduleJob(userEmailsJob, userEmailsTrigger);
            // _scheduler.Start();
        }
    }

    public class MyJobFactory : IJobFactory
    {
        private readonly IServiceProvider _container;

        public MyJobFactory(IServiceProvider container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var item = (IJob)_container.GetService(bundle.JobDetail.JobType);
            return item;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}