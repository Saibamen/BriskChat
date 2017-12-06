using System;
using BriskChat.BusinessLogic.Actions.Base;
using BriskChat.BusinessLogic.Models;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IGetUserById : IAction
    {
        UserModel Invoke(Guid id);
    }
}