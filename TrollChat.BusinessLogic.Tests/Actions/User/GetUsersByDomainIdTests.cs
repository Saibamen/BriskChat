using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.User.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    public class GetDomainByUserIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var userId = Guid.NewGuid();

            var usersInDomain = new List<DataAccess.Models.User>
            {
                new DataAccess.Models.User
                {
                    Id = userId,
                    Name = "TestUser",
                    CreatedOn = DateTime.MinValue,
                    ModifiedOn = DateTime.MinValue,
                    DeletedOn = null
                },
                new DataAccess.Models.User
                {
                    Name = "TestUser2"
                }
            };

            // prepare

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(usersInDomain.AsQueryable());
            var action = new GetUsersByDomainId(mockedUserRepository.Object);

            // action
            var users = action.Invoke(userId);

            // check
            Assert.Contains(users, y => y.Id == userId);
            Assert.Equal(2, users.Count);
            Assert.Equal("TestUser", users[0].Name);
            Assert.Contains(users, y => y.Id == userId);
            Assert.Contains(users, y => y.Name == "TestUser");
            Assert.Equal(DateTime.MinValue, users[0].CreatedOn);
            Assert.Equal(DateTime.MinValue, users[0].ModifiedOn);
            Assert.Null(users[0].DeletedOn);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var action = new GetUsersByDomainId(mockedUserRepository.Object);

            // action
            var users = action.Invoke(Guid.NewGuid());

            // check
            Assert.NotNull(users);
            Assert.Empty(users);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var action = new GetUsersByDomainId(mockedUserRepository.Object);

            // action
            var users = action.Invoke(new Guid());

            // check
            Assert.Null(users);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Never);
        }
    }
}