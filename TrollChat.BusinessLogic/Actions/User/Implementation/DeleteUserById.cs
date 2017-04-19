using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class DeleteUserById : IDeleteUserById
    {
        private readonly IUserRepository userRepository;

        public DeleteUserById(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public bool Invoke(int userId)
        {
            var roomToDelete = userRepository.GetById(userId);

            if (roomToDelete == null)
            {
                return false;
            }

            userRepository.Delete(roomToDelete);
            userRepository.Save();

            return true;
        }
    }
}