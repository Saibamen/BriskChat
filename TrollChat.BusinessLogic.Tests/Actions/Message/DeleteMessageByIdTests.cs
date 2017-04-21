using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TrollChat.BusinessLogic.Actions.Message.Implementations;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Message
{
    public class DeleteMessageByIdTests
    {
        [Fact]
        public void Invoke_ValidData_DeleteAndSaveAreCalled()
        {
            // prepare
            var MessageFromDb = new DataAccess.Models.Message()
            {
                Id = 1
            };

            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.GetById(1)).Returns(MessageFromDb);

            var action = new DeleteMessageById(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(1);

            // assert
            Assert.True(result);
            mockedMessageRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Message>()), Times.Once());
            mockedMessageRepository.Verify(r => r.Save(), Times.Once());
        }

        [Fact]
        public void Invoke_ValidData_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();

            var action = new DeleteMessageById(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(1);

            // assert
            Assert.False(result);
            mockedMessageRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
        }
    }
}