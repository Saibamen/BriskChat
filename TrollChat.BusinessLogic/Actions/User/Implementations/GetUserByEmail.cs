using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserByEmail : IGetUserByEmail
    {
        private readonly IUserRepository userRepository;
        private readonly IDomainRepository domainRepository;

        public GetUserByEmail(IUserRepository userRepository, IDomainRepository domainRepository)
        {
            this.userRepository = userRepository;
            this.domainRepository = domainRepository;
        }

        public UserModel Invoke(string email, string domainName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(domainName))
            {
                return null;
            }

            var domain = domainRepository.FindBy(x => x.Name == domainName).FirstOrDefault();

            if (domain == null)
            {
                return null;
            }

            var dbUser = userRepository.FindBy(x => x.Email == email && x.Domain == domain).FirstOrDefault();

            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<UserModel>(dbUser);

            return user;
        }
    }
}