using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
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