using System;
using BriskChat.BusinessLogic.Actions.Email.Implementations;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Email
{
    public class DeleteEmailMessageTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare
            var guid = Guid.NewGuid();
            var messageFromDb = new EmailMessage
            {
                Id = guid
            };

            var mockedEmailRepository = new Mock<IEmailRepository>();
            mockedEmailRepository.Setup(r => r.GetById(guid)).Returns(messageFromDb);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteEmailMessageById(mockedEmailRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(guid);

            // assert
            Assert.True(result);
            mockedEmailRepository.Verify(r => r.Delete(It.IsAny<EmailMessage>()), Times.Once());
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedEmailRepository = new Mock<IEmailRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteEmailMessageById(mockedEmailRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedEmailRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once());
            mockedEmailRepository.Verify(r => r.Delete(It.IsAny<EmailMessage>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedEmailRepository = new Mock<IEmailRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new DeleteEmailMessageById(mockedEmailRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.False(result);
            mockedEmailRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedEmailRepository.Verify(r => r.Delete(It.IsAny<EmailMessage>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}