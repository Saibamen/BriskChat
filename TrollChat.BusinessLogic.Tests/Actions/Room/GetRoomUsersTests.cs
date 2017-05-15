using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.Room.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Room
{
    [Collection("mapper")]
    public class GetRoomUsersTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guidRoom = new Guid();
            var guid = new Guid();

            var domenka = new DataAccess.Models.Domain
            {
                Name = "sluzbowa"
            };

            var usersFromDb = new List<DataAccess.Models.User>
            {
                new DataAccess.Models.User {
                    Id = guid,
                    Name = "Name",
                    Email = "email@dot.com",
                    EmailConfirmedOn = DateTime.MinValue,
                    CreatedOn = DateTime.MinValue,
                    ModifiedOn = DateTime.MinValue,
                    Domain = domenka
                }
            };
            var tag = new DataAccess.Models.Tag
            {
                Name = "TestTag"
            };

            var userroomFromDb = new DataAccess.Models.Room()
            {
                Id = guidRoom,
                Tags = new List<DataAccess.Models.Tag> { tag },
                Owner = usersFromDb[0],
                Domain = domenka,
                Name = "testowy",
                Description = "Januszowa",
                Customization = 1
            };
            // prepare
            var mockedRoomRepository = new Mock<IRoomRepository>();
            mockedRoomRepository.Setup(r => r.GetRoomUsers(It.IsAny<Guid>())).Returns(usersFromDb.AsQueryable());
            var action = new GetRoomUsers(mockedRoomRepository.Object);

            //action
            var users = action.Invoke(guidRoom);

            //check
            Assert.Equal(guid, users[0].Id);
            Assert.Equal("Name", users[0].Name);
            Assert.Equal(DateTime.MinValue, users[0].ModifiedOn);
            Assert.Equal("dsds", userroomFromDb.Name);
        }
    }
}