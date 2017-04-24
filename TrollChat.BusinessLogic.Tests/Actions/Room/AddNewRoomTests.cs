using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

//TODO: Update tests
namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class AddNewRoomTests
    {
        [Fact]
        public void Invoke_ValidData_AddsRoomToDatabaseWithCorrectValues()
        {
            // prepare
            var user = new DataAccess.Models.User
            {
                Name = "Test"
            };

            var tag = new TagModel
            {
                Name = "TestTag"
            };

            var roomData = new RoomModel
            {
                Tags = new List<TagModel> { tag },
                Name = "TestRoom",
                Topic = "RoomTrool",
                Description = "TroloRoom",
                Customization = 1,
                IsPublic = true
            };

            DataAccess.Models.Room roomSaved = null;

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.Room>()))
                .Callback<DataAccess.Models.Room>(u => roomSaved = u);

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(user);

            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();

            var action = new AddNewRoom(mockedRoomRepository.Object, mockedUserRepo.Object, mockedUserRoomRepository.Object);

            // action
            action.Invoke(roomData, new Guid());

            // assert
            Assert.Equal("TestTag", roomSaved.Tags.ElementAt(0).Name);
            Assert.Equal("TestRoom", roomSaved.Name);
            Assert.Equal("RoomTrool", roomSaved.Topic);
            Assert.Equal("TroloRoom", roomSaved.Description);
            Assert.False(roomSaved.IsPrivateConversation);
            Assert.Equal(1, roomSaved.Customization);
            Assert.True(roomSaved.IsPublic);
            Assert.Equal("Test", roomSaved.Owner.Name);
            mockedRoomRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Once());
            mockedRoomRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var roomToAdd = new RoomModel();
            var mockedRoomRepository = new Mock<IRoomRepository>();
            var mockedUserRepo = new Mock<IUserRepository>();

            var mockedUserRoomRepository = new Mock<IUserRoomRepository>();

            var action = new AddNewRoom(mockedRoomRepository.Object, mockedUserRepo.Object, mockedUserRoomRepository.Object);

            // action
            var actionResult = action.Invoke(roomToAdd, Guid.NewGuid());

            // assert
            Assert.Equal(Guid.Empty, actionResult);
            mockedRoomRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}