using System.Globalization;

namespace linc.Models.ViewModels.Pdfs
{
    public abstract class BasePdfViewModel
    {
        public bool IsPreviewing { get; set; }

        public string Language { get; set; } = CultureInfo.CurrentUICulture.Name;

        public LinkViewModel Logo { get; set; } = new();
    }
}
