using linc.Data;
using linc.Contracts;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Home;
using Microsoft.Extensions.Caching.Memory;

namespace linc.Services;

public class ContentService : IContentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMemoryCache _cache; 

    public ContentService(ApplicationDbContext dbContext, LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _dbContext = dbContext;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public IndexViewModel GetIndexViewModel()
    {
        // TODO: Expand and fill this model with information
        var viewModel = new IndexViewModel()
        {
            CountsViewModel = new(),
            ReviewsViewModel = new()
        };

        return viewModel;
    }

    public List<SuggestionsViewModel> GetSuggestions(int count = 5)
    {
        var result = new List<SuggestionsViewModel>();

        // for (var i = 1; i <= 5; i++)
        // {
        //     result.Add(new()
        //     {
        //         Content = "Брой №" + i,
        //         Href = "#"
        //     });
        // }
        //
        // result.Shuffle();

        return result;
    }

    public string GetVersion()
    {
        var result = "0.0.1";

        var assembly = typeof(Program).Assembly.GetName();
        if (assembly.Version is not null)
        {
            result = assembly.Version.ToString();
        }

        var assemblyFileName = Environment.GetCommandLineArgs().FirstOrDefault();
        if (!string.IsNullOrEmpty(assemblyFileName))
        {
            var buildDate = File.GetLastWriteTime(assemblyFileName);
            result += $" - {buildDate:MMMM dd, yyyy}";
        }

        return result;
    }
}