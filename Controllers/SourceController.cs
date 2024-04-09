using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using linc.Models.ViewModels.Source;

namespace linc.Controllers
{
    public class SourceController : BaseController
    {
        private readonly ILogger<SourceController> _logger;
        private readonly ISourceService _sourceService;
        private readonly IIssueService _issuesService;

        public SourceController(
            ILocalizationService localizationService,
            ILogger<SourceController> logger,
            ISourceService sourceService, 
            IIssueService issuesService)
            : base(localizationService)
        {
            _logger = logger;
            _sourceService = sourceService;
            _issuesService = issuesService;
        }

        public async Task<IActionResult> Index(int? page, int? year, string filter, int? issueId)
        {
            filter = System.Net.WebUtility.UrlDecode(filter);
            var languageId = LocalizationService.GetCurrentLanguageId();
            var viewModel = await _sourceService.GetSourcesPagedAsync(filter: filter, languageId: languageId, 
                year: year, issueId: issueId, pageIndex: page);
            
            viewModel.YearFilter = await _sourceService.GetSourcesCountByYears();
            viewModel.IssuesFilter = await _sourceService.GetSourcesCountByIssues();

            viewModel.AuthorsFilter = filter;
            viewModel.CurrentIssueId = issueId;
            viewModel.CurrentAuthorsFilter = filter;
            viewModel.CurrentYearFilter = year;

            return View(viewModel);
        }

        // TODO
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var applicationSource = await _sourceService.GetSourceAsync(id.Value);
        //     if (applicationSource == null)
        //     {
        //         _logger.LogWarning("Could not find source with the id of {@Id}",
        //             id.Value);
        //         return NotFound();
        //     }
        //
        //     return View(applicationSource);
        // }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Create()
        {
            var vModel = new SourceCreateViewModel
            {
                Issues = await GetIssuesAsync(),
                Languages = GetLanguages(),

                LanguageId = LocalizationService.GetCurrentLanguageId()
            };

            return View(vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Create(SourceCreateViewModel vModel) 
        {
            if (!ModelState.IsValid)
            {
                vModel.Issues = await GetIssuesAsync();
                vModel.Languages = GetLanguages();

                vModel.LanguageId = LocalizationService.GetCurrentLanguageId();

                return View(vModel);
            }

            var sourceId = await _sourceService.CreateSourceAsync(vModel);

            _logger.LogInformation("Source {SourceId} has been created successfully, redirecting...", 
                sourceId);

            return RedirectToAction("Details", "Issue", new { id = vModel.IssueId });
        }

        private async Task<List<SelectListItem>> GetIssuesAsync()
        {
            var issues = await _issuesService.GetIssuesAsync();
            return issues.Select(x =>
                    new SelectListItem($"{x.IssueNumber}/{x.ReleaseYear}", x.Id.ToString()))
                .ToList();
        }

        private List<SelectListItem> GetLanguages()
        {
            var currentLanguageId = LocalizationService.GetCurrentLanguageId();
            var list = SiteConstant.SupportedCultures.Select(supportedCulture =>
                    new SelectListItem(supportedCulture.Value, supportedCulture.Key.ToString(), 
                        supportedCulture.Key == currentLanguageId))
                .ToList();

            return list;
        }
    }
}
