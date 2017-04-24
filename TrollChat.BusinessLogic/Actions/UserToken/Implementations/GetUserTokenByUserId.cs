using System;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using System.Linq;
using AutoMapper;
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

        public UserTokenModel Invoke(Guid userId)
        {
            var token = userRepository.FindBy(x => x.User.Id == userId).FirstOrDefault();

            if (token == null)
            {
                return null;
            }

            var userTokenModel = Mapper.Map<UserTokenModel>(token);

            return userTokenModel;
        }
    }
}