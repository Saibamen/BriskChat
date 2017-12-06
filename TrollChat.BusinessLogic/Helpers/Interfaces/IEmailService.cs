using System.Threading.Tasks;
using BriskChat.BusinessLogic.Actions.Base;
using MimeKit;

namespace BriskChat.BusinessLogic.Helpers.Interfaces
{
    public interface IEmailService : IAction
    {
        Task<bool> ConnectClient();

        Task<bool> DisconnectClient();

        Task<bool> SendEmailAsync(MimeMessage emailMessage);

        MimeMessage CreateMessage(string emailAddress, string subject, string message);
    }
}
