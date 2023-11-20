using linc.Models.ConfigModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace linc.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger;

        private readonly EmailConfig _emailConfig;

        private readonly IWebHostEnvironment _env;

        public EmailSender(IOptions<EmailConfig> emailConfig,
            ILogger<EmailSender> logger, 
            IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mimeMessage = new MimeMessage()
            {
                Subject = subject,
            };

            mimeMessage.From.Add(MailboxAddress.Parse(_emailConfig.Sender));
            mimeMessage.To.Add(MailboxAddress.Parse(email));

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            mimeMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                if (_env.IsDevelopment())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                // The third parameter is useSSL (true if the client should make 
                // an SSL-wrapped connection to the server; otherwise, false).
                await client.ConnectAsync(_emailConfig.Host, _emailConfig.Port,
                    SecureSocketOptions.StartTlsWhenAvailable);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(_emailConfig.Sender, _emailConfig.Password);

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(quit: true);
            }
        }
    }
}
