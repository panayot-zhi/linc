using linc.Models.ViewModels;
using linc.Models.ViewModels.Home;

namespace linc.Contracts;

public interface IContentService
{
    List<SourceSuggestionViewModel> GetSourceSuggestions(int count = 3);

    Task<IndexViewModel> GetIndexViewModel();

    string GetVersion();
}