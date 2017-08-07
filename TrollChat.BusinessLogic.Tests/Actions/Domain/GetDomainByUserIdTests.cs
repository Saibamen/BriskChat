using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using TrollChat.BusinessLogic.Actions.Domain.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    [Collection("mapper")]
    public class GetDomainByUserIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var domainId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var usersInDomain = new List<DataAccess.Models.User>
            {
                new DataAccess.Models.User
                {
                    Id = userId,
                    Name = "TestUser"
                }
            };

            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = domainId,
                Name = "Name",
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null,
                Users = usersInDomain
            };

            // prepare
            var findByResult = new List<DataAccess.Models.Domain> { domainFromDb };

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var action = new GetDomainByUserId(mockedDomainRepository.Object);

            // action
            var domain = action.Invoke(userId);

            // check
            Assert.Equal(domainId, domain.Id);
            Assert.Equal("Name", domain.Name);
            Assert.Contains(domain.Users, y => y.Id == userId);
            Assert.Contains(domain.Users, y => y.Name == "TestUser");
            Assert.Equal(DateTime.MinValue, domain.CreatedOn);
            Assert.Equal(DateTime.MinValue, domain.ModifiedOn);
            Assert.Null(domain.DeletedOn);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new GetDomainByUserId(mockedDomainRepository.Object);

            // action
            var user = action.Invoke(Guid.NewGuid());

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new GetDomainByUserId(mockedDomainRepository.Object);

            // action
            var user = action.Invoke(new Guid());

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Never);
        }
    }
}