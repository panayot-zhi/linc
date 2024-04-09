namespace linc.Models.ViewModels;

public class SuggestionsViewModel
{
    public int SourceId { get; set; }

    public int IssueId { get; set; }

    public int StartingPage { get; set; }

    public string Content { get; set; }

    public string Href { get; set; }
}