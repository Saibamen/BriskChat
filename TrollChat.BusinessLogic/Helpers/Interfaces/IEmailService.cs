using System.Threading.Tasks;
using MimeKit;
using TrollChat.BusinessLogic.Actions.Base;

namespace TrollChat.BusinessLogic.Helpers.Interfaces
{
    public interface IEmailService : IAction
    {
        Task<bool> ConnectClient();

        Task<bool> DisconnectClient();

        Task<bool> SendEmailAsync(MimeMessage emailMessage);

        MimeMessage CreateMessage(string emailAddress, string subject, string message);
    }
}
