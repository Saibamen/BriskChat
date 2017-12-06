using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Helpers.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class AuthenticateUser : IAuthenticateUser
    {
        private readonly IUserRepository userRepository;
        private readonly IDomainRepository domainRepository;
        private readonly IHasher hasher;

        public AuthenticateUser(
            IUserRepository userRepository,
            IDomainRepository domainRepository,
            IHasher hasher = null)
        {
            this.userRepository = userRepository;
            this.domainRepository = domainRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public UserModel Invoke(string email, string password, string domainName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(domainName))
            {
                return null;
            }

            var domain = domainRepository.FindBy(x => x.Name == domainName).FirstOrDefault();

            if (domain == null)
            {
                return null;
            }

            var dbUser = userRepository.FindBy(x => x.Email == email && x.EmailConfirmedOn != null && x.Domain == domain).FirstOrDefault();

            if (dbUser == null)
            {
                return null;
            }

            var salt = dbUser.PasswordSalt;
            var hashedPassword = hasher.CreatePasswordHash(password, salt);

            if (hashedPassword != dbUser.PasswordHash)
            {
                return null;
            }

            var domainModel = new DomainModel
            {
                Id = domain.Id,
                Name = domain.Name
            };

            var userModel = new UserModel
            {
                Id = dbUser.Id,
                Name = dbUser.Name,
                Email = dbUser.Email,
                Domain = domainModel
            };

            return userModel;
        }
    }
}