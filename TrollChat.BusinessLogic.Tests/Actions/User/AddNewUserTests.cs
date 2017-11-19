using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class AddNewUserTests
    {
        [Fact]
        public void Invoke_ValidData_AddsUserToDatabaseWithCorrectValues()
        {
            // prepare
            var userData = new UserModel
            {
                Email = "email",
                Password = "plain",
                Name = "Ryszard"
            };

            DataAccess.Models.User userSaved = null;
            DataAccess.Models.UserToken userTokenSaved = null;

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

            mockedUserTokenRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()))
                .Callback<DataAccess.Models.UserToken>(u => userTokenSaved = u);

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(r => r.Add(It.IsAny<DataAccess.Models.User>()))
                .Callback<DataAccess.Models.User>(u => userSaved = u);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.GenerateRandomSalt()).Returns("salt-generated");
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewUser(mockedUserRepo.Object, mockedUserTokenRepository.Object, mockedUnitOfWork.Object, mockedHasher.Object);

            // action
            action.Invoke(userData);

            // assert
            Assert.Equal("plain-hashed", userSaved.PasswordHash);
            Assert.Equal("salt-generated", userSaved.PasswordSalt);
            Assert.Equal("Ryszard", userSaved.Name);
            mockedUserRepo.Verify(r => r.Add(It.IsAny<DataAccess.Models.User>()), Times.Once());
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Exactly(1));
            Assert.NotNull(userTokenSaved);
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var userToAdd = new UserModel();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewUser(mockedUserRepository.Object, mockedUserTokenRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(userToAdd);

            // assert
            Assert.Null(actionResult);
            mockedUserRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_AlreadyExists_AddNorSaveAreCalled()
        {
            // prepare
            var userToAdd = new UserModel
            {
                Email = "test",
                Password = "Password"
            };
            var userFromDb = new DataAccess.Models.User
            {
                Email = "test"
            };
            var findByResult = new List<DataAccess.Models.User> { userFromDb };

            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewUser(mockedUserRepository.Object, mockedUserTokenRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(userToAdd);

            // assert
            Assert.Null(actionResult);
            mockedUserRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}