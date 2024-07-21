using linc.Data;
using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface IIssueService
    {
        Task<ApplicationIssue> GetIssueAsync(int id, int? sourcesLanguageId = null);

        Task<ApplicationDocument> GetIssueDocumentAsync(int id, int? documentId);

        Task<List<ApplicationIssue>> GetIssuesAsync();

        Task<int> CreateIssueAsync(IssueCreateViewModel input);

        static string DisplayIssueLabelInformation(int issueNumber, int releaseYear)
        {
            return $"{issueNumber} / {releaseYear}";
        }
    }
}
