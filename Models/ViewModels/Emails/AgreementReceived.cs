namespace linc.Models.ViewModels.Emails
{
    public class AgreementReceived : BaseEmailViewModel
    {
        public string Names { get; set; }

        public LinkViewModel AgreementLink { get; set; }
    }
}
