using linc.Data;
using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface ISourceService
    {
        Task<ApplicationSource> GetSourceAsync(int id);

        Task<List<ApplicationIssue>> GetIssuesAsync();

        Task<int> CreateSourceAsync(SourceCreateViewModel input);
    }
}
