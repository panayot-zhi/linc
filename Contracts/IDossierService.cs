using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Data;

namespace linc.Contracts
{
    public partial interface IDossierService
    {
        Task<int> CreateDossierAsync(DossierCreateViewModel input);

        Task UpdateAssigneeAsync(int id, string targetUserId);

        Task UpdateStatusAsync(int id, ApplicationDossierStatus status);

        Task UpdateDossierAsync(DossierEditViewModel input);
        
        Task SaveAgreementAsync(ApplicationDossier dossier, ApplicationAuthor author, byte[] stampedPdfFile);

        Task DeleteAgreementAsync(ApplicationDossier dossier);

    }
}
