using System;
using Xunit;
using TrollChat.DataAccess.Repositories.Interfaces;
using Moq;
using TrollChat.BusinessLogic.Actions.Domain.Implementations;

namespace TrollChat.BusinessLogic.Tests.Actions.Domain
{
    public class DeleteDomainByIdTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare

            var guid = new Guid();
            var domainFromDb = new DataAccess.Models.Domain
            {
                Id = guid
            };

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.GetById(guid)).Returns(domainFromDb);

            var action = new DeleteDomainById(mockedDomainRepository.Object);

            // action
            var result = action.Invoke(guid);

            // assert
            Assert.True(result);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Once());
            mockedDomainRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedDomainRepository = new Mock<IDomainRepository>();

            var action = new DeleteDomainById(mockedDomainRepository.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}