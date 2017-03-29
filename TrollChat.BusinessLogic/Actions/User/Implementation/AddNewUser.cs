using System;
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
        private readonly IHasher hasher;

        public AddNewUser(IUserRepository userRepository,
            IHasher hasher = null)
        {
            this.userRepository = userRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public DataAccess.Models.User Invoke(Models.User user)
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
                Name = user.Name,
                SecretToken = hasher.GenerateRandomGuid(),
                SecretTokenTimeStamp = DateTime.UtcNow.AddDays(14),
            };

            userRepository.Add(newUser);
            userRepository.Save();

            return newUser;
        }
    }
}