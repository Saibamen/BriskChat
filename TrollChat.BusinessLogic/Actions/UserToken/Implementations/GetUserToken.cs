using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using System.Linq;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class GetUserToken : IGetUserToken
    {
        private readonly IUserTokenRepository userRepository;

        public GetUserToken(IUserTokenRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public string Invoke(int userId)
        {
            var token = userRepository.FindBy(x => x.User.Id == userId).FirstOrDefault().SecretToken;
            return token;
        }
    }
}