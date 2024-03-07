using linc.Data;
using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface IIssueService
    {
        Task<ApplicationDocument> GetFileAsync(int id);

        Task<ApplicationIssue> GetIssueAsync(int id);

        Task<int> CreateIssueAsync(IssueCreateViewModel input);
    }
}
