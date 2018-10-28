using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Configuration.Interfaces
{
    public interface IDbContextSeeder : IAction
    {
        bool Seed();
    }
}