using System;
using System.Collections.Generic;
using System.Linq;
using BriskChat.BusinessLogic.Actions.Message.Implementations;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Message
{
    [Collection("mapper")]
    public class GetMessagesOffsetByRoomIdTests
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
            mockedMessageRepository.Setup(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(messageFromDb.AsQueryable());
            mockedMessageRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(new DataAccess.Models.Message());
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(guid, Guid.NewGuid(), 2);

            // check
            Assert.NotNull(message);
            Assert.Equal("Testowa wiadomość", message[0].Text);
            Assert.Equal("Testowa wiadomość2", message[1].Text);
            Assert.Null(message[0].DeletedOn);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRepository_ReturnNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), Guid.NewGuid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Invoke_NoMessages_ReturnNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);
            mockedMessageRepository.Setup(r => r.GetById(It.IsAny<Guid>())).Returns(new DataAccess.Models.Message());

            // action
            var message = action.Invoke(Guid.NewGuid(), Guid.NewGuid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyRoomId_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(new Guid(), Guid.NewGuid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Invoke_EmptyMessageId_ReturnsNull()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), new Guid(), 2);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(1)]
        public void Invoke_InvalidNumbers_ReturnsNull(int limit)
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();
            var action = new GetMessagesOffsetByRoomId(mockedMessageRepository.Object);

            // action
            var message = action.Invoke(Guid.NewGuid(), new Guid(), limit);

            // check
            Assert.Null(message);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            mockedMessageRepository.Verify(r => r.GetRoomMessagesOffset(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<int>()), Times.Never);
        }
    }
}