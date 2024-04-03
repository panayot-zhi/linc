using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using linc.Services;

namespace linc.Controllers
{
    public class IssueController : BaseController
    {
        private readonly ILogger<IssueController> _logger;
        private readonly IIssueService _issueService;
        private readonly ApplicationConfig _config;

        public IssueController(
            IOptions<ApplicationConfig> configOptions,
            ILogger<IssueController> logger,
            ILocalizationService localizer,
            IIssueService issueService)
            : base(localizer)
        {
            _logger = logger;
            _issueService = issueService;
            _config = configOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentLanguageId = LocalizationService.GetCurrentLanguageId();
            var applicationIssue = await _issueService.GetIssueAsync(id.Value, currentLanguageId);
            if (applicationIssue == null)
            {
                _logger.LogWarning("Could not find issue with the id of {@Id}",
                    id.Value);
                return NotFound();
            }

            if (!applicationIssue.IsAvailable && !User.IsAtLeast(SiteRole.UserPlus))
            {
                return NotFound();
            }

            return View(applicationIssue);
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
            return RedirectToAction(nameof(Details), new { id = issueId });
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

        public async Task<IActionResult> LoadIssuePdf(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueEntry = await _issueService.GetIssueAsync(id.Value);
            if (issueEntry == null)
            {
                return NotFound();
            }

            var issuePdf = await _issueService.GetFileAsync(issueEntry.Pdf.Id);
            var pdfPath = Path.Combine(_config.RepositoryPath, issuePdf.RelativePath);
            var content = await System.IO.File.ReadAllBytesAsync(pdfPath);

            return new FileContentResult(content, issuePdf.MimeType);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadFile(int id)
        {
            var file = await _issueService.GetFileAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            var path = Path.Combine(_config.RepositoryPath, file.RelativePath);

            if (!System.IO.File.Exists(path))
            {
                _logger.LogWarning("Could not find physical file {@File}", 
                    file);
                return NotFound();
            }

            return new PhysicalFileResult(path, file.MimeType)
            {
                FileDownloadName = $"{file.FileName}.{file.Extension}"
            };
        }
    }
}
