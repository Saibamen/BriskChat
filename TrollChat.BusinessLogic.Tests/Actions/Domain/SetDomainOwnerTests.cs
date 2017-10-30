using System;
using Xunit;
using TrollChat.DataAccess.Repositories.Interfaces;
using Moq;
using TrollChat.BusinessLogic.Actions.Domain.Implementations;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    public class SetDomainOwnerTests
    {
        [Fact]
        public void Invoke_ValidData_EditAndSaveAreCalled()
        {
            // prepare
            var domainId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = domainId,
                Owner = null
            };

            var userFromDb = new DataAccess.Models.User
            {
                Id = userId,
                Name = "TestUser"
            };

            DataAccess.Models.Domain domainSaved = null;

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.GetById(domainId)).Returns(domainFromDb);
            mockedDomainRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Domain>()))
                .Callback<DataAccess.Models.Domain>(u => domainSaved = u);

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(userId)).Returns(userFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new SetDomainOwner(mockedDomainRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(userId, domainId);

            // assert
            Assert.True(result);
            Assert.Equal(userId, domainSaved.Owner.Id);
            Assert.Equal("TestUser", domainSaved.Owner.Name);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_EmptyUser_EditNorSaveAreCalled()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new SetDomainOwner(mockedDomainRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(Guid.NewGuid(), Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyDomain_EditNorSaveAreCalled()
        {
            // prepare
            var userId = Guid.NewGuid();
            var userFromDb = new DataAccess.Models.User
            {
                Id = userId
            };

            var mockedDomainRepository = new Mock<IDomainRepository>();

            var mockedUserRepository = new Mock<IUserRepository>();
            mockedUserRepository.Setup(r => r.GetById(userId)).Returns(userFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new SetDomainOwner(mockedDomainRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(userId, Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_EditNorSaveAreCalled()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUserRepository = new Mock<IUserRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new SetDomainOwner(mockedDomainRepository.Object, mockedUserRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(new Guid(), new Guid());

            // assert
            Assert.False(result);
            mockedUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Edit(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}