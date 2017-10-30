using Moq;
using TrollChat.BusinessLogic.Actions.Email.Implementations;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;
using TrollChat.DataAccess.UnitOfWork;

namespace TrollChat.BusinessLogic.Tests.Actions.Email
{
    [Collection("mapper")]
    public class AddNewEmailMessageTests
    {
        [Fact]
        public void Invoke_ValidData_AddsEmailMessageToDatabaseWithCorrectValues()
        {
            // prepare
            var emailMessage = new EmailMessageModel
            {
                From = "from@from.from",
                Recipient = "to@to.to",
                Subject = "subject",
                Message = "message"
            };

            EmailMessage emailMessageFromDb = null;
            var mockedEmailMessageRepository = new Mock<IEmailRepository>();
            mockedEmailMessageRepository.Setup(r => r.Add(It.IsAny<EmailMessage>()))
                .Callback<EmailMessage>(u => emailMessageFromDb = u);
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewEmailMessage(mockedEmailMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(emailMessage);

            // assert
            Assert.True(result);
            Assert.Equal("from@from.from", emailMessageFromDb.From);
            Assert.Equal("to@to.to", emailMessageFromDb.Recipient);
            Assert.Equal("subject", emailMessageFromDb.Subject);
            Assert.Equal("message", emailMessageFromDb.Message);
            Assert.Equal(0, emailMessageFromDb.FailureCount);

            mockedEmailMessageRepository.Verify(r => r.Add(It.IsAny<EmailMessage>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var emailMessageToAdd = new EmailMessageModel();
            var mockedEmailMessageRepository = new Mock<IEmailRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new AddNewEmailMessage(mockedEmailMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var result = action.Invoke(emailMessageToAdd);

            // assert
            Assert.False(result);
            mockedEmailMessageRepository.Verify(r => r.Add(It.IsAny<EmailMessage>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}