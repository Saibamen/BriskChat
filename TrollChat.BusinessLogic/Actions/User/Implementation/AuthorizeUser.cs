using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class AuthorizeUser : IAuthorizeUser
    {
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;

        public AuthorizeUser(
            IUserRepository userRepository,
            IHasher hasher = null)
        {
            this.userRepository = userRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public bool Invoke(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var dbUser = userRepository.FindBy(x => x.Email == email
           // && x.EmailConfirmedOn != null
            ).FirstOrDefault();

            if (dbUser == null)
            {
                return false;
            }

            var salt = dbUser.PasswordSalt;
            var hashedPassword = hasher.CreatePasswordHash(password, salt);

            return Equals(hashedPassword, dbUser.PasswordHash);
        }
    }
}