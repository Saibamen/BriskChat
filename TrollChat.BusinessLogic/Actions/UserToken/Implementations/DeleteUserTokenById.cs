using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class DeleteUserTokenById : IDeleteUserTokenById
    {
        private readonly IUserTokenRepository userTokenRepository;

        public DeleteUserTokenById(IUserTokenRepository userTokenRepository)
        {
            this.userTokenRepository = userTokenRepository;
        }

        public bool Invoke(int userTokenId)
        {
            var userToken = userTokenRepository.GetById(userTokenId);

            if (userToken == null)
            {
                return false;
            }

            userTokenRepository.Delete(userToken);
            userTokenRepository.Save();

            return true;
        }
    }
}