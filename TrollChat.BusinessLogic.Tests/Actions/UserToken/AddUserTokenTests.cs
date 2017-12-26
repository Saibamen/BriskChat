using System;
using BriskChat.BusinessLogic.Actions.UserToken.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.UserToken
{
    public class AddUserTokenTests
    {
        [Fact]
        public void Invoke_ValidData_AddsUserToDatabaseWithCorrectValues()
        {
            var user = new DataAccess.Models.User
            {
                Name = "Ryszard"
            };

            DataAccess.Models.UserToken tokensaved = null;

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedHasher = new Mock<IHasher>();

            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(user);

            mockedHasher.Setup(r => r.GenerateRandomGuid()).Returns("123");

            mockedUserTokenRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()))
                .Callback<DataAccess.Models.UserToken>(u => tokensaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object, mockedHasher.Object);

            action.Invoke(Guid.NewGuid());

            Assert.Equal("Ryszard", tokensaved.User.Name);
            Assert.Equal("123", tokensaved.SecretToken);

            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_UserAlreadyHadToken()
        {
            var user = new DataAccess.Models.User
            {
                Name = "Ryszard"
            };

            var userTokenFromDatabase = new DataAccess.Models.UserToken();

            DataAccess.Models.UserToken tokensaved = null;

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedHasher = new Mock<IHasher>();

            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(user);

            mockedHasher.Setup(r => r.GenerateRandomGuid()).Returns("123");

            mockedUserTokenRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()))
                .Callback<DataAccess.Models.UserToken>(u => tokensaved = u);

            mockedUserTokenRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(userTokenFromDatabase);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object, mockedHasher.Object);

            action.Invoke(Guid.NewGuid());

            Assert.Equal("Ryszard", tokensaved.User.Name);
            Assert.Equal("123", tokensaved.SecretToken);

            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Equal(string.Empty, actionResult);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_AddNorSaveAreCalled()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddUserTokenToUser(mockedUserTokenRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(new Guid());

            // assert
            Assert.Equal(string.Empty, actionResult);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}