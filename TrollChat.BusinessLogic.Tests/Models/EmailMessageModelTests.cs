using TrollChat.BusinessLogic.Models;
using Xunit;

namespace TrollChat.BusinessLogic.Tests.Models
{
    public class EmailMessageModelTests
    {
        [Fact]
        public void ValidData_ModelAreCorrect()
        {
            // prepare
            const string from = "from";
            const string recipient = "recipient";
            const string subject = "subject";
            const string message = "message";
            const int failureCount = 5;
            const string failError = "failError";
            const string failErrorMessage = "failErrorMessage";

            // action
            var action = new EmailMessageModel
            {
                From = from,
                Recipient = recipient,
                Subject = subject,
                Message = message,
                FailureCount = failureCount,
                FailError = failError,
                FailErrorMessage = failErrorMessage
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