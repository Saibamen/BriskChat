using Moq;
using TrollChat.BusinessLogic.Actions.Email.Implementations;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Email
{
    public class DeleteEmailMessageTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare
            var messageFromDb = new EmailMessage()
            {
                Id = 1
            };

            var mockedEmailRepository = new Mock<IEmailRepository>();
            mockedEmailRepository.Setup(r => r.GetById(1)).Returns(messageFromDb);

            var action = new DeleteEmailMessageById(mockedEmailRepository.Object);

            // action
            var result = action.Invoke(1);

            // assert
            Assert.True(result);
            mockedEmailRepository.Verify(r => r.Delete(It.IsAny<EmailMessage>()), Times.Once());
            mockedEmailRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedEmailRepository = new Mock<IEmailRepository>();

            var action = new DeleteEmailMessageById(mockedEmailRepository.Object);

            // action
            var result = action.Invoke(1);

            // assert
            Assert.False(result);
            mockedEmailRepository.Verify(r => r.Delete(It.IsAny<EmailMessage>()), Times.Never);
            mockedEmailRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}