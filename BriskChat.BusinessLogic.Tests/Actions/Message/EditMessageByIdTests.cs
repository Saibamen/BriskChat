using System;
using BriskChat.BusinessLogic.Actions.Message.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using BriskChat.DataAccess.UnitOfWork;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Message
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
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditMessageById(mockedMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var message = action.Invoke(guid, "Nowa wiadomość");

            // check
            Assert.True(message);
            Assert.Equal(guid, messageSaved.Id);
            Assert.Equal("Nowa wiadomość", messageSaved.Text);
            Assert.Null(messageSaved.DeletedOn);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditMessageById(mockedMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), "test");

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditMessageById(mockedMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var message = action.Invoke(new Guid(), "test");

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Invoke_EmptyString_ReturnsNull(string messageText)
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var mockedUnitOfWork = new Mock<IUnitOfWork>();

            var action = new EditMessageById(mockedMessageRepository.Object, mockedUnitOfWork.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), messageText);

            // check
            Assert.False(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedUnitOfWork.Verify(r => r.Save(), Times.Never);
        }
    }
}