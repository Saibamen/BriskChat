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
        private readonly SmtpClient _client;
        private readonly IOptions<EmailServiceCredentials> _settings;

        public EmailService(IOptions<EmailServiceCredentials> settings)
        {
            _settings = settings;
            _client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            _client.AuthenticationMechanisms.Remove("XOAUTH2");
        }

        public MimeMessage CreateMessage(string emailAddress, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("BriskChat", _settings.Value.Login));
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
                _client.Connect(_settings.Value.Host, _settings.Value.Port, _settings.Value.UseSsl);
                // Note: only needed if the SMTP server requires authentication
                _client.Authenticate(_settings.Value.Login, _settings.Value.Password);

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
                await _client.DisconnectAsync(true);

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
                await _client.SendAsync(emailMessage);

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