using linc.Models.ConfigModels;
using linc.Models.ViewModels.Emails;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace linc.Contracts
{
    public interface ISiteEmailSender : IEmailSender
    {
        EmailConfig EmailConfig { get; }

        Task SendEmailAsync<T>(SiteEmailDescriptor<T> siteEmailDescriptor) where T : BaseEmailViewModel =>
            SendEmailsAsync(new[] { siteEmailDescriptor });

        Task SendEmailsAsync<T>(IEnumerable<SiteEmailDescriptor<T>> siteEmailDescriptors) where T : BaseEmailViewModel;
    }
}
