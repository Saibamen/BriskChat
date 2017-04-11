using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using TrollChat.BusinessLogic.Quartz.Jobs.EmailUsers;

namespace TrollChat.BusinessLogic.Quartz
{
    public static class QuartzDependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IJobFactory, CustomJobFactory>(provider => new CustomJobFactory(provider));
            // TODO: Create automaitc dependency register if there will be too many jobs
            services.AddTransient<EmailUsersJob>();
        }
    }
}