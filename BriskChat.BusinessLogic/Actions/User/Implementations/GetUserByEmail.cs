using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserByEmail : IGetUserByEmail
    {
        private readonly IUserRepository _userRepository;
        private readonly IDomainRepository _domainRepository;

        public GetUserByEmail(IUserRepository userRepository, IDomainRepository domainRepository)
        {
            _userRepository = userRepository;
            _domainRepository = domainRepository;
        }

        public UserModel Invoke(string email, string domainName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(domainName))
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
                .FindBy(x => x.Email == email && x.Domain == domain)
                .FirstOrDefault();

            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<UserModel>(dbUser);

            return user;
        }
    }
}