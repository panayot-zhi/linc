using linc.Data;
using linc.Models.ViewModels.Source;

namespace linc.Contracts
{
    public interface ISourceService
    {
        Task<ApplicationSource> GetSourceAsync(int id);

        Task<SourceIndexViewModel> GetSourcesPagedAsync(string filter, int languageId, int? year, int? issueId,
            int? pageIndex, int pageSize = 10);

        Task<SourceIndexViewModel> GetAdminSourcesPagedAsync(int languageId, int? pageIndex, int pageSize = 10);

        Task<List<SourceCountByYears>> GetSourcesCountByYears();

        Task<List<SourceCountByIssues>> GetSourcesCountByIssues();

        Task<int> CreateSourceAsync(SourceCreateViewModel input);

        Task UpdateSourceAsync(SourceUpdateViewModel input);

        // Task UpdateAuthorAsync(ApplicationUser user);

        Task DeleteSourceAsync(int id);
    }
}
