using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using linc.Data;
using linc.Models.Enumerations;
using linc.Utility;

namespace linc.Controllers.Administration
{
    public class ApplicationSourceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationSourceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sources.Include(a => a.Author).Include(a => a.Issue).Include(a => a.Language).Include(a => a.Pdf);
            return View(await applicationDbContext.ToListAsync());
        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sources == null)
            {
                return NotFound();
            }

            var applicationSource = await _context.Sources
                .Include(a => a.Author)
                .Include(a => a.Issue)
                .Include(a => a.Language)
                .Include(a => a.Pdf)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (applicationSource == null)
            {
                return NotFound();
            }

            return View(applicationSource);
        }

        [SiteAuthorize(SiteRole.HeadEditor)]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id");
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Id");
            ViewData["PdfId"] = new SelectList(_context.Documents, "Id", "Extension");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.HeadEditor)]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,AuthorNames,AuthorNotes,DOI,IsSection,IsTheme,Title,TitleNotes,StartingPage,LastPage,LanguageId,IssueId,PdfId,AuthorId,LastUpdated,DateCreated")] ApplicationSource applicationSource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationSource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", applicationSource.AuthorId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", applicationSource.IssueId);
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Id", applicationSource.LanguageId);
            ViewData["PdfId"] = new SelectList(_context.Documents, "Id", "Extension", applicationSource.PdfId);
            return View(applicationSource);
        }

        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sources == null)
            {
                return NotFound();
            }

            var applicationSource = await _context.Sources.FindAsync(id);
            if (applicationSource == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", applicationSource.AuthorId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", applicationSource.IssueId);
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Id", applicationSource.LanguageId);
            ViewData["PdfId"] = new SelectList(_context.Documents, "Id", "Extension", applicationSource.PdfId);
            return View(applicationSource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,AuthorNames,AuthorNotes,DOI,IsSection,IsTheme,Title,TitleNotes,StartingPage,LastPage,LanguageId,IssueId,PdfId,AuthorId,LastUpdated,DateCreated")] ApplicationSource applicationSource)
        {
            if (id != applicationSource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationSource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationSourceExists(applicationSource.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", applicationSource.AuthorId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", applicationSource.IssueId);
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Id", applicationSource.LanguageId);
            ViewData["PdfId"] = new SelectList(_context.Documents, "Id", "Extension", applicationSource.PdfId);
            return View(applicationSource);
        }

        [SiteAuthorize(SiteRole.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sources == null)
            {
                return NotFound();
            }

            var applicationSource = await _context.Sources
                .Include(a => a.Author)
                .Include(a => a.Issue)
                .Include(a => a.Language)
                .Include(a => a.Pdf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationSource == null)
            {
                return NotFound();
            }

            return View(applicationSource);
        }

        [SiteAuthorize(SiteRole.Administrator)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sources == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sources'  is null.");
            }
            var applicationSource = await _context.Sources.FindAsync(id);
            if (applicationSource != null)
            {
                _context.Sources.Remove(applicationSource);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationSourceExists(int id)
        {
          return _context.Sources.Any(e => e.Id == id);
        }
    }
}
