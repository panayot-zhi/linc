using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Data;

namespace linc.Contracts
{
    public partial interface IDossierService
    {
        Task<ApplicationDocument> GetDossierDocumentAsync(int id, int documentId);

        Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10);

        Task<DossierDetailsViewModel> GetDossierDetailsViewModelAsync(int id);

        Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id);

        Task<ApplicationDossier> GetDossierAsync(int id);
    }
}
