using System;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class DeleteUserById : IDeleteUserById
    {
        private readonly IUserRepository userRepository;

        public DeleteUserById(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public bool Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return false;
            }

            var userToDelete = userRepository.GetById(userId);

            if (userToDelete == null)
            {
                return false;
            }

            userRepository.Delete(userToDelete);
            userRepository.Save();

            return true;
        }
    }
}