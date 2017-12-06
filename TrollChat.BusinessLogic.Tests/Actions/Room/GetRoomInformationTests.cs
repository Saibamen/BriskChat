using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Room.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class GetRoomInformationTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var owner = new DataAccess.Models.User
            {
                Name = "OwnerName"
            };

            var roomsFromDb = new List<DataAccess.Models.Room>
            {
                new DataAccess.Models.Room {
                    Name = "TestRoom",
                    Owner = owner
                }
            };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetRoomInformation(It.IsAny<Guid>())).Returns(roomsFromDb.AsQueryable());
            var action = new GetRoomInformation(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // check
            Assert.NotNull(result);
            Assert.Equal("TestRoom", result.Name);
            Assert.Equal("OwnerName", result.Owner.Name);
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