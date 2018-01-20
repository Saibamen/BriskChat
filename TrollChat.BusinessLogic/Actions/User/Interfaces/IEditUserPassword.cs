using System;
using BriskChat.BusinessLogic.Actions.Base;

namespace BriskChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IEditUserPassword : IAction
    {
        bool Invoke(Guid userId, string plaintextPassword);
    }
}