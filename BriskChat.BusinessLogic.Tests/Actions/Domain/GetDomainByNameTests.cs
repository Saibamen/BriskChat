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
    [Collection("mapper")]
    public class GetDomainByNameTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var id = Guid.NewGuid();
            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = id,
                Name = "Name",
                CreatedOn = DateTime.MinValue,
                ModifiedOn = DateTime.MinValue,
                DeletedOn = null
            };

            // prepare
            var findByResult = new List<DataAccess.Models.Domain> { domainFromDb };

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var action = new GetDomainByName(mockedDomainRepository.Object);

            // action
            var domain = action.Invoke(id.ToString());

            // check
            Assert.Equal(id, domain.Id);
            Assert.Equal("Name", domain.Name);
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
            var action = new GetDomainByName(mockedDomainRepository.Object);

            // action
            var user = action.Invoke("test");

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Invoke_EmptyId_ReturnsNull(string name)
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var action = new GetDomainByName(mockedDomainRepository.Object);

            // action
            var user = action.Invoke(name);

            // check
            Assert.Null(user);
            mockedDomainRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()), Times.Never);
        }
    }
}