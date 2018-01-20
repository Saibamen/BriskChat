using System;
using BriskChat.BusinessLogic.Actions.Room.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Room
{
    public class EditRoomCustomizationTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();
            var roomFromDb = new DataAccess.Models.Room
            {
                Id = guid,
                Customization = 1
            };

            // prepare
            DataAccess.Models.Room roomSaved = null;

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(roomFromDb);
            mockedRoomRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomCustomization(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(guid, 3);

            // check
            Assert.True(room);
            Assert.Equal(guid, roomSaved.Id);
            Assert.Equal(3, roomSaved.Customization);
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

            var action = new EditRoomCustomization(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), 3);

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

            var action = new EditRoomCustomization(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(new Guid(), 3);

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_Zero_ReturnsTrue()
        {
            var roomFromDb = new DataAccess.Models.Room
            {
                Customization = 3
            };

            // prepare
            DataAccess.Models.Room roomSaved = null;
            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(roomFromDb);
            mockedRoomRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomCustomization(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), 0);

            // check
            Assert.True(room);
            Assert.Equal(0, roomSaved.Customization);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_NegativeNumber_ReturnsNull()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditRoomCustomization(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var room = action.Invoke(Guid.NewGuid(), -2);

            // check
            Assert.False(room);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}