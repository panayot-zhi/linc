namespace linc.Models.ViewModels.Emails
{
    public class ConfirmEmail : BaseEmailViewModel
    {
        public string Names { get; set; }

        public EmailButton Confirm { get; set; } = new();
    }
}
