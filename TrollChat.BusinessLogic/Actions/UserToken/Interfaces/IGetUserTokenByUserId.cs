using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IGetUserTokenByUserId : IRepository
    {
        string Invoke(int userId);
    }
}