using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;

namespace linc
{
    [SiteAuthorize(SiteRole.Editor)]
    public class DossierController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IDossierService _dossierService;

        public DossierController(
            ApplicationDbContext context,
            IDossierService dossierService,
            ILocalizationService localizationService) 
            : base(localizationService)
        {
            _context = context;
            _dossierService = dossierService;
        }

        // GET: Dossier
        public async Task<IActionResult> Index(int? page, string sort, string order)
        {
            Enum.TryParse(order, ignoreCase: true, result: out SiteSortOrder sortOrder);
            var viewModel = await _dossierService.GetDossiersPagedAsync(page, sort, sortOrder);
            return View(viewModel);
        }

        // GET: Dossier/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Dossiers == null)
            {
                return NotFound();
            }

            var applicationDossier = await _context.Dossiers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationDossier == null)
            {
                return NotFound();
            }

            return View(applicationDossier);
        }

        // GET: Dossier/Create
        [SiteAuthorize(SiteRole.HeadEditor)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dossier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.HeadEditor)]
        public async Task<IActionResult> Create(DossierCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var currentUserId = User.GetUserId();
            await _dossierService.CreateDossierAsync(viewModel, currentUserId);
            return RedirectToAction(nameof(Index));

        }

        // GET: Dossier/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Dossiers == null)
            {
                return NotFound();
            }

            var applicationDossier = await _context.Dossiers.FindAsync(id);
            if (applicationDossier == null)
            {
                return NotFound();
            }
            return View(applicationDossier);
        }

        // POST: Dossier/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,FirstName,LastName,Email,Status")] ApplicationDossier applicationDossier)
        {
            if (id != applicationDossier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationDossier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationDossierExists(applicationDossier.Id))
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
            return View(applicationDossier);
        }

        // GET: Dossier/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Dossiers == null)
            {
                return NotFound();
            }

            var applicationDossier = await _context.Dossiers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationDossier == null)
            {
                return NotFound();
            }

            return View(applicationDossier);
        }

        // POST: Dossier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Dossiers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Dossiers'  is null.");
            }
            var applicationDossier = await _context.Dossiers.FindAsync(id);
            if (applicationDossier != null)
            {
                _context.Dossiers.Remove(applicationDossier);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationDossierExists(int id)
        {
          return _context.Dossiers.Any(e => e.Id == id);
        }
    }
}
