using System;
using Moq;
using TrollChat.BusinessLogic.Actions.UserToken.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.UserToken
{
    public class DeleteUserTokenByIdTests
    {
        public class DeleteUserTokenByTokenStringTests
        {
            [Fact]
            public void Invoke_ValidData_DeleteAndSaveAreCalled()
            {
                // prepare
                var guid = Guid.NewGuid();
                var userTokenFromDb = new DataAccess.Models.UserToken
                {
                    Id = guid
                };

                var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
                mockedUserTokenRepository.Setup(r => r.GetById(guid)).Returns(userTokenFromDb);

                var action = new DeleteUserTokenById(mockedUserTokenRepository.Object);

                // action
                var actionResult = action.Invoke(guid);

                // assert
                Assert.True(actionResult);
                mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Once());
                mockedUserTokenRepository.Verify(r => r.Save(), Times.Once());
            }

            [Fact]
            public void Invoke_ValidData_DeleteNorSaveAreCalled()
            {
                var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

                var action = new DeleteUserTokenById(mockedUserTokenRepository.Object);

                // action
                var actionResult = action.Invoke(Guid.NewGuid());

                // assert
                Assert.False(actionResult);
                mockedUserTokenRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
                mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
                mockedUserTokenRepository.Verify(r => r.Save(), Times.Never);
            }

            [Fact]
            public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
            {
                var mockedUserTokenRepository = new Mock<IUserTokenRepository>();

                var action = new DeleteUserTokenById(mockedUserTokenRepository.Object);

                // action
                var actionResult = action.Invoke(new Guid());

                // assert
                Assert.False(actionResult);
                mockedUserTokenRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
                mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
                mockedUserTokenRepository.Verify(r => r.Save(), Times.Never);
            }
        }
    }
}