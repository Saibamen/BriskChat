using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using System.Linq;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class GetUserTokenByUserId : IGetUserTokenByUserId
    {
        private readonly IUserTokenRepository userRepository;

        public GetUserTokenByUserId(IUserTokenRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserTokenModel Invoke(int userId)
        {
            var token = userRepository.FindBy(x => x.User.Id == userId).FirstOrDefault();

            if (token == null)
            {
                return null;
            }

            var userTokenModel = new UserTokenModel()
            {
                Id = token.Id,
                SecretToken = token.SecretToken,
                SecretTokenTimeStamp = token.SecretTokenTimeStamp,
                CreatedOn = token.CreatedOn,
                ModifiedOn = token.ModifiedOn,
                DeletedOn = token.DeletedOn,
            };

            return userTokenModel;
        }
    }
}