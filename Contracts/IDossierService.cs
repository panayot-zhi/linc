using linc.Models.ViewModels.Dossier;

namespace linc.Contracts
{
    public interface IDossierService
    {
        Task<int> CreateDossierAsync(DossierCreateViewModel input);
    }
}
