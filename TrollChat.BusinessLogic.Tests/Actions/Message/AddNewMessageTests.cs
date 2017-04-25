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
            };

            DataAccess.Models.Message messageSaved = null;
            var mockedMessagerepository = new Mock<IMessageRepository>();
            mockedMessagerepository.Setup(r => r.Add(It.IsAny<DataAccess.Models.Message>()))
            .Callback<DataAccess.Models.Message>(u => messageSaved = u);

            var action = new AddNewMessage(mockedMessagerepository.Object);

            // action
            action.Invoke(messageData);

            // assert
            Assert.Equal("Testmessage", messageSaved.Text);
            mockedMessagerepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Message>()), Times.Once());
            mockedMessagerepository.Verify(r => r.Save(), Times.Exactly(1));
        }

        [Fact]
        public void Invoke_InvalidData_AddNorSaveAreCalled()
        {
            // prepare
            var MessageToAdd = new MessageModel();
            var mockedMessageRepository = new Mock<IMessageRepository>();

            var action = new AddNewMessage(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(MessageToAdd);

            // assert
            Assert.False(result);
            mockedMessageRepository.Verify(r => r.Add(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}