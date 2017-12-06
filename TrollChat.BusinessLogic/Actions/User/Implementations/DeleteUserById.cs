using System;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
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