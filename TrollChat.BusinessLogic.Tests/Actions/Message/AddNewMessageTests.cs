using System;
using Moq;
using TrollChat.BusinessLogic.Actions.Message.Implementations;
using TrollChat.BusinessLogic.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Message
{
    [Collection("mapper")]
    public class AddNewMessageTests
    {
        [Fact]
        public void Invoke_ValidData_AddsMessageToDatabaseWithCorrectValues()
        {
            var messageData = new MessageModel
            {
                Text = "Testmessage",
                UserRoom = new UserRoomModel()
            };

            DataAccess.Models.Message messageSaved = null;
            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.Message>()))
                .Callback<DataAccess.Models.Message>(u => messageSaved = u);

            var action = new AddNewMessage(mockedMessageRepository.Object);

            // action
            action.Invoke(messageData);

            // assert
            mockedMessageRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Message>()), Times.Once());
            mockedMessageRepository.Verify(r => r.Save(), Times.Exactly(1));
            Assert.Equal("Testmessage", messageSaved.Text);
        }

        [Fact]
        public void Invoke_NoUserRoom_AddNorSaveAreCalled()
        {
            var messageData = new MessageModel
            {
                Text = "Testmessage"
            };

            var mockedMessageRepository = new Mock<IMessageRepository>();

            var action = new AddNewMessage(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(messageData);

            // assert
            mockedMessageRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var messageToAdd = new MessageModel();
            var mockedMessageRepository = new Mock<IMessageRepository>();

            var action = new AddNewMessage(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(messageToAdd);

            // assert
            Assert.Equal(Guid.Empty, result);
            mockedMessageRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}