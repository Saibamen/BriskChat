using System;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.UserToken.Interfaces
{
    public interface IGetUserTokenByUserId : IRepository
    {
        UserTokenModel Invoke(Guid userId);
    }
}