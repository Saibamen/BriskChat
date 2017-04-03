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
                var userTokenFromDb = new DataAccess.Models.UserToken()
                {
                    Id = 1,
                };

                var mockedUserTokenRepository = new Mock<IUserTokenRepository>();
                mockedUserTokenRepository.Setup(r => r.GetById(1)).Returns(userTokenFromDb);

                var action = new DeleteUserTokenById(mockedUserTokenRepository.Object);

                // action
                var actionResult = action.Invoke(1);

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
                var actionResult = action.Invoke(1);

                // assert
                Assert.False(actionResult);
                mockedUserTokenRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.UserToken>()), Times.Never);
                mockedUserTokenRepository.Verify(r => r.Save(), Times.Never);
            }
        }
    }
}