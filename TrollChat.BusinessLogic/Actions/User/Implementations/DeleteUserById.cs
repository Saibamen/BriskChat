using System;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class DeleteUserById : IDeleteUserById
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserById(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            _unitOfWork = unitOfWork;
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
            _unitOfWork.Save();

            return true;
        }
    }
}