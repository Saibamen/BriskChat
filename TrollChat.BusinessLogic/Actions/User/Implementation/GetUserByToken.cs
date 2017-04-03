using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementation
{
    public class GetUserByToken : IGetUserByToken
    {
        private readonly IUserTokenRepository userTokenRepository;

        public GetUserByToken(IUserTokenRepository userTokenRepository)
        {
            this.userTokenRepository = userTokenRepository;
        }

        public DataAccess.Models.User Invoke(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var result = userTokenRepository.FindBy(y => y.SecretToken == token).FirstOrDefault();

            return result?.User;
        }
    }
}