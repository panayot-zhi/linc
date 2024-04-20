using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;

namespace linc.Controllers
{
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

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Index(int? page, string sort, string order)
        {
            Enum.TryParse(order, ignoreCase: true, result: out SiteSortOrder sortOrder);
            var viewModel = await _dossierService.GetDossiersPagedAsync(page, sort, sortOrder);
            return View(viewModel);
        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var viewModel = await _dossierService.GetDossierDetailsViewModelAsync(id.Value);
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

            await _dossierService.CreateDossierAsync(viewModel);
            return RedirectToAction(nameof(Index));

        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _dossierService.GetDossierEditViewModelAsync(id.Value);
            if (viewModel == null)
            {
                return NotFound();
            }

            if (User.IsAtLeast(SiteRole.HeadEditor))
            {
                return View(viewModel);
            }

            // check permissions
            var currentUserId = User.GetUserId();
            if (viewModel.AssigneeId != currentUserId)
            {
                return Forbid();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> Edit(DossierEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // sadly we have to repopulate the whole model if something fails
                viewModel = await _dossierService.GetDossierEditViewModelAsync(viewModel.Id);
                return View(viewModel);
            }

            await _dossierService.UpdateDossierAsync(viewModel);

            return RedirectToAction(nameof(Details), new { id = viewModel.Id });
        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> UpdateAssignee(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            await _dossierService.UpdateAssigneeAsync(id.Value, User.GetUserId());
            return RedirectToAction(nameof(Details), new { id });
        }

        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> UpdateStatus(int? id, ApplicationDossierStatus status)
        {
            if (id is null)
            {
                return NotFound();
            }

            await _dossierService.UpdateStatusAsync(id.Value, status);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
