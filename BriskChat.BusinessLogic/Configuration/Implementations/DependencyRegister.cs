using BriskChat.BusinessLogic.Actions.Base;
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
        }
    }
}