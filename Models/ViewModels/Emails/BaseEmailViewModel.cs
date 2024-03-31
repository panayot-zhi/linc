using System.Globalization;
using Newtonsoft.Json;

namespace linc.Models.ViewModels.Emails
{
    public abstract class BaseEmailViewModel
    {
        [JsonIgnore]
        public bool ModelPopulated { get; set; }

        public string Language { get; set; } = CultureInfo.CurrentUICulture.Name;

        public string Preview { get; set; }

        public EmailLink Logo { get; set; } = new();

        public List<EmailLink> FooterLinks { get; set; } = new();
        
        public string FooterText { get; set; }
        
        public bool Unsubscribe { get; set; }

        public bool IsPreviewing { get; set; }
    }
}
