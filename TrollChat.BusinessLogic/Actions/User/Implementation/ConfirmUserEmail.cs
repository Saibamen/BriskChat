using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class ConfirmUserEmail : IConfirmUserEmail
    {
        private readonly IUserRepository userRepository;

        public ConfirmUserEmail(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public bool Invoke(int userId)
        {
            var userToEdit = userRepository.GetById(userId);

            if (userToEdit == null || userToEdit.EmailConfirmedOn != null)
            {
                return false;
            }

            userToEdit.EmailConfirmedOn = DateTime.UtcNow;

            userRepository.Edit(userToEdit);
            userRepository.Save();

            return true;
        }
    }
}