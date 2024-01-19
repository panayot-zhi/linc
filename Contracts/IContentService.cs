using linc.Models.ViewModels;

namespace linc.Contracts;

public interface IContentService
{
    List<SuggestionsViewModel> GetSuggestions(int count = 5);
}