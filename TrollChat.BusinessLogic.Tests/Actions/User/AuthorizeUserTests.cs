using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Helpers.Interfaces;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using System.Linq;
using TrollChat.BusinessLogic.Actions.User.Implementations;

namespace TrollChat.BusinessLogic.Tests.Actions.User
{
    public class AuthenitcateUserTests
    {
        [Fact]
        public void Invoke_ReturnsTrue()
        {
            // prepare
            var domain = new DataAccess.Models.Domain
            {
                Name = "Test Domain"
            };

            var dataUser = new DataAccess.Models.User
            {
                Email = "email@dot.com",
                PasswordSalt = "salt-generated",
                PasswordHash = "plain-hashed"
            };

            var findByResult = new List<DataAccess.Models.User> { dataUser };
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var mockedHasher = new Mock<IHasher>();
            mockedHasher.Setup(h => h.CreatePasswordHash("plain", "salt-generated")).Returns("plain-hashed");

            var mockedDomainRepository = new Mock<IDomainRepository>();
            var findDomainResult = new List<DataAccess.Models.Domain> { domain };
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findDomainResult.AsQueryable);

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object, mockedHasher.Object);

            // action
            var user = action.Invoke("email@dot.com", "plain", "123");

            // check
            Assert.NotNull(user);
            Assert.Equal("email@dot.com", user.Email);
            Assert.Equal("Test Domain", user.Domain.Name);
        }

        [Fact]
        public void Invoke_EmptyRepository_EmailIsEmpty()
        {
            // prepare
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("", "test", "123");

            // check
            Assert.Null(user);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsFalse()
        {
            // prepare
            var findByResult = new List<DataAccess.Models.User>();
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("email@dot.com", "test", "123");

            // check
            Assert.Null(user);
        }
    }
}