using System;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IGetUserTokenByUserId : IRepository
    {
        UserTokenModel Invoke(Guid userId);
    }
}