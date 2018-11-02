using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserByToken : IGetUserByToken
    {
        private readonly IUserTokenRepository _userTokenRepository;

        public GetUserByToken(IUserTokenRepository userTokenRepository)
        {
            _userTokenRepository = userTokenRepository;
        }

        public UserModel Invoke(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var result = _userTokenRepository
                .FindBy(y => y.SecretToken == token)
                .Include(x => x.User)
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<UserModel>(result.User);

            return user;
        }
    }
}