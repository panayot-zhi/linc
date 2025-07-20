using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Author;
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

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Admin(int? page)
        {
            var languageId = LocalizationService.GetCurrentLanguageId();
            var viewModel = await _sourceService.GetAdminSourcesPagedAsync(languageId: languageId, pageIndex: page);

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
                vModel.Issues = await GetIssuesAsync(vModel.IssueId);
                vModel.Languages = GetLanguages(vModel.LanguageId);

                vModel.LanguageId = LocalizationService.GetCurrentLanguageId();

                return View(vModel);
            }

            var sourceId = await _sourceService.CreateSourceAsync(vModel);

            _logger.LogInformation("Source {SourceId} has been created successfully, redirecting...", 
                sourceId);

            return RedirectToAction("Edit", new { id = sourceId });
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
                DossierId = source.DossierId,

                AuthorNotes = source.AuthorNotes,

                StartingPdfPage = source.StartingPdfPage,
                LastPdfPage = source.LastPdfPage,

                StartingIndexPage = source.StartingIndexPage,

                IssueId = source.IssueId,
                LanguageId = source.LanguageId,

                IsTheme = source.IsTheme,
                IsSection = source.IsSection,

                Authors = source.Authors.Select(a => new SourceAuthorViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,

                    UserId = a.UserId,
                    UserName = a.User?.UserName

                }).ToList(),

                Issues = await GetIssuesAsync(source.IssueId),
                Languages = GetLanguages(source.LanguageId),

                LastUpdated = source.LastUpdated,
                DateCreated = source.DateCreated,
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
                vModel.Issues = await GetIssuesAsync(vModel.IssueId);
                vModel.Languages = GetLanguages(vModel.LanguageId);

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

        private async Task<List<SelectListItem>> GetIssuesAsync(int? issueId = null)
        {
            var issues = await _issuesService.GetIssuesAsync();
            return issues.Select(x => new SelectListItem(
                    IIssueService.DisplayIssueLabelInformation(x.IssueNumber, x.ReleaseYear), 
                    x.Id.ToString(),
                    x.Id == issueId))
                .ToList();
        }

        private List<SelectListItem> GetLanguages(int? languageId = null)
        {
            var currentLanguageId = LocalizationService.GetCurrentLanguageId();
            var list = SiteConstant.SupportedCultures.Select(supportedCulture =>
                    new SelectListItem(
                        supportedCulture.Value, 
                        supportedCulture.Key.ToString(),
                        languageId.HasValue ? supportedCulture.Key == languageId :
                            supportedCulture.Key == currentLanguageId))
                .ToList();

            return list;
        }
    }
}
