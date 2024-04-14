using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using System.ComponentModel.DataAnnotations;

namespace linc.Contracts
{
    public interface IDossierService
    {
        Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10);

        Task<DossierDetailsViewModel> GetDossierDetailsViewModelAsync(int id);

        Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id);

        Task<int> CreateDossierAsync(DossierCreateViewModel input);

        Task UpdateAssigneeAsync(int id, string targetUserId);

        Task UpdateStatusAsync(int id, ApplicationDossierStatus status);

        Task UpdateDossierAsync(DossierEditViewModel input);
    }
}
