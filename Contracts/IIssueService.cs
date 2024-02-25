using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface IIssueService
    {
        Task<int> CreateIssueAsync(IssueCreateViewModel input);
    }
}
