using System;
using Xunit;
using TrollChat.DataAccess.Repositories.Interfaces;
using Moq;
using TrollChat.BusinessLogic.Actions.Domain.Implementations;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    public class DeleteDomainByIdTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare

            var guid = Guid.NewGuid();
            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = guid
            };

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.GetById(guid)).Returns(domainFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteDomainById(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(guid);

            // assert
            Assert.True(result);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteDomainById(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteDomainById(mockedDomainRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.False(result);
            mockedDomainRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}