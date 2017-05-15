using System;
using Moq;
using TrollChat.BusinessLogic.Actions.Message.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Message
{
    [Collection("mapper")]
    public class EditMessageByIdTests
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
            DataAccess.Models.Message messageSaved = null;

            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.GetById(It.IsAny<Guid>()))
                .Returns(messageFromDb);
            mockedMessageRepository.Setup(r => r.Edit(It.IsAny<DataAccess.Models.Message>()))
                .Callback<DataAccess.Models.Message>(u => messageSaved = u);
            var action = new EditMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(guid, "Nowa wiadomość");

            // check
            Assert.True(message);
            Assert.Equal(guid, messageSaved.Id);
            Assert.Equal("Nowa wiadomość", messageSaved.Text);
            Assert.Null(messageSaved.DeletedOn);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedMessageRepository.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new EditMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), "test");

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new EditMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(new Guid(), "test");

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyString_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new EditMessageById(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), "");

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}