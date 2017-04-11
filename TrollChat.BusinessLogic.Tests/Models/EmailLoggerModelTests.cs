using TrollChat.BusinessLogic.Models;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Models
{
    public class EmailLoggerModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            var from = "from";
            var recipient = "recipient";
            var subject = "subject";
            var message = "message";
            var failureCount = 5;
            var failError = "failError";
            var failErrorMessage = "failErrorMessage";

            // action
            var action = new EmailLoggerModel
            {
                From = from,
                Recipient = recipient,
                Subject = subject,
                Message = message,
                FailureCount = failureCount,
                FailError = failError,
                FailErrorMessage = failErrorMessage,
            };

            // check
            Assert.Equal("from", action.From);
            Assert.Equal("recipient", action.Recipient);
            Assert.Equal("subject", action.Subject);
            Assert.Equal("message", action.Message);
            Assert.Equal(5, action.FailureCount);
            Assert.Equal("failError", action.FailError);
            Assert.Equal("failErrorMessage", action.FailErrorMessage);
        }
    }
}