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