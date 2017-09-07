using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Configuration.Interfaces
{
    public interface IDbContextSeeder : IAction
    {
        bool Seed();
    }
}