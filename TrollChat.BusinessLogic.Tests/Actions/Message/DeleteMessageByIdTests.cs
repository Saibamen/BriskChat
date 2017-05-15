﻿using Moq;
using System;
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
            var guid = Guid.NewGuid();
            var messageFromDb = new DataAccess.Models.Message
            {
                Id = guid
            };

            var mockedMessageRepository = new Mock<IMessageRepository>();
            mockedMessageRepository.Setup(r => r.GetById(guid)).Returns(messageFromDb);

            var action = new DeleteMessageById(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(guid);

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
            var result = action.Invoke(Guid.NewGuid());

            // assert
            Assert.False(result);
            mockedMessageRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void Invoke_EmptyGuid_DeleteNorSaveAreCalled()
        {
            // prepare
            var mockedMessageRepository = new Mock<IMessageRepository>();

            var action = new DeleteMessageById(mockedMessageRepository.Object);

            // action
            var result = action.Invoke(new Guid());

            // assert
            Assert.False(result);
            mockedMessageRepository.Verify(r => r.Delete(It.IsAny<DataAccess.Models.Message>()), Times.Never);
            mockedMessageRepository.Verify(r => r.Save(), Times.Never);
            mockedMessageRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }
    }
}