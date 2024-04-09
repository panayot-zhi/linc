using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using linc.Services;

namespace linc.Controllers
{
    public class IssueController : BaseController
    {
        private readonly ILogger<IssueController> _logger;
        private readonly IIssueService _issueService;

        public IssueController(
            ILocalizationService localizationService,
            ILogger<IssueController> logger,
            IIssueService issueService)
            : base(localizationService)
        {
            _logger = logger;
            _issueService = issueService;
        }

        // TODO
        // public async Task<IActionResult> Index(int? page, int? year, string filter, int? issueId)
        // {
        //     filter = System.Net.WebUtility.UrlDecode(filter);
        //     var languageId = LocalizationService.GetCurrentLanguageId();
        //     var viewModel = await _sourceService.GetSourcesPagedAsync(filter: filter, languageId: languageId,
        //         year: year, issueId: issueId, pageIndex: page);
        //
        //     viewModel.YearFilter = await _sourceService.GetSourcesCountByYears();
        //     viewModel.IssuesFilter = await _sourceService.GetSourcesCountByIssues();
        //
        //     viewModel.AuthorsFilter = filter;
        //     viewModel.CurrentIssueId = issueId;
        //     viewModel.CurrentAuthorsFilter = filter;
        //     viewModel.CurrentYearFilter = year;
        //
        //     return View(viewModel);
        // }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentLanguageId = LocalizationService.GetCurrentLanguageId();
            var issue = await _issueService.GetIssueAsync(id.Value, currentLanguageId);
            if (issue == null)
            {
                _logger.LogWarning("Could not find issue with the id of {@Id}", id.Value);
                return NotFound();
            }

            if (!issue.IsAvailable && !User.IsAtLeast(SiteRole.UserPlus))
            {
                return NotFound();
            }

            _logger.LogInformation("Loading issue {Id}", id.Value);

            return View(issue);
        }

        [SiteAuthorize(SiteRole.Editor)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Create(IssueCreateViewModel vModel) 
        {
            if (!ModelState.IsValid)
            {
                return View(vModel);
            }

            var issueId = await _issueService.CreateIssueAsync(vModel);

            _logger.LogInformation("Issue {IssueId} has been created successfully, redirecting...",
                issueId);

            return RedirectToAction(nameof(Details), new { id = issueId });
        }
    }
}
