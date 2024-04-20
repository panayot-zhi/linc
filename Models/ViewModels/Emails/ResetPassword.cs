namespace linc.Models.ViewModels.Emails
{
    public class ResetPassword : BaseEmailViewModel
    {
        public string IpAddress { get; set; }

        public LinkViewModel Reset { get; set; } = new();
    }
}
