using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.UserRoom.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.UserRoom
{
    [Collection("mapper")]
    public class GetUserRoomByIdsTests
    {
        [Fact]
        public void Invoke_ValidData_EditAndSaveAreCalled()
        {
            // prepare
            var user = new DataAccess.Models.User
            {
                Name = "TestUser"
            };

            var role = new DataAccess.Models.Role
            {
                Name = "TestRole"
            };

            var message = new DataAccess.Models.Message
            {
                Text = "TestMessage"
            };

            var room = new DataAccess.Models.Room();

            var userRoomInDb = new List<DataAccess.Models.UserRoom>
            {
                new DataAccess.Models.UserRoom
                {
                    Messages = new List<DataAccess.Models.Message> { message },
                    LastMessage = message,
                    Room = room,
                    User = user,
                    Role = role,
                    LockedUntil = DateTime.MaxValue
                }
            };

            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();
            mockedUserRoomRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserRoom, bool>>>()))
                .Returns(userRoomInDb.AsQueryable());

            var action = new GetUserRoomByIds(mockedUserRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid(), Guid.NewGuid());

            // assert
            Assert.NotNull(result);
            Assert.Equal("TestMessage", result.Messages[0].Text);
            //Assert.Equal("TestUser", result.Room.Users[0].Name);
            Assert.Equal("TestUser", result.User.Name);
            Assert.Equal("TestRole", result.Role.Name);
            Assert.Equal(DateTime.MaxValue, result.LockedUntil);
            mockedUserRoomRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserRoom, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();

            var action = new GetUserRoomByIds(mockedUserRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid(), Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedUserRoomRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserRoom, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();

            var action = new GetUserRoomByIds(mockedUserRoomRepository.Object);

            // action
            var result = action.Invoke(new Guid(), new Guid());

            // assert
            Assert.Null(result);
            mockedUserRoomRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.UserRoom, bool>>>()), Times.Never);
        }
    }
}