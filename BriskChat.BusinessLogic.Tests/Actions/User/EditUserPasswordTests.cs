﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.User.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.User
{
    public class EditUserPasswordTests
    {
        [Fact]
        public void Invoke_ValidData_SavedAndEditAreCalled()
        {
            // prepare
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                Name = "Name",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            };

            var userToken = new DataAccess.Models.UserToken
            {
                SecretToken = "123"
            };

            var findByResult = new List<DataAccess.Models.UserToken> { userToken };

            DataAccess.Models.User userSaved = null;
            var mockedUserRepository = new Mock<IUserRepository>();

            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(userFromDb);
            mockedUserRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.User>()))
                .Callback<DataAccess.Models.User>(u => userSaved = u);

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();
            mockedUserTokenRepo.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
               .Returns(findByResult.AsQueryable);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.GenerateRandomSalt()).Returns("salt-generated");
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object, mockedUnitOfWork.Object, mockedHasher.Object);

            // action
            var actionResult = action.Invoke(Guid.NewGuid(), "plain");

            // assert
            Assert.True(actionResult);
            Assert.Equal("plain-hashed", userSaved.PasswordHash);
            Assert.Equal("salt-generated", userSaved.PasswordSalt);
            Assert.Equal("Name", userSaved.Name);
            Assert.Equal("123", userToken.SecretToken);

            mockedHasher.Verify(r => r.GenerateRandomSalt(), Times.Once);
            mockedHasher.Verify(r => r.CreatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Once());

            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_NoUser()
        {
            // prepare
            var userToken = new DataAccess.Models.UserToken
            {
                SecretToken = "123"
            };

            var findByResult = new List<DataAccess.Models.UserToken> { userToken };

            DataAccess.Models.User userSaved = null;
            var mockedUserRepository = new Mock<IUserRepository>();

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();
            mockedUserTokenRepo.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.GenerateRandomSalt()).Returns("salt-generated");
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object, mockedUnitOfWork.Object, mockedHasher.Object);

            // action
            var actionResult = action.Invoke(Guid.NewGuid(), "plain");

            // assert
            Assert.False(actionResult);
            Assert.Null(userSaved);

            mockedHasher.Verify(r => r.GenerateRandomSalt(), Times.Never);
            mockedHasher.Verify(r => r.CreatePasswordHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);

            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Invoke_EmptyPassword_SaveNorEditAreCalled(string password)
        {
            // prepare
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User { Id = guid };
            var mockedUserRepo = new Mock<IUserRepository>();

            mockedUserRepo.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(userFromDb);

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditUserPassword(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(guid, password);

            // assert
            Assert.False(actionResult);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);

            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_NoToken_SaveNorEditAreCalled()
        {
            // prepare
            var guid = Guid.NewGuid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                Name = "Name",
                PasswordHash = "hash",
                PasswordSalt = "salt"
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(userFromDb);

            var mockedUserTokenRepo = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditUserPassword(mockedUserTokenRepo.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(guid, "123");

            // assert
            Assert.False(actionResult);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedUserRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);

            mockedUserTokenRepo.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Once);
            mockedUserTokenRepo.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}