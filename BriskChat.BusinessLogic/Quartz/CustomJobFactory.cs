using System;
using Quartz;
using Quartz.Spi;

namespace BriskChat.BusinessLogic.Quartz
{
    public class CustomJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = (IJob)_serviceProvider.GetService(bundle.JobDetail.JobType);

            return job;
        }

        public void ReturnJob(IJob job) { }
    }
}