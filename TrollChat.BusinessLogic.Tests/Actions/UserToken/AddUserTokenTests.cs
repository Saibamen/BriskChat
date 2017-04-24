using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using TrollChat.BusinessLogic.Actions.UserToken.Implementations;

namespace TrollChat.BusinessLogic.Tests.Actions.UserToken
{
    public class AddUserTokenTests
    {
        [Fact]
        public void Invoke_ValidData_AddsUserToDatabaseWithCorrectValues()
        {
            var user = new DataAccess.Models.User()
            {
                Name = "Ryszard",
            };

            DataAccess.Models.UserToken tokensaved = null;

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedHasher = new Mock<IHasher>();

            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(user);

            mockedHasher.Setup(r => r.GenerateRandomGuid()).Returns("123");

            mockedUserTokenRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()))
             .Callback<DataAccess.Models.UserToken>(u => tokensaved = u);

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedHasher.Object);

            action.Invoke(Guid.NewGuid());

            Assert.Equal("Ryszard", tokensaved.User.Name);
            Assert.Equal("123", tokensaved.SecretToken);

            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUserTokenRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_UserAlreadyHadToken()
        {
            var user = new DataAccess.Models.User()
            {
                Name = "Ryszard",
            };

            var userTokenFromDatabase = new DataAccess.Models.UserToken()
            {
            };

            var findByResult = new List<DataAccess.Models.UserToken>() { userTokenFromDatabase };

            DataAccess.Models.UserToken tokensaved = null;

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedHasher = new Mock<IHasher>();

            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(user);

            mockedHasher.Setup(r => r.GenerateRandomGuid()).Returns("123");

            mockedUserTokenRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()))
             .Callback<DataAccess.Models.UserToken>(u => tokensaved = u);

            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(findByResult.AsQueryable());

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedHasher.Object);

            action.Invoke(Guid.NewGuid());

            Assert.Equal("Ryszard", tokensaved.User.Name);
            Assert.Equal("123", tokensaved.SecretToken);

            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUserTokenRepository.Verify(r => r.Save(), Times.Exactly(2));
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object);

            // action
            var actionResult = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Equal(string.Empty, actionResult);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}