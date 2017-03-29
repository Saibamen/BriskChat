using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.Room.Interfaces
{
    public interface IAddNewRoom : IAction
    {
        int Invoke(Models.Room room);
    }
}