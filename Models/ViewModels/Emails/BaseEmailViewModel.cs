using System.Globalization;

namespace linc.Models.ViewModels.Emails
{
    public abstract class BaseEmailViewModel
    {
        public string Language { get; set; } = CultureInfo.CurrentUICulture.Name;

        public bool ModelPopulated { get; set; }

        public string Preview { get; set; }

        public EmailLink Logo { get; set; } = new();

        public List<EmailLink> FooterLinks { get; set; } = new();

        public bool Unsubscribe { get; set; }

        public bool IsPreviewing { get; set; }
    }
}
