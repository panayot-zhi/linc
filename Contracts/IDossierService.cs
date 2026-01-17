using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Data;

namespace linc.Contracts
{
    public interface IDossierService
    {
        Task<ApplicationDocument> GetDossierDocumentAsync(int id, int documentId);

        Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10);

        Task<DossierDetailsViewModel> GetDossierDetailsViewModelAsync(int id);

        Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id);

        Task<ApplicationDossier> GetDossierAsync(int id);

        Task<int> CreateDossierAsync(DossierCreateViewModel input);

        Task UpdateAssigneeAsync(int id, string targetUserId);

        Task UpdateStatusAsync(int id, ApplicationDossierStatus status);

        Task UpdateDossierAsync(DossierEditViewModel input);
        
        Task UpdateAuthorAsync(ApplicationUser user);
        
        Task UpdateReviewerAsync(ApplicationUser user);

        Task SaveAgreementAsync(ApplicationDossier dossier, byte[] stampedPdfFile);

        Task DeleteAgreementAsync(ApplicationDossier dossier);

    }
}
