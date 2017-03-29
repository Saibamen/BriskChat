using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IGetUserToken : IRepository
    {
        string Invoke(int userId);
    }
}