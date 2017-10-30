using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.UserToken.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Tests.Actions.UserToken
{
    public class DeleteUserTokenByTokenStringTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            var guid = new Guid();
            var userTokenFromDb = new DataAccess.Models.UserToken
            {
                Id = guid
            };

            var findByResult = new List<DataAccess.Models.UserToken> { userTokenFromDb };

            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            mockedUserTokenRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserTokenByTokenString(mockedUserTokenRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("123");

            // assert
            Assert.True(actionResult);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserTokenByTokenString(mockedUserTokenRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("123");

            // assert
            Assert.False(actionResult);
            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Once);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyString_DeleteNorSaveAreCalled()
        {
            var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteUserTokenByTokenString(mockedUserTokenRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke("");

            // assert
            Assert.False(actionResult);
            mockedUserTokenRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserToken, bool>>>()), Times.Never);
            mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}