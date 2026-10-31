namespace linc.Models.ViewModels.Pdfs
{
    public class AgreementViewModel
    {
        public int AuthorId { get; set; }

        public int DossierId { get; set; }

        public string Layout { get; set; }

        public bool Previewing { get; set; }

        public string Title { get; set; }

        public string AuthorNames { get; set; }

        public string SignerNames { get; set; }

        public string CurrentDate { get; set; }

        public string SiteLink { get; set; }
    }
}
