using Moq;
using System;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.DataAccess.UnitOfWork;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    public class DeleteRoomByIdTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare
            var guid = Guid.NewGuid();
            var roomFromDb = new DataAccess.Models.Room
            {
                Id = guid
            };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetById(guid)).Returns(roomFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteRoomById(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(guid);

            // assert
            Assert.True(result);
            mockedRoomRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Room>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteRoomById(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedRoomRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Room>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteRoomById(mockedRoomRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.False(result);
            mockedRoomRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Room>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
            mockedRoomRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}