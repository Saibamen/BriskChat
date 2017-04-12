using System.Linq;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class AddUserTokenToUser : IAddUserTokenToUser
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;

        public AddUserTokenToUser(IUserTokenRepository userTokenRepository,
            IUserRepository userRepository,
            IHasher hasher = null)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public string Invoke(int userId)
        {
            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return string.Empty;
            }

            var token = userTokenRepository.FindBy(x => x.User == user).FirstOrDefault();

            if (token != null)
            {
                userTokenRepository.Delete(token);
                userTokenRepository.Save();
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