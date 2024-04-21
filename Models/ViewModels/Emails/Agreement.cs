namespace linc.Models.ViewModels.Emails
{
    public class Agreement : BaseEmailViewModel
    {
        public string Names { get; set; }

        public string DossierStatus { get; set; }

        public LinkViewModel AgreementLink { get; set; }
    }
}
