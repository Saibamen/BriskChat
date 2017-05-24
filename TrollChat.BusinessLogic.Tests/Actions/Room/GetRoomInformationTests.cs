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
    public class GetRoomInformationTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guid = Guid.NewGuid();

            var owner = new DataAccess.Models.User
            {
                Name = "OwnerName",
                Email = "test@test.com"
            };

            var roomsFromDb = new List<DataAccess.Models.Room>
            {
                new DataAccess.Models.Room {
                    Id = guid,
                    Name = "TestRoom",
                    Owner = owner
                }
            };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetRoomInformation(It.IsAny<Guid>())).Returns(roomsFromDb.AsQueryable());
            var action = new GetRoomInformation(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(guid);

            // check
            Assert.NotNull(result);
            Assert.Equal(guid, result.Id);
            Assert.Equal("TestRoom", result.Name);
            Assert.Equal("OwnerName", result.Owner.Name);
            Assert.Equal("test@test.com", result.Owner.Email);
            mockedRoomRepository.Verify(r => r.GetRoomInformation(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomInformation(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetRoomInformation(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomInformation(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetRoomInformation(It.IsAny<Guid>()), Times.Never);
        }
    }
}