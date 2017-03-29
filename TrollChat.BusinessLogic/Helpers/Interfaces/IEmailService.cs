using System.Threading.Tasks;
using MimeKit;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Helpers.Interfaces
{
    public interface IEmailService : IAction
    {
        Task SendEmailAsync(MimeMessage emailMessage);

        MimeMessage CreateMessage(string emailAddress, string subject, string message);
    }
}
