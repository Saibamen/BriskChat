using System;
using System.Collections.Generic;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class AddNewPrivateConversationTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();

            var issuer = new DataAccess.Models.User
            {
                Name = "IssuerName"
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(issuer);

            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(guidRoom, new List<Guid>());

            // check
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Once);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Once);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Once);
            mockedRoomRepository.Verify(r => r.Save(), Times.Once);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.IsPrivateConversation);
            Assert.False(result.IsPublic);
            Assert.Equal("IssuerName", result.Owner.Name);
            Assert.Equal("IssuerName", result.Users[0].Name);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid(), new List<Guid>());

            // assert
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Once);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Never);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public void Invoke_ConversationWithHimself_ReturnsNull()
        {
            // prepare
            var guid = Guid.NewGuid();

            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(guid, new List<Guid> { guid });

            // assert
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Never);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(new Guid(), new List<Guid>());

            // assert
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Never);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Never);
            Assert.Null(result);
        }
    }
}