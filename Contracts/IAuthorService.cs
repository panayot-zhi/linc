using linc.Data;
using linc.Models.ViewModels.Author;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace linc.Contracts
{
    public interface IAuthorService
    {
        Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(int languageId, string searchTerm);

        Task<List<ApplicationAuthor>> CreateAuthorsAsync(int languageId, List<SourceAuthorViewModel> authors);

        Task<List<ApplicationAuthor>> CreateDossierAuthorsAsync(int languageId, List<DossierAuthorViewModel> authors, int dossierId);

        Task UpdateAuthorsAsync(ApplicationSource source, List<SourceAuthorViewModel> newAuthors);

        Task UpdateDossierAuthorsAsync(ApplicationDossier dossier, List<DossierAuthorViewModel> newAuthors);

        Task UpdateAuthorUserAsync(ApplicationUser user);
    }
}
