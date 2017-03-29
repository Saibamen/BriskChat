using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class AddUserToken : IAddUserToken
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IHasher hasher;

        public AddUserToken(IUserTokenRepository userTokenRepository, IHasher hasher)
        {
            this.userTokenRepository = userTokenRepository;
            this.hasher = hasher;
        }

        public bool Invoke(int userId)
        {
            var userToken = new DataAccess.Models.UserToken()
            {
                User = new DataAccess.Models.User(),
                SecretToken = hasher.GenerateRandomGuid(),
            };

            userTokenRepository.Add(userToken);
            userTokenRepository.Save();

            return true;
        }
    }
}