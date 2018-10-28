using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.Domain.Implementations;
using BriskChat.BusinessLogic.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Domain
{
    [Collection("mapper")]
    public class AddNewDomainTests
    {
        [Fact]
        public void Invoke_ValidData_AddsDomainToDatabaseWithCorrectValues()
        {
            // prepare
            var domainData = new DomainModel
            {
                Name = "testdomain"
            };

            DataAccess.Models.Domain domainSaved = null;

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.Domain>()))
                .Callback<DataAccess.Models.Domain>(u => domainSaved = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewDomain(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            action.Invoke(domainData, Guid.NewGuid());

            // assert
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Exactly(1));
            Assert.Equal("testdomain", domainSaved.Name);
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var domainToAdd = new DomainModel();
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewDomain(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(domainToAdd, Guid.NewGuid());

            // assert
            Assert.Equal(Guid.Empty, actionResult);
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
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
                Name = "testdomain"
            };
            var findByResult = new List<DataAccess.Models.Domain> { domainFromDb };

            var mockedDomainRepository = new Mock<IDomainRepository>();

            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
                .Returns(findByResult.AsQueryable());
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewDomain(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            var actionResult = action.Invoke(domainToAdd, Guid.NewGuid());

            // assert
            Assert.Equal(Guid.Empty, actionResult);
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}