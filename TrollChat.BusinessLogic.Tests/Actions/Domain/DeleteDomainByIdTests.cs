using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Models;
using Xunit;
using TrollChat.DataAccess.Models;
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
            var DomainFromDb = new DataAccess.Models.Domain()
            {
                Id = 1
            };

            var mockedDomainRepository = new Mock<IDomainRepository>();
            mockedDomainRepository.Setup(r => r.GetById(1)).Returns(DomainFromDb);

            var action = new DeleteDomainById(mockedDomainRepository.Object);

            // action
            var result = action.Invoke(1);

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
            var result = action.Invoke(1);

            // assert
            Assert.False(result);
            mockedDomainRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Domain>()), Times.Never);
            mockedDomainRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}