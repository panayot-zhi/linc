using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace linc.Controllers
{
    public class IssueController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IIssueService _issueService;

        public IssueController(ApplicationDbContext context,
            ILocalizationService localizer,
            IIssueService issueService)
            : base(localizer)
        {
            _context = context;
            _issueService = issueService;
        }

        public async Task<IActionResult> Index()
        {
              return View(await _context.Issues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
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
            return RedirectToAction(nameof(Edit), new { id = issueId });
        }

        // GET: Issue/Edit/5
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
        }

        [SiteAuthorize(SiteRole.Editor)]
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
        }

        private bool ApplicationIssueExists(int id)
        {
          return _context.Issues.Any(e => e.Id == id);
        }
    }
}
