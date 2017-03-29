using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Room.Implementation;
using TrollChat.DataAccess.Repositories.Interfaces;
using Moq;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using Xunit;
using System.Linq.Expressions;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    public class AddNewRoomTests
    {
        [Fact]
        public void Invoke_ValidData_AddsRoomToDatabaseWithCorrectValues()
        {
            // prepare
            var roomData = new Models.Room
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
        public void Invoke_ValidData_AddAndSaveAreCalled()
        {
            // prepare
            var roomToAdd = new Models.Room { Name = "TestTrool", Topic = "Trollchat", Description = "Romms", Customization = 2, IsPublic = true };
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new AddNewRoom(mockedRoomRepository.Object);

            // action
            action.Invoke(roomToAdd);

            // assert
            mockedRoomRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Once());
            mockedRoomRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var roomToAdd = new Models.Room();
            var mockedRoomRepository = new Mock<IRoomRepository>();

            var action = new AddNewRoom(mockedRoomRepository.Object);

            // action
            var actionResult = action.Invoke(roomToAdd);

            // assert
            Assert.Equal(0, actionResult);
            mockedRoomRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Room>()), Times.Never);
            mockedRoomRepository.Verify(r => r.Save(), Times.Never);
        }

        // [Fact]
        public void Invoke_AlreadyExists_AddNorSaveAreCalled()
        {
            // prepare
            var roomToAdd = new Models.Room
            {
                Name = "TestRoom",
                Topic = "RoomTrool",
                Description = "TroloRoom",
                Customization = 1,
                IsPublic = true
            };
            var roomFromDb = new DataAccess.Models.Room
            {
                Name = "TestRoom",
            };
            var findByResult = new List<DataAccess.Models.Room> { roomFromDb };

            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Room, bool>>>()))
                .Returns(findByResult.AsQueryable());

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