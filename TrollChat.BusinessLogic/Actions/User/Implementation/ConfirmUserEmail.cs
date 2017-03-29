using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Invoke(string guid)
        {
            var userToEdit = userRepository.FindBy(x=> x.SecretToken == guid).FirstOrDefault();

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