using System;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    public class EditRoomNameTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();
            var roomFromDb = new DataAccess.Models.Room
            {
                Id = guid,
                Name = "OldName"
            };

            // prepare
            DataAccess.Models.Room roomSaved = null;

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(roomFromDb);
            mockedRoomRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);
            var action = new EditRoomName(mockedRoomRepository.Object);

            // action
            var room = action.Invoke(guid, "New Room Name");

            // check
            Assert.True(room);
            Assert.Equal(guid, roomSaved.Id);
            Assert.Equal("New Room Name", roomSaved.Name);
            Assert.Null(roomSaved.DeletedOn);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedRoomRepository.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var action = new EditRoomName(mockedRoomRepository.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), "test");

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var action = new EditRoomName(mockedRoomRepository.Object);

            // action
            var room = action.Invoke(new Guid(), "test");

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyString_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var action = new EditRoomName(mockedRoomRepository.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), "");

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_TooLongString_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var action = new EditRoomName(mockedRoomRepository.Object);

            // 101 characters
            const string string101 = "user left this channel user left this channel user left this channel user left this channel user lerr";
            // action
            var room = action.Invoke(Guid.NewGuid(), string101);

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}