using System;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementation;
using TrollChat.BusinessLogic.Tests.TestConfig;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    public class GetUserByIdTests : IClassFixture<AutoMapperFixture>
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var userFromDb = new DataAccess.Models.User()
            {
                Id = 1,
                Name = "Name",
                Email = "email@dot.com",
                EmailConfirmedOn = DateTime.MinValue,
                LockedOn = null,
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null,
            };

            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(userFromDb);
            var action = new GetUserById(mockedUserRepository.Object);

            // action
            var user = action.Invoke(1);

            // check
            Assert.Equal(1, user.Id);
            Assert.Equal("Name", user.Name);
            Assert.Equal("email@dot.com", user.Email);
            Assert.Equal(DateTime.MinValue, user.EmailConfirmedOn);
            Assert.Null(user.LockedOn);
            Assert.Equal(DateTime.MinValue, user.CreatedOn);
            Assert.Equal(DateTime.MinValue, user.ModifiedOn);
            Assert.Null(user.DeletedOn);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var action = new GetUserById(mockedUserRepository.Object);

            // action
            var user = action.Invoke(1337);

            // check
            Assert.Null(user);
        }
    }
}