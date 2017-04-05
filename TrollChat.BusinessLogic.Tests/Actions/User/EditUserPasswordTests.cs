using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementation;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using System.Linq;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    public class EditUserPasswordTests
    {
        [Fact]
        public void Invoke_ValidData_SavedAndEditAreCalled()
        {
            // prepare
            var userFromDb = new DataAccess.Models.User()
            {
                Id = 1,
                Name = "Name",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            };

            var userToken = new DataAccess.Models.UserToken()
            {
                SecretToken = "123"
            };

            var findByResult = new List<DataAccess.Models.UserToken>() { userToken };

            DataAccess.Models.User userSaved = null;
            var mockedUserRepository = new Mock<IUserRepository>();

            mockedUserRepository.Setup(r => r.GetById(1)).Returns(userFromDb);
            mockedUserRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.User>()))
                .Callback<DataAccess.Models.User>(u => userSaved = u);

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();
            mockedUserTokenRepo.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
               .Returns(findByResult.AsQueryable);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.GenerateRandomSalt()).Returns("salt-generated");
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object, mockedHasher.Object);

            // action
            var actionResult = action.Invoke(1, "plain");

            // assert
            Assert.True(actionResult);
            Assert.Equal("plain-hashed", userSaved.PasswordHash);
            Assert.Equal("salt-generated", userSaved.PasswordSalt);
            Assert.Equal("Name", userSaved.Name);
            Assert.Equal("123", userToken.SecretToken);

            mockedHasher.Verify(r => r.GenerateRandomSalt(), Times.Once);
            mockedHasher.Verify(r => r.CreatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Once());
            mockedUserRepository.Verify(r => r.Save(), Times.Once());

            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUserTokenRepo.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_NoUser()
        {
            // prepare
            var userToken = new DataAccess.Models.UserToken()
            {
                SecretToken = "123"
            };

            var findByResult = new List<DataAccess.Models.UserToken>() { userToken };

            DataAccess.Models.User userSaved = null;
            var mockedUserRepository = new Mock<IUserRepository>();

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();
            mockedUserTokenRepo.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.GenerateRandomSalt()).Returns("salt-generated");
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object, mockedHasher.Object);

            // action
            var actionResult = action.Invoke(1, "plain");

            // assert
            Assert.False(actionResult);
            Assert.Null(userSaved);

            mockedHasher.Verify(r => r.GenerateRandomSalt(), Times.Never);
            mockedHasher.Verify(r => r.CreatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUserRepository.Verify(r => r.Save(), Times.Never);

            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepo.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyPassword_SaveNorEditAreCalled()
        {
            // prepare
            var userFromDb = new DataAccess.Models.User() { Id = 1 };
            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(r => r.GetById(1))
                .Returns(userFromDb);

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

            var action = new EditUserPassword(mockedUserTokenRepository.Object, mockedUserRepo.Object);

            // action
            var actionResult = action.Invoke(1, "");

            // assert
            Assert.False(actionResult);
            mockedUserRepo.Verify(r => r.Save(), Times.Never);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);

            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_NoToken_SaveNorEditAreCalled()
        {
            // prepare
            var userFromDb = new DataAccess.Models.User()
            {
                Id = 1,
                Name = "Name",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(1))
                .Returns(userFromDb);

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object);

            // action
            var actionResult = action.Invoke(1, "123");

            // assert
            Assert.False(actionResult);
            mockedUserRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);

            mockedUserTokenRepo.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Once);
            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUserTokenRepo.Verify(r => r.Save(), Times.Never);
        }
    }
}