using System.Globalization;
using System.Text;
using System.Text.Json;
using linc.Contracts;
using linc.Models.ConfigModels;
using linc.Models.ViewModels.Emails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace linc.Services
{
    public class SiteEmailSender : ISiteEmailSender
    {
        public EmailConfig EmailConfig { get; }

        private readonly ApplicationConfig _config;
        private readonly ILogger<SiteEmailSender> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IRazorViewToStringRenderer _razor;
        private readonly IWebHostEnvironment _env;
        private readonly LinkGenerator _link;

        public SiteEmailSender(
            IOptions<EmailConfig> emailConfig,
            IOptions<ApplicationConfig> configOptions,
            ILogger<SiteEmailSender> logger,
            ILocalizationService localizationService,
            IRazorViewToStringRenderer razor,
            IWebHostEnvironment env, 
            LinkGenerator link)
        {
            _env = env;
            _link = link;
            _razor = razor;
            _logger = logger;
            _config = configOptions.Value;
            _localizationService = localizationService;

            EmailConfig = emailConfig.Value;
        }

        public async Task SendEmailsAsync<T>(IEnumerable<SiteEmailDescriptor<T>> siteEmailDescriptors) where T : BaseEmailViewModel
        {
            var mimeMessages = new List<MimeMessage>();

            foreach (var emailDescriptor in siteEmailDescriptors)
            {
                var mimeMessage = await BuildMimeMessage(emailDescriptor);
                mimeMessages.Add(mimeMessage);
            }

            await SendMimeMessages(mimeMessages);
        }

        private async Task<MimeMessage> BuildMimeMessage<T>(SiteEmailDescriptor<T> siteEmailDescriptor) where T : BaseEmailViewModel
        {
            ArgumentNullException.ThrowIfNull(nameof(siteEmailDescriptor.Emails));
            ArgumentNullException.ThrowIfNull(nameof(siteEmailDescriptor.Subject));

            
            var mimeMessage = new MimeMessage()
            {
                From =
                {
                    MailboxAddress.Parse(EmailConfig.Sender)
                }
            };

            foreach (var email in siteEmailDescriptor.Emails)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(email));
            }

            foreach (var email in siteEmailDescriptor.CcEmails)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(email));
            }

            foreach (var email in siteEmailDescriptor.BccEmails.Union(EmailConfig.BlindCarbonCopies))
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(email));
            }

            var htmlBody = await RenderViewToString(siteEmailDescriptor);

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlBody
            };

            mimeMessage.Subject = siteEmailDescriptor.Subject;
            mimeMessage.Body = builder.ToMessageBody();

            return mimeMessage;
        }

        private async Task<string> RenderViewToString<T>(SiteEmailDescriptor<T> siteEmailDescriptor) where T : BaseEmailViewModel
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;
            var currentCultureName = currentCulture.Name.Clone().ToString();
            var viewModel = siteEmailDescriptor.ViewModel;
            var changedCulture = false;

            if (viewModel.Language != currentUiCulture.Name)
            {
                CultureInfo.CurrentCulture = new CultureInfo(viewModel.Language);
                CultureInfo.CurrentUICulture = new CultureInfo(viewModel.Language);

                changedCulture = true;
            }

            var siteName = _localizationService["Logo_Short"].Value;

            if (!siteEmailDescriptor.ViewModel.ModelPopulated)
            {
                viewModel.Logo.Url = _config.ServerUrl;
                viewModel.Logo.Text = siteName;

                viewModel.FooterLinks.Add(new()
                {
                    Text = _localizationService["Footer_GuidelinesPolicies_TermsOfService"].Value,
                    Url = GetActionLink("Terms", "Home")
                });

                viewModel.FooterLinks.Add(new()
                {
                    Text = _localizationService["Footer_GuidelinesPolicies_PrivacyPolicy"].Value,
                    Url = GetActionLink("Privacy", "Home")
                });

                viewModel.FooterLinks.Add(new()
                {
                    Text = _localizationService["BaseEmailModel_FooterProfileLink"].Value,
                    Url = _config.ServerUrl + "/identity/account/manage"
                });

                viewModel.FooterText = _localizationService["BaseEmailModel_FooterText"].Value;

                var previewLink = GetEmailPreviewLink(siteEmailDescriptor.Template, viewModel);
                viewModel.Preview = _localizationService["BaseEmailModel_PreviewEmailInBrowser", previewLink].Value;

                viewModel.ModelPopulated = true;
            }

            if (!siteEmailDescriptor.Subject.StartsWith(siteName))
            {
                siteEmailDescriptor.Subject = $"{siteName} - {siteEmailDescriptor.Subject}";
            }

            var viewName = $"/Views/Shared/Emails/{siteEmailDescriptor.Template}.{viewModel.Language}.cshtml";
            var htmlView = await _razor.RenderViewToStringAsync(viewName, siteEmailDescriptor.ViewModel);

            if (changedCulture) // restore
            {
                CultureInfo.CurrentCulture = new CultureInfo(currentCultureName);
                CultureInfo.CurrentUICulture = new CultureInfo(currentCultureName);
            }

            return htmlView;
        }

        private async Task SendMimeMessages(IEnumerable<MimeMessage> mimeMessages)
        {
            using (var client = new SmtpClient())
            {
                if (_env.IsDevelopment())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                // The third parameter is useSSL (true if the client should make 
                // an SSL-wrapped connection to the server; otherwise, false).
                await client.ConnectAsync(EmailConfig.Host, EmailConfig.Port,
                    SecureSocketOptions.StartTlsWhenAvailable);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(EmailConfig.Sender, EmailConfig.Password);

                foreach (var mimeMessage in mimeMessages)
                {
                    await client.SendAsync(mimeMessage);
                }

                await client.DisconnectAsync(quit: true);
            }
        }

        private string GetEmailPreviewLink(string templateName, dynamic viewModel)
        {
            var jsonViewModel = JsonSerializer.Serialize(viewModel);
            var plainTextBytes = Encoding.UTF8.GetBytes(jsonViewModel);
            var data = Convert.ToBase64String(plainTextBytes);

            return GetActionLink("Email", "Home", new { id = templateName, data });
        }

        private string GetActionLink(string action, string controller, object values = null)
        {
            var currentUri = new Uri(_config.ServerUrl);
            return _link.GetUriByAction(action, controller, values, currentUri.Scheme, new HostString(currentUri.Host, currentUri.Port),
                options: new LinkOptions() { LowercaseUrls = false });
        }

        [Obsolete("Delete")]
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mimeMessage = new MimeMessage()
            {
                Subject = subject,
            };

            mimeMessage.From.Add(MailboxAddress.Parse(EmailConfig.Sender));
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
                await client.ConnectAsync(EmailConfig.Host, EmailConfig.Port,
                    SecureSocketOptions.StartTlsWhenAvailable);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(EmailConfig.Sender, EmailConfig.Password);

                await client.SendAsync(mimeMessage);

                await client.DisconnectAsync(quit: true);
            }
        }
    }
}
