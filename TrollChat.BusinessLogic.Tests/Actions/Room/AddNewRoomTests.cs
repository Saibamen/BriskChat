using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementation;
using TrollChat.BusinessLogic.Tests.TestConfig;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class AddNewRoomTests
    {
        [Fact]
        public void Invoke_ValidData_AddsRoomToDatabaseWithCorrectValues()
        {
            // prepare
            var roomData = new Models.RoomModel
            {
                Name = "TestRoom",
                Topic = "RoomTrool",
                Description = "TroloRoom",
                Customization = 1,
                IsPublic = true
            };
            DataAccess.Models.Room roomSaved = null;

            var mockedRoomRepo = new Mock<IRoomRepository>();
            mockedRoomRepo.Setup(r => r.Add(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);

            var action = new AddNewRoom(mockedRoomRepo.Object);

            // action
            action.Invoke(roomData);

            // assert

            Assert.Equal("TestRoom", roomSaved.Name);
            Assert.Equal("RoomTrool", roomSaved.Topic);
            Assert.Equal("TroloRoom", roomSaved.Description);
            Assert.Equal(1, roomSaved.Customization);
            Assert.Equal(true, roomSaved.IsPublic);
            mockedRoomRepo.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Once());
            mockedRoomRepo.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var roomToAdd = new Models.RoomModel();
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new AddNewRoom(mockedRoomRepository.Object);

            // action
            var actionResult = action.Invoke(roomToAdd);

            // assert
            Assert.Equal(0, actionResult);
            mockedRoomRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}