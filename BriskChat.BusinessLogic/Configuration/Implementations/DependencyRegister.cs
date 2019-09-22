using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Helpers.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BriskChat.BusinessLogic.Configuration.Implementations
{
    public static class DependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            var dbDependencyBuilder = new DependencyBuilder<IRepository>();
            dbDependencyBuilder.Register(services);

            var blDependencyBuilder = new DependencyBuilder<IAction>();
            blDependencyBuilder.Register(services);

            services.AddScoped<IEmailService, EmailService>();
        }
    }
}