using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;

namespace linc.Contracts
{
    public interface IDossierService
    {
        Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10);

        Task<DossierDetailsViewModel> GetDossierDetailsAsync(int id);

        Task<DossierEditViewModel> GetDossierEditAsync(int id);

        Task AssignDossierAsync(int dossierId, string userId);

        Task<int> CreateDossierAsync(DossierCreateViewModel input, string currentUserId);

        Task UpdateDossierAsync(DossierEditViewModel input);
    }
}
