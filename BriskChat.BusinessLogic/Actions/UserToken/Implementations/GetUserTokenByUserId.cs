﻿using System;
using System.Linq;
using AutoMapper;
using BriskChat.BusinessLogic.Actions.UserToken.Interfaces;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;

namespace BriskChat.BusinessLogic.Actions.UserToken.Implementations
{
    public class GetUserTokenByUserId : IGetUserTokenByUserId
    {
        private readonly IUserTokenRepository userTokenRepository;

        public GetUserTokenByUserId(IUserTokenRepository userTokenRepository)
        {
            this.userTokenRepository = userTokenRepository;
        }

        public UserTokenModel Invoke(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var token = userTokenRepository.FindBy(x => x.User.Id == userId).FirstOrDefault();

            if (token == null)
            {
                return null;
            }

            var userTokenModel = Mapper.Map<UserTokenModel>(token);

            return userTokenModel;
        }
    }
}