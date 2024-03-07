namespace linc.Models.ViewModels.Home
{
    public class PortfolioViewModel
    {
        public List<int> IssueYears { get; set; }

        public List<IssueViewModel> Issues { get; set; }
    }

    public class IssueViewModel
    {
        public int Id { get; set; }

        public int IssueNumber { get; set; }

        public int ReleaseYear { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Description { get; set; }

        public int CoverPageId { get; set; }

        public string CoverPageRelativePath { get; set; }

        // public List<int> IndexPageIds { get; set; }
    }
}
