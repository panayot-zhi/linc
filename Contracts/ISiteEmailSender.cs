using linc.Models.ConfigModels;
using linc.Models.ViewModels.Emails;

namespace linc.Contracts
{
    public interface ISiteEmailSender
    {
        EmailConfig EmailConfig { get; }

        Task SendEmailAsync<T>(SiteEmailDescriptor<T> siteEmailDescriptor) where T : BaseEmailViewModel =>
            SendEmailsAsync(new[] { siteEmailDescriptor });

        Task SendEmailsAsync<T>(IEnumerable<SiteEmailDescriptor<T>> siteEmailDescriptors) where T : BaseEmailViewModel;
    }
}
