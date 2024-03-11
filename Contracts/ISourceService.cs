using linc.Data;
using linc.Models.ViewModels.Source;

namespace linc.Contracts
{
    public interface ISourceService
    {
        Task<ApplicationSource> GetSourceAsync(int id);

        Task<List<ApplicationIssue>> GetIssuesAsync();

        Task<int> CreateSourceAsync(SourceCreateViewModel input);
    }
}
