using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BriskChat.BusinessLogic.Actions.Email.Implementations;
using BriskChat.DataAccess.Models;
using BriskChat.DataAccess.Repositories.Interfaces;
using Moq;
using Xunit;

namespace BriskChat.BusinessLogic.Tests.Actions.Email
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

            // action
            var list = action.Invoke(2);

            // assert
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

            // action
            var list = action.Invoke(2);

            // assert
            Assert.Empty(list);
            mockedEmailRepository.Verify(r => r.FindBy(It.IsAny<Expression<Func<EmailMessage, bool>>>()), Times.Once);
        }
    }
}