using linc.Models.ViewModels.Author;

namespace linc.Contracts
{
    public interface IAuthorService
    {
        Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(string searchTerm);
    }
}
