using TrollChat.BusinessLogic.Actions.Base;
using TrollChat.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TrollChat.BusinessLogic.Configuration.Implementations
{
    public static class DependencyRegister
    {
        public static void Register(IServiceCollection services)
        {
            var dbDependencyBuilder = new DependencyBuilder<IRepository>();
            dbDependencyBuilder.Register(services);

            var blDependencyBuilder = new DependencyBuilder<IAction>();
            blDependencyBuilder.Register(services);
        }
    }
}