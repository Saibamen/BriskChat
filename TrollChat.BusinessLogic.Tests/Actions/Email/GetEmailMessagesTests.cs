using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TrollChat.BusinessLogic.Actions.Email.Implementations;
using TrollChat.DataAccess.Models;
using TrollChat.DataAccess.Repositories.Interfaces;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Actions.Email
{
    public class GetEmailMessagesTests
    {
        [Fact]
        public void Invoke_ValidData_ReturnsCorrectModel()
        {
            // prepare
            var findByResult = new List<EmailMessage>
            {
                new EmailMessage(),
                new EmailMessage(),
                new EmailMessage()
            };

            var mockedEmailRepository = new Mock<IEmailRepository>();
            mockedEmailRepository.Setup(r => r.FindBy(It.IsAny<Expression<Func<EmailMessage, bool>>>()))
                .Returns(findByResult.AsQueryable);

            var action = new GetEmailMessages(mockedEmailRepository.Object);

            //action
            var list = action.Invoke(2);

            //assert
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
            mockedEmailRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<EmailMessage, bool>>>()), Times.Once);
        }

        [Fact]
        public void Invoke_ValidData_EmptyRepository()
        {
            // prepare
            var mockedEmailRepository = new Mock<IEmailRepository>();
            var action = new GetEmailMessages(mockedEmailRepository.Object);

            //action
            var list = action.Invoke(2);

            //assert
            Assert.Equal(0, list.Count);
            mockedEmailRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<EmailMessage, bool>>>()), Times.Once);
        }
    }
}