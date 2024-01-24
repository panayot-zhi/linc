using linc.Models.ViewModels;
using linc.Models.ViewModels.Home;

namespace linc.Contracts;

public interface IContentService
{
    List<SuggestionsViewModel> GetSuggestions(int count = 5);

    IndexViewModel GetIndexViewModel();
}