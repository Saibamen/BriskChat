using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class GetUserByEmail : IGetUserByEmail
    {
        private readonly IUserRepository userRepository;

        public GetUserByEmail(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public DataAccess.Models.User Invoke(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var dbUser = userRepository.FindBy(x => x.Email == email).FirstOrDefault();
            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<DataAccess.Models.User>(dbUser);

            return user;
        }
    }
}