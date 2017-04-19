using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
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

            var newUser = AutoMapper.Mapper.Map<DataAccess.Models.User>(user);
            newUser.PasswordSalt = hasher.GenerateRandomSalt();
            newUser.PasswordHash = hasher.CreatePasswordHash(user.Password, newUser.PasswordSalt);

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