﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserRoomsTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var guidRoom = Guid.NewGuid();
            var guid = Guid.NewGuid();

            var userRoomsFromDb = new List<DataAccess.Models.Room>
            {
                new DataAccess.Models.Room {
                    Id = guid,
                    Name = "TestRoom"
                }
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetUserRooms(It.IsAny<Guid>(), false)).Returns(userRoomsFromDb.AsQueryable());
            var action = new GetUserRooms(mockedUserRepository.Object);

            // action
            var result = action.Invoke(guidRoom);

            // check
            Assert.NotNull(result);
            Assert.Equal(guid, result[0].Id);
            Assert.Equal("TestRoom", result[0].Name);
            mockedUserRepository.Verify(r => r.GetUserRooms(It.IsAny<Guid>(), false), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserRooms(mockedUserRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.Null(result);
            mockedUserRepository.Verify(r => r.GetUserRooms(It.IsAny<Guid>(), false), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();

            var action = new GetUserRooms(mockedUserRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.Null(result);
            mockedUserRepository.Verify(r => r.GetUserRooms(It.IsAny<Guid>(), false), Times.Never);
        }
    }
}