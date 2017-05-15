﻿using System;
using TrollChat.BusinessLogic.Actions.UserToken.Interfaces;
using TrollChat.BusinessLogic.Helpers.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;

namespace TrollChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class AddUserTokenToUser : IAddUserTokenToUser
    {
        private readonly IUserTokenRepository userTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IHasher hasher;

        public AddUserTokenToUser(IUserTokenRepository userTokenRepository,
            IUserRepository userRepository,
            IHasher hasher = null)
        {
            this.userTokenRepository = userTokenRepository;
            this.userRepository = userRepository;
            this.hasher = hasher ?? new Hasher();
        }

        public string Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return string.Empty;
            }

            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return string.Empty;
            }

            var token = userTokenRepository.GetById(userId);

            if (token != null)
            {
                userTokenRepository.Delete(token);
                userTokenRepository.Save();
            }

            var userToken = new DataAccess.Models.UserToken
            {
                User = user,
                SecretToken = hasher.GenerateRandomGuid()
            };

            userTokenRepository.Add(userToken);
            userTokenRepository.Save();

            return userToken.SecretToken;
        }
    }
}