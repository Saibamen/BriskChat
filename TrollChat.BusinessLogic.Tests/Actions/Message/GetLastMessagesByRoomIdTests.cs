using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TrollChat.BusinessLogic.Actions.Message.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Message
{
    [Collection("mapper")]
    public class GetLastMessageByRoomIdTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            var guid = Guid.NewGuid();

            var userRoom = new DataAccess.Models.UserRoom
            {
                CreatedOn = DateTime.Today
            };

            var messageFromDb = new List<DataAccess.Models.Message>
            {
                new DataAccess.Models.Message
                {
                    Id = new Guid(),
                    Text = "Testowa wiadomość",
                    CreatedOn = DateTime.Now,
                    UserRoom = userRoom
                },
                new DataAccess.Models.Message
                {
                    Id = new Guid(),
                    Text = "Testowa wiadomość2",
                    CreatedOn = DateTime.Now,
                    UserRoom = userRoom
                }
            };

            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.GetLastRoomMessages(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(messageFromDb.AsQueryable());
            var action = new GetLastMessagesByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(guid, 2);

            // check
            Assert.NotNull(message);
            Assert.Equal("Testowa wiadomość2", message[0].Text);
            Assert.Equal("Testowa wiadomość", message[1].Text);
            Assert.Null(message[0].DeletedOn);
            mockedMessageRepository.Verify(r => r.GetLastRoomMessages(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Invoke_InvalidData_EmptyRepository()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetLastMessagesByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetLastRoomMessages(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetLastMessagesByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(new Guid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetLastRoomMessages(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void Invoke_InvalidNumber_ReturnsNull(int number)
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetLastMessagesByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), number);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetLastRoomMessages(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }
    }
}