using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;

namespace linc
{
    [SiteAuthorize(SiteRole.Editor)]
    public class DossierController : BaseController
    {
        private readonly IDossierService _dossierService;

        public DossierController(
            IDossierService dossierService,
            ILocalizationService localizationService) 
            : base(localizationService)
        {
            _dossierService = dossierService;
        }

        public async Task<IActionResult> Index(int? page, string sort, string order)
        {
            Enum.TryParse(order, ignoreCase: true, result: out SiteSortOrder sortOrder);
            var viewModel = await _dossierService.GetDossiersPagedAsync(page, sort, sortOrder);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var viewModel = await _dossierService.GetDossierDetailsAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }
            
            return View(viewModel);
        }

        [SiteAuthorize(SiteRole.HeadEditor)]
        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> AssignToMe(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var userId = User.GetUserId();
            await _dossierService.AssignDossierAsync(id.Value, userId);

            return RedirectToAction(nameof(Details));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _dossierService.GetDossierEditAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DossierEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _dossierService.UpdateDossierAsync(viewModel);

            return RedirectToAction(nameof(Details));
        }
    }
}
