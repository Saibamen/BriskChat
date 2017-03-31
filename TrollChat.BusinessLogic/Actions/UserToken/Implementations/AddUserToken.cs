using System.Linq;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class AddUserToken : IAddUserToken
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;

        public AddUserToken(IUserTokenRepository userTokenRepository, IUserRepository userRepository, IHasher hasher)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            this.hasher = hasher;
        }

        public string Invoke(int userId)
        {
            var user = userRepository.GetById(userId);

            // TODO: Move logic to repository ??
            if (user.Tokens.Any(x => x.DeletedOn == null))
            {
                userTokenRepository.Delete(user.Tokens.FirstOrDefault(x => x.DeletedOn == null));
            }

            var userToken = new DataAccess.Models.UserToken()
            {
                User = user,
                SecretToken = hasher.GenerateRandomGuid(),
            };

            userTokenRepository.Add(userToken);
            userTokenRepository.Save();

            return userToken.SecretToken;
        }
    }
}