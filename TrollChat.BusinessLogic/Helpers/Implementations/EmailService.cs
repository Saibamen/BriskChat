using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BriskChat.BusinessLogic.Configuration.Implementations;
using BriskChat.BusinessLogic.Helpers.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BriskChat.BusinessLogic.Helpers.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient client;
        private readonly IOptions<EmailServiceCredentials> settings;

        public EmailService(IOptions<EmailServiceCredentials> settings)
        {
            this.settings = settings;
            client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");
        }

        public MimeMessage CreateMessage(string emailAddress, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("BriskChat", settings.Value.Login));
            emailMessage.To.Add(new MailboxAddress("", emailAddress));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };

            emailMessage.Body = builder.ToMessageBody();

            return emailMessage;
        }

        public Task<bool> ConnectClient()
        {
            try
            {
                client.Connect(settings.Value.Host, settings.Value.Port, settings.Value.UseSsl);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(settings.Value.Login, settings.Value.Password);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> DisconnectClient()
        {
            try
            {
                await client.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(MimeMessage emailMessage)
        {
            try
            {
                await client.SendAsync(emailMessage);

                return true;
            }
            // TODO: LOG failures
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                return false;
            }
        }
    }
}