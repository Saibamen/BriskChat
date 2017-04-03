using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class AddNewUser : IAddNewUser
    {
        private readonly IUserRepository userRepository;
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IHasher hasher;

        public AddNewUser(IUserRepository userRepository,
            IUserTokenRepository userTokenRepository,
            IHasher hasher = null)
        {
            this.userRepository = userRepository;
            this.userTokenRepository = userTokenRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public DataAccess.Models.User Invoke(Models.UserModel user)
        {
            if (!user.IsValid() || userRepository.FindBy(x => x.Email == user.Email).Any())
            {
                return null;
            }

            var salt = hasher.GenerateRandomSalt();
            var passwordHash = hasher.CreatePasswordHash(user.Password, salt);

            var newUser = new DataAccess.Models.User
            {
                Email = user.Email,
                PasswordSalt = salt,
                PasswordHash = passwordHash,
                Name = user.Name
            };

            userRepository.Add(newUser);

            var newUserToken = new DataAccess.Models.UserToken()
            {
                User = newUser,
                SecretToken = hasher.GenerateRandomGuid(),
            };

            userTokenRepository.Add(newUserToken);
            userRepository.Save();

            return newUser;
        }
    }
}