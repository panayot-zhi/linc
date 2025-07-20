using linc.Data;
using linc.Models.ViewModels.Author;

namespace linc.Contracts
{
    public interface IAuthorService
    {
        Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(int languageId, string searchTerm);

        Task<List<ApplicationAuthor>> CreateAuthorsAsync(int languageId, List<SourceAuthorViewModel> authors);

        Task UpdateAuthorsAsync(ApplicationSource source, List<SourceAuthorViewModel> newAuthors);

        Task UpdateAuthorUserAsync(ApplicationUser user);
    }
}
