using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Implementations;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserByEmailTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var userFromDb = new DataAccess.Models.User()
            {
                Id = 1,
                Name = "Name",
                Email = "email@dot.com",
                PasswordHash = "123",
            };

            var findByResult = new List<DataAccess.Models.User>() { userFromDb };
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var action = new GetUserByEmail(mockedUserRepository.Object);

            // action
            var user = action.Invoke("email@dot.com");

            // check
            Assert.Equal(1, user.Id);
            Assert.Equal("Name", user.Name);
            Assert.Equal("email@dot.com", user.Email);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyString()
        {
            // prepare
            var findByResult = new List<DataAccess.Models.User>();
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var action = new GetUserByEmail(mockedUserRepository.Object);

            // action
            var user = action.Invoke("");

            // check
            Assert.Null(user);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Never);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var findByResult = new List<DataAccess.Models.User>();
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var action = new GetUserByEmail(mockedUserRepository.Object);

            // action
            var user = action.Invoke("whatislove@wp.pl");

            // check
            Assert.Null(user);
        }
    }
}