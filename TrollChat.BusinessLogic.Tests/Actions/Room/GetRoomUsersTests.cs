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
    public class GetRoomUsersTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();
            var guid = Guid.NewGuid();

            var domenka = new DataAccess.Models.Domain
            {
                Name = "sluzbowa"
            };

            var usersFromDb = new List<DataAccess.Models.User>
            {
                new DataAccess.Models.User {
                    Id = guid,
                    Name = "Name",
                    Email = "email@dot.com",
                    EmailConfirmedOn = DateTime.MinValue,
                    CreatedOn = DateTime.MinValue,
                    ModifiedOn = DateTime.MinValue,
                    Domain = domenka
                }
            };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetRoomUsers(It.IsAny<Guid>())).Returns(usersFromDb.AsQueryable());
            var action = new GetRoomUsers(mockedRoomRepository.Object);

            // action
            var users = action.Invoke(guidRoom);

            // check
            Assert.NotNull(users);
            Assert.Equal(guid, users[0].Id);
            Assert.Equal("Name", users[0].Name);
            Assert.Equal(DateTime.MinValue, users[0].ModifiedOn);
            mockedRoomRepository.Verify(r => r.GetRoomUsers(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomUsers(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetRoomUsers(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomUsers(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetRoomUsers(It.IsAny<Guid>()), Times.Never);
        }
    }
}