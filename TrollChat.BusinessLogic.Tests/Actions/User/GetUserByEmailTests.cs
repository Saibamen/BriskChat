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
            var guid = Guid.NewGuid();
            var domain = new DataAccess.Models.Domain
            {
                Name = "Test Domain"
            };
            var userFromDb = new DataAccess.Models.User
            {
                Id = guid,
                Name = "Name",
                Email = "email@dot.com",
                PasswordHash = "123",
                Domain = domain
            };

            var findByResult = new List<DataAccess.Models.User> { userFromDb };
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var findDomainResult = new List<DataAccess.Models.Domain> { domain };
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findDomainResult.AsQueryable);
            var action = new GetUserByEmail(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("email@dot.com", "Test Domain");

            // check
            Assert.NotNull(user);
            Assert.Equal(guid, user.Id);
            Assert.Equal("Name", user.Name);
            Assert.Equal("email@dot.com", user.Email);
            Assert.Equal("Test Domain", user.Domain.Name);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_WrongDomain_ReturnsNull()
        {
            // prepare
            var domainInDatabase = new DataAccess.Models.Domain
            {
                Name = "Database Domain"
            };

            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var findDomainResult = new List<DataAccess.Models.Domain> { domainInDatabase };
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findDomainResult.AsQueryable);
            var action = new GetUserByEmail(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("email@dot.com", "Database Domain");

            // check
            Assert.Null(user);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Once);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyString()
        {
            // prepare
            var findByResult = new List<DataAccess.Models.User>();
            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new GetUserByEmail(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("", "");

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
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new GetUserByEmail(mockedUserRepository.Object, mockedDomainRepository.Object);

            // action
            var user = action.Invoke("whatislove@wp.pl", "");

            // check
            Assert.Null(user);
            mockedUserRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.User, bool>>>()), Times.Never);
        }
    }
}