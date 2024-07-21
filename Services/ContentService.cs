using linc.Data;
using linc.Contracts;
using linc.Models.Enumerations;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Home;
using linc.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace linc.Services;

public class ContentService : IContentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMemoryCache _cache; 

    public ContentService(ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IConfiguration configuration,
        IMemoryCache cache)
    {
        _cache = cache;
        _dbContext = dbContext;
        _configuration = configuration;
        _userManager = userManager;
        _localizationService = localizationService;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public async Task<IndexViewModel> GetIndexViewModel()
    {
        // TODO: Expand and fill this model with information
        // TODO: Cache this
        
        var countsViewModel = new CountsViewModel()
        {
            AdministratorsCount = _dbContext.CountByRole(SiteRolesHelper.AdministratorRoleName),
            HeadEditorsCount = _dbContext.CountByRole(SiteRolesHelper.HeadEditorRoleName),
            EditorsCount = _dbContext.CountByRole(SiteRolesHelper.EditorRoleName),
            UsersCount = _dbContext.CountByRole(SiteRolesHelper.UserRoleName),

            // NOTE: a hard-coded value
            EditorsBoardCount = 9
        };

        var viewModel = new IndexViewModel()
        {
            CountsViewModel = countsViewModel,
        };

        if (!_dbContext.Issues.Any())
        {
            return viewModel;
        }

        var issuesList = await _dbContext.Issues
            .Include(x => x.Files)
            .OrderByDescending(x => x.LastUpdated)
            .Where(x => x.IsAvailable)
            .Select(x => new IssueViewModel()
            {
                Id = x.Id,
                Description = x.Description,
                IssueNumber = x.IssueNumber,
                ReleaseYear = x.ReleaseYear,
                ReleaseDate = x.ReleaseDate,
                CoverPageId = x.CoverPage.Id,
                CoverPageRelativePath = x.CoverPage.RelativePath,
                // IndexPageIds = x.IndexPages
                //     .Select(y => y.Id)
                //     .ToList()

            })
            .Take(6)
            .ToListAsync();

        var issuesViewModel = new PortfolioViewModel()
        {
            Issues = issuesList,
            IssueYears = issuesList
                .GroupBy(x => x.ReleaseYear)
                .Select(g => g.Key)
                .ToList()
        };

        viewModel.PortfolioViewModel = issuesViewModel;

        return viewModel;
    }

    public List<SourceSuggestionViewModel> GetSourceSuggestions(int count = 3)
    {
        if (!_dbContext.Issues.Any())
        {
            return new List<SourceSuggestionViewModel>();
        }

        var lastIssue = _dbContext.Issues.Where(x => x.IsAvailable).Max(x => x.Id);
        var sources = _dbContext.Sources
            .Include(x => x.Issue)
            .Where(x => x.IssueId == lastIssue)
            .Where(x => !x.IsSection)
            .Select(x => new SourceSuggestionViewModel()
            {
                SourceId = x.Id,
                IssueId = x.IssueId,
                AuthorNames = x.AuthorNames,
                Title = x.Title,
                StartingPdfPage = x.StartingPdfPage,
                StartingIndexPage = x.StartingIndexPage,
                LastIndexPage = x.LastIndexPage,
                IssueNumber = x.Issue.IssueNumber,
                IssueYear = x.Issue.ReleaseYear

            })
            .ToList();
        
        sources.Shuffle();

        var result = sources.Take(count).ToList();

        result.ForEach(x =>
        {
            var sourceIssueLink = _linkGenerator.GetUriByAction(
                _httpContextAccessor.HttpContext!,
                "LoadIssueDocument",
                "Document",
                new { issueId = x.IssueId },
                fragment: new FragmentString($"#page={x.StartingPdfPage}"));

            var issueDetailsLink = _linkGenerator.GetUriByAction(
                _httpContextAccessor.HttpContext!,
                "Details",
                "Issue",
                new { id = x.IssueId });

            x.IssueInformation = _localizationService["SourceSuggestion_IssueInformation_Template",
                issueDetailsLink, x.IssueNumber, x.IssueYear, sourceIssueLink, $"{x.StartingIndexPage}-{x.LastIndexPage}"].Value;

            x.SourceLink =
                // NOTE: link to the source pdf itself
                _linkGenerator.GetUriByAction(
                    _httpContextAccessor.HttpContext!,
                    "LoadSourceDocument",
                    "Document",
                    new { sourceId = x.SourceId });
        });

        return result;  
    }

    // cache version for as long
    // as the application is running
    private static string _version;

    public static string GetVersion()
    {
        if (_version != null)
        {
            return _version;
        }

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

        _version = result;

        return result;
    }
}