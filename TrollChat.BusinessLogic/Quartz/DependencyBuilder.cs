using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using TrollChat.BusinessLogic.Quartz.Jobs;

namespace TrollChat.BusinessLogic.Quartz
{
    public static class QuartzDependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IJobFactory, CustomJobFactory>(provider => new CustomJobFactory(provider));
            services.AddTransient<EmailJob>();
        }
    }
}