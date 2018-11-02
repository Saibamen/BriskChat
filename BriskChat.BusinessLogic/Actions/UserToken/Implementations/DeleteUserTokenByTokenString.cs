using System.Linq;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class DeleteUserTokenByTokenString : IDeleteUserTokenByTokenString
    {
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserTokenByTokenString(IUserTokenRepository userTokenRepository, IUnitOfWork unitOfWork)
        {
            _userTokenRepository = userTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var userToken = _userTokenRepository
                .FindBy(x => x.SecretToken == token)
                .FirstOrDefault();

            if (userToken == null)
            {
                return false;
            }

            _userTokenRepository.Delete(userToken);
            _unitOfWork.Save();

            return true;
        }
    }
}