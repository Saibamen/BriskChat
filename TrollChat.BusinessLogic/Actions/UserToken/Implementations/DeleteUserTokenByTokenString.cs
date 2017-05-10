using System.Linq;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class DeleteUserTokenByTokenString : IDeleteUserTokenyByTokenString
    {
        private readonly IUserTokenRepository userTokenRepository;

        public DeleteUserTokenByTokenString(IUserTokenRepository userTokenRepository)
        {
            this.userTokenRepository = userTokenRepository;
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
            userTokenRepository.Save();

            return true;
        }
    }
}