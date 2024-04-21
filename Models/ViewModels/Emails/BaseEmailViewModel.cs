using System.Globalization;
using System.Text.Json.Serialization;

namespace linc.Models.ViewModels.Emails
{
    public abstract class BaseEmailViewModel
    {
        [JsonIgnore]
        public bool ModelPopulated { get; set; }

        public string Language { get; set; } = CultureInfo.CurrentUICulture.Name;

        public string Preview { get; set; }

        public LinkViewModel Logo { get; set; } = new();

        public List<LinkViewModel> FooterLinks { get; set; } = new();
        
        public string FooterText { get; set; }
        
        public bool Unsubscribe { get; set; }

        public bool IsPreviewing { get; set; }
    }
}
