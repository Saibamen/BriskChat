using System;
using Quartz;
using Quartz.Spi;

namespace TrollChat.BusinessLogic.Quartz
{
    public class CustomJobFactory : IJobFactory
    {
        private readonly IServiceProvider container;

        public CustomJobFactory(IServiceProvider container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var item = container.GetService(bundle.JobDetail.JobType) as IJob;
            return item;
        }

        public void ReturnJob(IJob job)
        {
            //this shall remain empty
        }
    }
}