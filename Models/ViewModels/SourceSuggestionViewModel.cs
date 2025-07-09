namespace linc.Models.ViewModels;

public class SourceSuggestionViewModel
{
    public int SourceId { get; set; }

    public int IssueId { get; set; }

    public string Authors { get; set; }

    public string Title { get; set; }

    public string SourceLink { get; set; }

    public string IssueInformation { get; set; }

    public int IssueNumber { get; set; }

    public int IssueYear { get; set; }

    public int StartingPdfPage { get; set; }

    public int StartingIndexPage { get; set; }
}