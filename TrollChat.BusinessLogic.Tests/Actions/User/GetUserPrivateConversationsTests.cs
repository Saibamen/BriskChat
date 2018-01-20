using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.User.Implementations;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserPrivateConversationsTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();
            var guid = Guid.NewGuid();

            var tag = new Tag
            {
                Name = "TestTag"
            };

            var room = new DataAccess.Models.Room
            {
                Name = "TestRoom"
            };

            var lastMessage = new DataAccess.Models.Message
            {
                Text = "LastMessage"
            };

            var userRoomsFromDb = new List<DataAccess.Models.UserRoom>
            {
                new DataAccess.Models.UserRoom {
                    Id = guid,
                    Room = room,
                    LastMessage = lastMessage,
                    Tags = new List<Tag> { tag }
                }
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetPrivateConversations(It.IsAny<Guid>())).Returns(userRoomsFromDb.AsQueryable());
            var action = new GetUserPrivateConversations(mockedUserRepository.Object);

            // action
            var result = action.Invoke(guidRoom);

            // check
            Assert.NotNull(result);
            Assert.Equal(guid, result[0].Id);
            Assert.Equal("TestTag", result[0].Tags[0].Name);
            Assert.Equal("TestRoom", result[0].Room.Name);
            Assert.Equal("LastMessage", result[0].LastMessage.Text);
            mockedUserRepository.Verify(r => r.GetPrivateConversations(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserPrivateConversations(mockedUserRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedUserRepository.Verify(r => r.GetPrivateConversations(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserPrivateConversations(mockedUserRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedUserRepository.Verify(r => r.GetPrivateConversations(It.IsAny<Guid>()), Times.Never);
        }
    }
}