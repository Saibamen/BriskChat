using System;
using Moq;
using TrollChat.BusinessLogic.Actions.Message.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Message
{
    [Collection("mapper")]
    public class GetMessageByIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();
            var messageFromDb = new DataAccess.Models.Message
            {
                Id = guid,
                Text = "Testowa wiadomość",
                DeletedOn = null
            };

            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(messageFromDb);
            var action = new GetMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(guid);

            // check
            Assert.NotNull(message);
            Assert.Equal(guid, message.Id);
            Assert.Equal("Testowa wiadomość", message.Text);
            Assert.Null(message.DeletedOn);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid());

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(new Guid());

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}