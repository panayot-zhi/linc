namespace linc.Models.ViewModels.Emails
{
    public class ConfirmEmail : BaseEmailViewModel
    {
        public string Names { get; set; }

        public LinkViewModel Confirm { get; set; } = new();
    }
}
