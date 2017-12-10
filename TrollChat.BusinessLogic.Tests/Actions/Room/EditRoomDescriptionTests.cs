using System;
using BriskChat.BusinessLogic.Actions.Room.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Room
{
    public class EditRoomDescriptionTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();
            var roomFromDb = new DataAccess.Models.Room
            {
                Id = guid,
                Description = "OldDesc"
            };

            // prepare
            DataAccess.Models.Room roomSaved = null;

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(roomFromDb);
            mockedRoomRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomDescription(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(guid, "New Room Desc");

            // check
            Assert.True(room);
            Assert.Equal(guid, roomSaved.Id);
            Assert.Equal("New Room Desc", roomSaved.Description);
            Assert.Null(roomSaved.DeletedOn);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomDescription(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), "test");

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomDescription(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(new Guid(), "test");

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyString_ReturnsTrue()
        {
            var roomFromDb = new DataAccess.Models.Room
            {
                Description = "OldDesc"
            };

            // prepare
            DataAccess.Models.Room roomSaved = null;
            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(roomFromDb);
            mockedRoomRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomDescription(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), "");

            // check
            Assert.True(room);
            Assert.Equal("", roomSaved.Description);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_TooLongString_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomDescription(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // 101 characters
            const string string101 = "user left this channel user left this channel user left this channel user left this channel user lerr";
            // action
            var room = action.Invoke(Guid.NewGuid(), string101);

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}