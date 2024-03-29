﻿using linc.Data;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly LinkGenerator _linkGenerator;
    private readonly IMemoryCache _cache; 

    public ContentService(ApplicationDbContext dbContext, 
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor, 
        IMemoryCache cache, 
        IConfiguration configuration, 
        UserManager<ApplicationUser> userManager)
    {
        _cache = cache;
        _dbContext = dbContext;
        _configuration = configuration;
        _userManager = userManager;
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
                ReleaseDate = x.DateCreated,
                CoverPageId = x.CoverPage.Id,
                CoverPageRelativePath = x.CoverPage.RelativePath,
                // IndexPageIds = x.IndexPages
                //     .Select(y => y.Id)
                //     .ToList()

            })
            .Take(12)
            .ToListAsync();

        var issuesViewModel = new PortfolioViewModel()
        {
            Issues = issuesList,
            IssueYears = issuesList
                .GroupBy(x => x.ReleaseYear)
                .Select(g => g.Key)
                .ToList()
        };

        var viewModel = new IndexViewModel()
        {
            CountsViewModel = countsViewModel,
            PortfolioViewModel = issuesViewModel
        };

        return viewModel;
    }

    public List<SuggestionsViewModel> GetSuggestions(int count = 5)
    {
        // TODO: Improve this query
        var sources = _dbContext.Sources
            .Select(x => new SuggestionsViewModel()
            {
                Content = x.Title,
                StartingPage = x.StartingPage,
                IssueId = x.IssueId.Value
            })
            .ToList();
        
        sources.Shuffle();

        var result = sources.Take(5).ToList();

        result.ForEach(x => x.Href = _linkGenerator.GetUriByAction(
            _httpContextAccessor.HttpContext!,
            "Pdf",
            "Source",
            new { id = x.IssueId },
            fragment: new FragmentString($"#page={x.StartingPage}")));

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