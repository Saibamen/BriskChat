using System;
using Quartz;
using Quartz.Spi;

namespace TrollChat.BusinessLogic.Quartz
{
    public class CustomJobFactory : IJobFactory
    {
        private readonly IServiceProvider _container;

        public CustomJobFactory(IServiceProvider container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var item = _container.GetService(bundle.JobDetail.JobType) as IJob;
            return item;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}