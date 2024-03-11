using linc.Data;
using linc.Models.ViewModels.Source;

namespace linc.Contracts
{
    public interface ISourceService
    {
        Task<ApplicationSource> GetSourceAsync(int id);

        Task<SourceIndexViewModel> GetSourcesPagedAsync(string filter, int? year, int? issueId,
            int? pageIndex, int pageSize = 10);

        Task<List<SourceCountByYears>> GetSourcesCountByYears();

        Task<List<SourceCountByIssues>> GetSourcesCountByIssues();

        Task<int> CreateSourceAsync(SourceCreateViewModel input);
    }
}
