using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    public class ConfirmUserEmailByTokenTests
    {
        [Fact]
        public void Invoke_ValidData_UpdatesEmailConfirmedOn()
        {
            // prepare
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                EmailConfirmedOn = null
            };

            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                User = userFromDb,
                SecretToken = "123"
            };

            DataAccess.Models.User userSaved = null;

            var getAllResults = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                 .Returns(getAllResults.AsQueryable());

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(r => r.Edit(It.IsAny<DataAccess.Models.User>()))
                    .Callback<DataAccess.Models.User>(u => userSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new ConfirmUserEmailByToken(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("1");

            // assert
            Assert.True(actionResult);
            Assert.NotNull(userSaved.EmailConfirmedOn);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Once());
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Exactly(2));
        }

        [Fact]
        public void Invoke_ValidData_TokenExpired()
        {
            // prepare
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                EmailConfirmedOn = null
            };

            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                User = userFromDb,
                SecretToken = "123",
                SecretTokenTimeStamp = DateTime.UtcNow.AddDays(-1)
            };

            DataAccess.Models.User userSaved = null;

            var getAllResults = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(getAllResults.AsQueryable());

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(r => r.Edit(It.IsAny<DataAccess.Models.User>()))
                .Callback<DataAccess.Models.User>(u => userSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new ConfirmUserEmailByToken(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("1");

            // assert
            Assert.False(actionResult);
            Assert.Null(userSaved);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Verify_EmailAlreadyConfirmed_SaveNorEditCalled()
        {
            // prepare
            DateTime dateNow = DateTime.UtcNow;
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                EmailConfirmedOn = dateNow
            };

            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                User = userFromDb,
                SecretToken = "123"
            };

            var getAllResults = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(getAllResults.AsQueryable());
            var mockedUserRepo = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new ConfirmUserEmailByToken(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            action.Invoke("123");

            // assert
            Assert.Equal(userFromDb.EmailConfirmedOn, dateNow);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Verify_ForDoubledAction_UpdatesEmailConfirmedOnOnce()
        {
            // prepare
            var guid = new Guid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                EmailConfirmedOn = null
            };

            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                User = userFromDb,
                SecretToken = "123",
                SecretTokenTimeStamp = DateTime.UtcNow.AddDays(1)
            };

            DataAccess.Models.User userSaved = null;

            var getAllResults = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.SetupSequence(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(getAllResults.AsQueryable())
                .Returns(new List<DataAccess.Models.UserToken>().AsQueryable());

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(r => r.Edit(It.IsAny<DataAccess.Models.User>()))
                .Callback<DataAccess.Models.User>(u => userSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new ConfirmUserEmailByToken(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            action.Invoke("1");
            action.Invoke("1");

            // assert
            Assert.NotNull(userSaved.EmailConfirmedOn);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Once());
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Exactly(2));
        }

        [Fact]
        public void Invoke_EmptyString_SaveNorEditCalled()
        {
            // prepare
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUserRepo = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new ConfirmUserEmailByToken(mockedUserTokenRepository.Object, mockedUserRepo.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("");

            // assert
            Assert.False(actionResult);
            mockedUserRepo.Verify(r => r.Edit(It.IsAny<DataAccess.Models.User>()), Times.Never());
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}