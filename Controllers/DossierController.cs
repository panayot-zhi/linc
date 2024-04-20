using iTextSharp.text;
using iTextSharp.text.pdf;
using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;
using iTextSharp.text.html.simpleparser;
using linc.Models.ConfigModels;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Pdfs;

namespace linc.Controllers
{
    public class DossierController : BaseController
    {
        private readonly ApplicationConfig _config;
        private readonly IDossierService _dossierService;
        private readonly IRazorViewToStringRenderer _viewRenderer;

        public DossierController(
            IDossierService dossierService,
            ILocalizationService localizationService, 
            IRazorViewToStringRenderer viewRenderer)
            : base(localizationService)
        {
            _dossierService = dossierService;
            _viewRenderer = viewRenderer;
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

        [HttpGet("dossier/{id:int}/agreement")]
        [SiteAuthorize]
        public async Task<IActionResult> Agreement(int id)
        {
            var viewModel = new AgreementViewModel()
            {
                Logo = new LinkViewModel()
                {
                    Text = SiteConstant.SiteName,
                    Url = _config.ServerUrl

                }
            };

            return View("Pdfs/Agreement", viewModel);
        }

        [HttpPost("dossier/{id:int}/agreement")]
        [SiteAuthorize]
        public async Task<IActionResult> SaveAgreement(int? id)
        {
            var viewName = "Pdfs/Agreement.cshtml";
            var viewModel = new AgreementViewModel()
            {

            };

            var htmlView = await _viewRenderer.RenderViewToStringAsync(viewName, viewModel);

            await using var fileStream = new FileStream($"F:\\temp\\linc\\dossiers\\test\\{Guid.NewGuid()}.pdf", FileMode.Create);
            using var reader = new StringReader(htmlView);

            // step 1: creation of a document-object
            var document = new Document(PageSize.A4, 30, 30, 30, 30);

            // step 2:
            // we create a writer that listens to the document
            // and directs a XML-stream to a file
            var writer = PdfWriter.GetInstance(document, fileStream);

            // step 3: we create a worker parse the document
            var worker = new HtmlWorker(document);

            // step 4: we open document and start the worker on the document
            document.Open();

            worker.StartDocument();

            // step 5: parse the html into the document
            worker.Parse(reader);

            // step 6: close the document and the worker
            worker.EndDocument();
            worker.Close();
            document.Close();
            writer.Close();

            return Redirect("/");
        }
    }
}
