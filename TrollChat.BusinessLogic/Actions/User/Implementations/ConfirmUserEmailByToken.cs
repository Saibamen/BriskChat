using System;
using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class ConfirmUserEmailByToken : IConfirmUserEmailByToken
    {
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository userTokenRepository;

        public ConfirmUserEmailByToken(IUserTokenRepository userTokenRepository, IUserRepository userRepository)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
        }

        public bool Invoke(string guid)
        {
            var userToken = userTokenRepository.FindBy(x => x.SecretToken == guid).FirstOrDefault();

            if (userToken == null
                || userToken.User.EmailConfirmedOn != null
                || userToken.SecretTokenTimeStamp <= DateTime.UtcNow)
            {
                return false;
            }

            userToken.User.EmailConfirmedOn = DateTime.UtcNow;

            userRepository.Edit(userToken.User);
            userRepository.Save();

            userTokenRepository.Delete(userToken);
            userTokenRepository.Save();

            return true;
        }
    }
}