using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserPrivateConversationsTargetsTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();
            var guid = Guid.NewGuid();

            var roomsFromDb = new List<DataAccess.Models.UserRoom>
            {
                new DataAccess.Models.UserRoom {
                    Id = guid
                }
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>())).Returns(roomsFromDb.AsQueryable());
            var action = new GetUserPrivateConversationsTargets(mockedUserRepository.Object);

            // action
            var result = action.Invoke(guidRoom);

            // check
            Assert.NotNull(result);
            Assert.Equal(guid, result[0].Id);
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserPrivateConversationsTargets(mockedUserRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Equal(0, result.Count);
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserPrivateConversationsTargets(mockedUserRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
        }
    }
}