using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Models;
using Xunit;
using Moq;
using TrollChat.DataAccess.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Linq;
using TrollChat.BusinessLogic.Actions.Domain.Implementations;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    [Collection("mapper")]
    public class AddNewDomainTests
    {
        [Fact]
        public void Invoke_ValidData_AddsDomainToDatabaseWithCorrectValues()
        {
            // prepare
            var userData = new DataAccess.Models.User
            {
                Name = "Test"
            };
            var domainData = new DomainModel
            {
                Name = "testdomain",
            };

            DataAccess.Models.Domain domainSaved = null;

            var mockedDomainrepository = new Mock<IDomainRepository>();
            mockedDomainrepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.Domain>()))
                .Callback<DataAccess.Models.Domain>(u => domainSaved = u);

            var mockedUserRepo = new Mock<IUserRepository>();
            mockedUserRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(userData);

            var action = new AddNewDomain(mockedDomainrepository.Object, mockedUserRepo.Object);

            // action
            action.Invoke(domainData, 1);

            // assert
            mockedDomainrepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedDomainrepository.Verify(r => r.Save(), Times.Exactly(1));
            Assert.Equal("testdomain", domainSaved.Name);
            Assert.Equal("Test", domainSaved.Owner.Name);
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var domainToAdd = new DomainModel();
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUserRepo = new Mock<IUserRepository>();

            var action = new AddNewDomain(mockedDomainRepository.Object, mockedUserRepo.Object);

            // action
            var actionResult = action.Invoke(domainToAdd, 1);

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
            var mockedUserRepo = new Mock<IUserRepository>();

            mockedDomainRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<DataAccess.Models.Domain, bool>>>()))
            .Returns(findByResult.AsQueryable());

            var action = new AddNewDomain(mockedDomainRepository.Object, mockedUserRepo.Object);

            // action
            var actionResult = action.Invoke(domainToAdd, 1);

            // assert
            Assert.Equal(0, actionResult);
            mockedDomainRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}