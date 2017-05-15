using System;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    [Collection("mapper")]
    public class GetUserByIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                Name = "Name",
                Email = "email@dot.com",
                EmailConfirmedOn = DateTime.MinValue,
                LockedOn = null,
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null
            };

            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(userFromDb);
            var action = new GetUserById(mockedUserRepository.Object);

            // action
            var user = action.Invoke(guid);

            // check
            Assert.Equal(guid, user.Id);
            Assert.Equal("Name", user.Name);
            Assert.Equal("email@dot.com", user.Email);
            Assert.Equal(DateTime.MinValue, user.EmailConfirmedOn);
            Assert.Null(user.LockedOn);
            Assert.Equal(DateTime.MinValue, user.CreatedOn);
            Assert.Equal(DateTime.MinValue, user.ModifiedOn);
            Assert.Null(user.DeletedOn);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var action = new GetUserById(mockedUserRepository.Object);

            // action
            var user = action.Invoke(Guid.NewGuid());

            // check
            Assert.Null(user);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var action = new GetUserById(mockedUserRepository.Object);

            // action
            var user = action.Invoke(new Guid());

            // check
            Assert.Null(user);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}