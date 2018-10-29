using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.User.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.User
{
    public class AuthenitcateUserTests
    {
        [Fact]
        public void Invoke_ReturnsCorrectModel()
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
        public void Invoke_WrongPassword_ReturnsNull()
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
            mockedHasher.Setup(h => h.CreatePasswordHash("wrongPassword", "salt-generated")).Returns("wrong-hashed");

            var mockedDomainRepository = new Mock<IDomainRepository>();
            var findDomainResult = new List<DataAccess.Models.Domain> { domain };
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findDomainResult.AsQueryable);

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object, mockedHasher.Object);

            // action
            var user = action.Invoke("email@dot.com", "wrongPassword", "123");

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
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
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyUser_ReturnsNull()
        {
            // prepare
            var domain = new DataAccess.Models.Domain
            {
                Name = "Test Domain"
            };

            var mockedUserRepository = new Mock<IUserRepository>();

            var findByResult = new List<DataAccess.Models.Domain> { domain };
            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("email@dot.com", "test", "123");

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnsNull()
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
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Never);
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("d", "", "")]
        [InlineData("", "d", "")]
        [InlineData("", "", "d")]
        [InlineData("d", "d", "")]
        [InlineData("", "d", "d")]
        [InlineData("d", "", "d")]
        [InlineData(" ", " ", " ")]
        [InlineData("d", " ", " ")]
        [InlineData(" ", "d", " ")]
        [InlineData(" ", " ", "d")]
        [InlineData("d", "d", " ")]
        [InlineData(" ", "d", "d")]
        [InlineData("d", " ", "d")]
        public void Invoke_EmptyParameters_ReturnsNull(string email, string password, string domainName)
        {
            // prepare
            var findByResult = new List<DataAccess.Models.User>();
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AuthenticateUser(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke(email, password, domainName);

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Never);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Never);
        }
    }
}