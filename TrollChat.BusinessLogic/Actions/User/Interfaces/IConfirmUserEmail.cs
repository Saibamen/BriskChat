using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Actions.User.Interfaces
{
    public interface IConfirmUserEmail : IAction
    {
        bool Invoke(int userId);
    }
}