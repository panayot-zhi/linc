using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using linc.Models.ViewModels.Source;
using linc.Data;

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

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Admin(int? page, int? year, string filter, int? issueId)
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

        [SiteAuthorize(SiteRole.HeadEditor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _sourceService.GetSourceAsync(id.Value);
            if (source == null)
            {
                return NotFound();
            }

            var vModel = new SourceUpdateViewModel()
            {
                Id = source.Id,
                Title = source.Title,
                TitleNotes = source.TitleNotes,

                DOI = source.DOI,

                FirstName = source.FirstName,
                LastName = source.LastName,
                AuthorNotes = source.AuthorNotes,

                StartingPage = source.StartingPage,
                LastPage = source.LastPage,


                IssueId = source.IssueId,
                LanguageId = source.LanguageId,

                IsTheme = source.IsTheme,
                IsSection = source.IsSection,

                Issues = await GetIssuesAsync(),
                Languages = GetLanguages(),
            };

            return View(vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> Edit(SourceUpdateViewModel vModel)
        {
            if (!ModelState.IsValid)
            {
                vModel.Issues = await GetIssuesAsync();
                vModel.Languages = GetLanguages();

                return View(vModel);
            }

            await _sourceService.UpdateSourceAsync(vModel);
            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _sourceService.DeleteSourceAsync(id.Value);
            return RedirectToAction(nameof(Admin));
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
