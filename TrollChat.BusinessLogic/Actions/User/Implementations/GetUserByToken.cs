using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrollChat.BusinessLogic.Actions.User.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserByToken : IGetUserByToken
    {
        private readonly IUserTokenRepository userTokenRepository;

        public GetUserByToken(IUserTokenRepository userTokenRepository)
        {
            this.userTokenRepository = userTokenRepository;
        }

        public UserModel Invoke(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var result = userTokenRepository.FindBy(y => y.SecretToken == token).Include(x => x.User).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<UserModel>(result.User);

            return user;
        }
    }
}