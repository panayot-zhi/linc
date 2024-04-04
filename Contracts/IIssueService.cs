using linc.Data;
using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface IIssueService
    {
        Task<ApplicationIssue> GetIssueAsync(int id, int? languageId = null);

        Task<List<ApplicationIssue>> GetIssuesAsync();

        Task<int> CreateIssueAsync(IssueCreateViewModel input);
    }
}
