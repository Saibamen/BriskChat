using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.Domain.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Domain
{
    public class CheckDomainExistByNameTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = Guid.NewGuid(),
                Name = "Name"
            };

            // prepare
            var findByResult = new List<DataAccess.Models.Domain> { domainFromDb };
            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findByResult.AsQueryable);
            var action = new CheckDomainExistsByName(mockedDomainRepository.Object);

            // action
            var domain = action.Invoke("Name");

            // check
            Assert.True(domain);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new CheckDomainExistsByName(mockedDomainRepository.Object);

            // action
            var domain = action.Invoke("test");

            // check
            Assert.False(domain);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Invoke_EmptyId_ReturnsNull(string name)
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new CheckDomainExistsByName(mockedDomainRepository.Object);

            // action
            var domain = action.Invoke(name);

            // check
            Assert.False(domain);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Never);
        }
    }
}