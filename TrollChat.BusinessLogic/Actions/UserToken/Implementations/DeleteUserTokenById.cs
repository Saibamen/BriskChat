using System;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class DeleteUserTokenById : IDeleteUserTokenById
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserTokenById(IUserTokenRepository userTokenRepository, IUnitOfWork unitOfWork)
        {
            this.userTokenRepository = userTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(Guid userTokenId)
        {
            if (userTokenId == Guid.Empty)
            {
                return false;
            }

            var userToken = userTokenRepository.GetById(userTokenId);

            if (userToken == null)
            {
                return false;
            }

            userTokenRepository.Delete(userToken);
            _unitOfWork.Save();
            
            return true;
        }
    }
}