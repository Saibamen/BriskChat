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
        private readonly IUserRepository _userRepository;
        private readonly IDomainRepository _domainRepository;
        private readonly IHasher _hasher;

        public AuthenticateUser(
            IUserRepository userRepository,
            IDomainRepository domainRepository,
            IHasher hasher = null)
        {
            _userRepository = userRepository;
            _domainRepository = domainRepository;
            _hasher = hasher ?? new Hasher();
        }

        public UserModel Invoke(string email, string password, string domainName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(domainName))
            {
                return null;
            }

            var domain = _domainRepository
                .FindBy(x => x.Name == domainName)
                .FirstOrDefault();

            if (domain == null)
            {
                return null;
            }

            var dbUser = _userRepository
                .FindBy(x => x.Email == email && x.EmailConfirmedOn != null && x.Domain == domain)
                .FirstOrDefault();

            if (dbUser == null)
            {
                return null;
            }

            var salt = dbUser.PasswordSalt;
            var hashedPassword = _hasher.CreatePasswordHash(password, salt);

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