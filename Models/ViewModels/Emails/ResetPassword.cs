namespace linc.Models.ViewModels.Emails
{
    public class ResetPassword : BaseEmailViewModel
    {
        public string IpAddress { get; set; }

        public EmailButton Reset { get; set; } = new();
    }
}
