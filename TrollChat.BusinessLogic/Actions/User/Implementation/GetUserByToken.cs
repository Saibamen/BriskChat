using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class GetUserByToken : IGetUserByToken
    {
        private readonly IUserRepository userRepository;

        public GetUserByToken(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public DataAccess.Models.User Invoke(string token)
        {
            // TODO: Move logic to repositry ??
            var user = userRepository.FindBy(x => x.Tokens.Any(y => y.SecretToken == token && y.DeletedOn == null)).FirstOrDefault();

            return user;
        }
    }
}