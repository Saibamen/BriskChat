using System;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IEditUserPassword : IAction
    {
        bool Invoke(Guid userId, string plaintextPassword);
    }
}