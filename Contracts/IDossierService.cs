using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Models.ViewModels.Source;

namespace linc.Contracts
{
    public interface IDossierService
    {
        Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10);

        Task<int> CreateDossierAsync(DossierCreateViewModel input, string currentUserId);
    }
}
