using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Models;
using Xunit;
using Moq;
using TrollChat.DataAccess.Repositories.Interfaces;
using TrollChat.BusinessLogic.Actions.Domain.Implementation;
using System.Linq.Expressions;
using System.Linq;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    [Collection("mapper")]
    public class AddNewDomainTests
    {
        [Fact]
        public void Invoke_ValidData_AddsDomainToDatabaseWithCorrectValues()
        {
            // prepare
            var userData = new UserModel
            {
                Email = "email",
                Password = "plain",
                Name = "Grzegorz"
            };
            var domainData = new DomainModel
            {
                Name = "testdomain",
                Owner = userData
            };
            DataAccess.Models.Domain domainSaved = null;

            var mockedDomainRepo = new Mock<IDomainRepository>();
            mockedDomainRepo.Setup(r => r.Add(It.IsAny<DataAccess.Models.Domain>()))
                .Callback<DataAccess.Models.Domain>(u => domainSaved = u);

            var action = new AddNewDomain(mockedDomainRepo.Object);

            // action
            action.Invoke(domainData);

            // assert
            Assert.Equal("testdomain", domainSaved.Name);
            Assert.Equal("email", domainSaved.Owner.Email);
            mockedDomainRepo.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedDomainRepo.Verify(r => r.Save(), Times.Exactly(1));
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var domainToAdd = new DomainModel();
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new AddNewDomain(mockedDomainRepository.Object);

            // action
            var actionResult = action.Invoke(domainToAdd);

            // assert
            Assert.Equal(0, actionResult);
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_AlreadyExists_AddNorSaveAreCalled()
        {
            // prepare
            var userData = new UserModel
            {
                Email = "email",
                Password = "plain",
                Name = "Grzegorz"
            };
            var domainToAdd = new DomainModel
            {
                Name = "testdomain",
                Owner = userData
            };
            var domainFromDb = new DataAccess.Models.Domain
            {
                Name = "testdomain",
            };
            var findByResult = new List<DataAccess.Models.Domain> { domainFromDb };

            var mockedDomainRepository = new Mock<IDomainRepository>();

            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
            .Returns(findByResult.AsQueryable());

            var action = new AddNewDomain(mockedDomainRepository.Object);

            // action
            var actionResult = action.Invoke(domainToAdd);

            // assert
            Assert.Equal(0, actionResult);
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}