﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.User.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserByTokenTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                Name = "Name",
                Email = "email@dot.com",
                EmailConfirmedOn = DateTime.MinValue,
                LockedOn = null,
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null
            };

            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                Id = new Guid(),
                User = userFromDb,
                SecretToken = "123"
            };

            var findByResult = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            // prepare
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                 .Returns(findByResult.AsQueryable);
            var action = new GetUserByToken(mockedUserTokenRepository.Object);

            // action
            var user = action.Invoke("123");

            // check
            Assert.Equal(guid, user.Id);
            Assert.Equal("Name", user.Name);
            Assert.Equal("email@dot.com", user.Email);
            Assert.Equal(DateTime.MinValue, user.EmailConfirmedOn);
            Assert.Null(user.LockedOn);
            Assert.Equal(DateTime.MinValue, user.CreatedOn);
            Assert.Equal(DateTime.MinValue, user.ModifiedOn);
            Assert.Null(user.DeletedOn);
            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyToken()
        {
            // prepare
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var action = new GetUserByToken(mockedUserTokenRepository.Object);

            // action
            var user = action.Invoke("");

            // check
            Assert.Null(user);
            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Never);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var action = new GetUserByToken(mockedUserTokenRepository.Object);

            // action
            var user = action.Invoke("123");

            // check
            Assert.Null(user);
            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Once);
        }
    }
}