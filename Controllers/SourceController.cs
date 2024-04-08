using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using linc.Models.ViewModels.Source;
using linc.Services;

namespace linc.Controllers
{
    public class SourceController : BaseController
    {
        private readonly ILogger<SourceController> _logger;
        private readonly IDocumentService _documentService;
        private readonly ISourceService _sourceService;
        private readonly IIssueService _issuesService;
        private readonly ApplicationConfig _config;

        public SourceController(
            ILocalizationService localizationService,
            IOptions<ApplicationConfig> configOptions,
            ILogger<SourceController> logger,
            IDocumentService documentService,
            ISourceService sourceService, 
            IIssueService issuesService)
            : base(localizationService)
        {
            _logger = logger;
            _sourceService = sourceService;
            _issuesService = issuesService;
            _documentService = documentService;
            _config = configOptions.Value;
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

            return RedirectToAction("Details", "Issue", new { id = vModel.IssueId });
        }

        /*// GET: Issue/Edit/5
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Issues == null)
            {
                return NotFound();
            }

            var applicationIssue = await _context.Issues.FindAsync(id);
            if (applicationIssue == null)
            {
                return NotFound();
            }
            return View(applicationIssue);
        }

        // POST: Issue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IssueNumber,ReleaseYear,Description,LastUpdated,DateCreated")] ApplicationIssue applicationIssue)
        {
            if (id != applicationIssue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationIssue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationIssueExists(applicationIssue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicationIssue);
        }*/

        /*[SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Issues == null)
            {
                return NotFound();
            }

            var applicationIssue = await _context.Issues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationIssue == null)
            {
                return NotFound();
            }

            return View(applicationIssue);
        }

        [SiteAuthorize(SiteRole.Editor)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Issues == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Issues'  is null.");
            }
            var applicationIssue = await _context.Issues.FindAsync(id);
            if (applicationIssue != null)
            {
                _context.Issues.Remove(applicationIssue);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        public async Task<IActionResult> LoadSourcePdf(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sourceEntry = await _sourceService.GetSourceAsync(id.Value);
            if (sourceEntry == null)
            {
                return NotFound();
            }

            if (!sourceEntry.PdfId.HasValue)
            {
                return NotFound();
            }

            var sourcePdf = await _documentService.GetFileAsync(sourceEntry.PdfId.Value);
            var pdfPath = Path.Combine(_config.RepositoryPath, sourcePdf.RelativePath);
            var content = await System.IO.File.ReadAllBytesAsync(pdfPath);

            return new FileContentResult(content, sourcePdf.MimeType);
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
