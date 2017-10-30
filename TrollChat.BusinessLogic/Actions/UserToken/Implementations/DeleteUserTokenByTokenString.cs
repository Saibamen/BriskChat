using System.Linq;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
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