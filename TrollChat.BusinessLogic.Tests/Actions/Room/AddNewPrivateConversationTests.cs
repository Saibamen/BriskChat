using System;
using System.Collections.Generic;
using System.Linq;
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
            var issuerId = Guid.NewGuid();

            var issuer = new DataAccess.Models.User
            {
                Id = issuerId,
                Name = "IssuerName"
            };

            var user1 = new DataAccess.Models.User
            {
                Id = Guid.NewGuid(),
                Name = "User1"
            };

            var user2 = new DataAccess.Models.User
            {
                Id = Guid.NewGuid(),
                Name = "User2"
            };

            var users = new List<Guid> { user1.Id, user2.Id };

            // UserRoom with User not listed in users var
            var userRoomList = new List<DataAccess.Models.UserRoom>
            {
                new DataAccess.Models.UserRoom
                {
                    User = new DataAccess.Models.User
                    {
                        Id = Guid.NewGuid(),
                        Name = "OtherUser"
                    },
                    Room = new DataAccess.Models.Room
                    {
                        IsPrivateConversation = true,
                        IsPublic = false
                    }
                }
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(issuer);

            mockedUserRepository.Setup(r => r.GetAll())
                .Returns(new List<DataAccess.Models.User> { user1, user2 }.AsQueryable);

            mockedUserRepository.Setup(r => r.GetPrivateConversationsTargets(issuerId))
                .Returns(userRoomList.AsQueryable);

            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(issuerId, users);

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
            Assert.Equal("User1, User2, IssuerName", result.Name);
        }

        [Fact]
        public void Invoke_UserRoomWithOneUserFromUserList_ReturnsCorrectModel()
        {
            // prepare
            var issuerId = Guid.NewGuid();

            var issuer = new DataAccess.Models.User
            {
                Id = issuerId,
                Name = "IssuerName"
            };

            var user1 = new DataAccess.Models.User
            {
                Id = Guid.NewGuid(),
                Name = "User1"
            };

            var user2 = new DataAccess.Models.User
            {
                Id = Guid.NewGuid(),
                Name = "User2"
            };

            var users = new List<Guid> { user1.Id, user2.Id };

            // UserRoom with user2
            var userRoomList = new List<DataAccess.Models.UserRoom>
            {
                new DataAccess.Models.UserRoom
                {
                    User = user2,
                    Room = new DataAccess.Models.Room
                    {
                        IsPrivateConversation = true,
                        IsPublic = false
                    }
                }
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(issuer);

            mockedUserRepository.Setup(r => r.GetAll())
                .Returns(new List<DataAccess.Models.User> { user1, user2 }.AsQueryable);

            mockedUserRepository.Setup(r => r.GetPrivateConversationsTargets(issuerId))
                .Returns(userRoomList.AsQueryable);

            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(issuerId, users);

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
            Assert.Equal("User1, User2, IssuerName", result.Name);
        }

        [Fact]
        public void Invoke_TooMuchUsers_ReturnsNull()
        {
            // prepare
            var issuerId = Guid.NewGuid();

            // 9
            var users = new List<Guid>
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(issuerId, users);

            // check
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Never);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Never);
            Assert.Null(result);
        }

        [Fact]
        public void Invoke_DuplicateUsers_ReturnsNull()
        {
            // prepare
            var issuerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var users = new List<Guid> { userId, userId };

            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(issuerId, users);

            // check
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetAll(), Times.Never);
            mockedDomainRepository.Verify(r => r.GetDomainByUserId(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
            mockedUserRoomRepository.Verify(r => r.Save(), Times.Never);
            Assert.Null(result);
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
            mockedUserRepository.Verify(r => r.GetPrivateConversationsTargets(It.IsAny<Guid>()), Times.Never);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
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
        public void Invoke_EmptyIssuerGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(new Guid(), new List<Guid> { Guid.NewGuid() });

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
        public void Invoke_EmptyUsersGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewPrivateConversation(mockedRoomRepository.Object, mockedUserRepository.Object,
                mockedUserRoomRepository.Object, mockedDomainRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid(), new List<Guid> { Guid.NewGuid(), new Guid() });

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