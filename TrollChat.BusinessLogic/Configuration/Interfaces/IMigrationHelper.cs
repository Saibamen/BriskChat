using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Configuration.Interfaces
{
    public interface IMigrationHelper : IAction
    {
        void Migrate();
    }
}