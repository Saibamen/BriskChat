using System;
using BriskChat.BusinessLogic.Actions.User.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.User.Implementations
{
    public class GetUserById : IGetUserById
    {
        private readonly IUserRepository _userRepository;

        public GetUserById(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserModel Invoke(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var dbUser = _userRepository.GetById(id);

            if (dbUser == null)
            {
                return null;
            }

            var user = AutoMapper.Mapper.Map<UserModel>(dbUser);

            return user;
        }
    }
}