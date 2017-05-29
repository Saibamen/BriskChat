using System;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class GetRoomByIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();
            var guid = Guid.NewGuid();

            var roomFromDb = new DataAccess.Models.Room
            {
                Id = guid,
                Name = "TestRoom"
            };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(roomFromDb);
            var action = new GetRoomById(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(guidRoom);

            // check
            Assert.NotNull(result);
            Assert.Equal(guid, result.Id);
            Assert.Equal("TestRoom", result.Name);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomById(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new GetRoomById(mockedRoomRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}