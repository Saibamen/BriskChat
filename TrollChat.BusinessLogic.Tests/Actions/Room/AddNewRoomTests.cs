using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementation;
using TrollChat.BusinessLogic.Models;
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
            var user = new UserModel
            {
                Name = "Test"
            };

            var tag = new TagModel
            {
                Name = "TestTag"
            };

            var roomData = new RoomModel
            {
                Owner = user,
                Tags = new List<TagModel> { tag },
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
            Assert.Equal("Test", roomSaved.Owner.Name);
            Assert.Equal("TestTag", roomSaved.Tags.ElementAt(0).Name);
            Assert.Equal("TestRoom", roomSaved.Name);
            Assert.Equal("RoomTrool", roomSaved.Topic);
            Assert.Equal("TroloRoom", roomSaved.Description);
            Assert.False(roomSaved.IsPrivateConversation);
            Assert.Equal(1, roomSaved.Customization);
            Assert.True(roomSaved.IsPublic);
            mockedRoomRepo.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Once());
            mockedRoomRepo.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var roomToAdd = new RoomModel();
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