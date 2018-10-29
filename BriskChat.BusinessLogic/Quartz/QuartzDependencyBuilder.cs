using BriskChat.BusinessLogic.Quartz.Jobs.EmailUsers;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;

namespace BriskChat.BusinessLogic.Quartz
{
    public static class QuartzDependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient<IJobFactory, CustomJobFactory>(provider => new CustomJobFactory(provider));
            // TODO: Create automatic dependency register if there will be too many jobs
            services.AddTransient<EmailUsersJob>();
        }
    }
}