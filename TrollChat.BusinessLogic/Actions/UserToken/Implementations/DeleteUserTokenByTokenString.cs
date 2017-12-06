using System.Linq;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;

namespace BriskChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class DeleteUserTokenByTokenString : IDeleteUserTokenyByTokenString
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserTokenByTokenString(IUserTokenRepository userTokenRepository, IUnitOfWork unitOfWork)
        {
            this.userTokenRepository = userTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public bool Invoke(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var userToken = userTokenRepository.FindBy(x => x.SecretToken == token).FirstOrDefault();

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