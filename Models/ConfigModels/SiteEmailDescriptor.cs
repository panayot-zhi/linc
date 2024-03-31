using System.Globalization;
using linc.Models.ViewModels.Emails;

namespace linc.Models.ConfigModels
{
    public class SiteEmailDescriptor<T> where T : BaseEmailViewModel
    {
        public List<string> Emails { get; set; } = new();

        public List<string> CcEmails { get; set; } = new();

        public List<string> BccEmails { get; set; } = new();

        public string Subject { get; set; }

        public string Template { get; set; } = typeof(T).Name;

        public T ViewModel { get; set; }
    }
}
