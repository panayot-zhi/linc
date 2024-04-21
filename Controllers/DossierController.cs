using System.Net.Mime;
using iTextSharp.text.pdf;
using linc.Contracts;
using Microsoft.AspNetCore.Mvc;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;
using linc.Models.ConfigModels;
using linc.Models.ViewModels.Pdfs;
using DinkToPdf.Contracts;
using DinkToPdf;
using linc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using linc.Services;

namespace linc.Controllers
{
    public class DossierController : BaseController
    {
        private readonly ApplicationConfig _config;
        private readonly IDossierService _dossierService;
        private readonly ILogger<DossierController> _logger;
        private readonly IRazorViewToStringRenderer _viewRenderer;
        private readonly IUserStore<ApplicationUser> _userService;
        private readonly IDocumentService _documentService;
        private readonly IConverter _converter;

        private const string AgreementPdfViewName = "/Views/Shared/Pdfs/Agreement.cshtml";

        public DossierController(
            ILogger<DossierController> logger,
            IOptions<ApplicationConfig> configOptions,
            ILocalizationService localizationService,
            IUserStore<ApplicationUser> userService,
            IRazorViewToStringRenderer viewRenderer,
            IDocumentService documentService,
            IDossierService dossierService,
            IConverter converter)
            : base(localizationService)
        {
            _logger = logger;
            _converter = converter;
            _documentService = documentService;
            _config = configOptions.Value;
            _dossierService = dossierService;
            _viewRenderer = viewRenderer;
            _userService = userService;
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
            var dossier = await _dossierService.GetDossierAsync(id);
            if (dossier == null)
            {
                _logger.LogWarning(
                    "An attempt was made to sign agreement document for a dossier that does not exist: {DossierId}",
                    id);

                return null;
            }

            var currentUserId = User.GetUserId();
            var currentUser = await _userService.FindByIdAsync(currentUserId,
                CancellationToken.None);

            ArgumentNullException.ThrowIfNull(currentUser);

            if (dossier.Agreement != null)
            {
                if (dossier.AuthorId != currentUserId)
                {
                    // if another user is attempting to view 
                    // the declaration forbid the interaction
                    return Forbid();
                }

                // if this is the user's agreement, let him view it
                return await GetDocumentFile(dossier.Agreement.Id);
            }

            if (dossier.Names != currentUser.Names)
            {
                _logger.LogWarning("Publication agreement was requested by a user with different names than those in the dossier: {DossierNames} != {CurrentUserNames}",
                    dossier.Names, currentUser.Names);
            }

            var viewModel = new AgreementViewModel
            {
                DossierId = dossier.Id,
                AuthorNames = dossier.Names,
                SignerNames = currentUser.Names,
                Title = dossier.Title,
                CurrentDate = DateTime.Now.ToString("dd.MM.yyyy"),
                SiteLink = _config.ServerUrl,

                Previewing = true,
                Layout = "_Layout"
            };

            return View(AgreementPdfViewName, viewModel);
        }

        [HttpPost("dossier/{id:int}/agreement")]
        [ValidateAntiForgeryToken]
        [SiteAuthorize]
        public async Task<IActionResult> SaveAgreement(int id, [Bind("agreement")] string agreement)
        {
            if (string.IsNullOrEmpty(agreement))
            {
                AddAlertMessage("Please check that you agree with the terms of this agreement!", 
                    type: AlertMessageType.Warning);

                return RedirectToAction(nameof(Agreement), new { id });
            }

            var dossier = await _dossierService.GetDossierAsync(id);
            if (dossier == null)
            {
                _logger.LogWarning(
                    "An attempt was made to sign agreement document for a dossier that does not exist: {DossierId}",
                    id);

                return NotFound();
            }

            var currentUserId = User.GetUserId();
            var user = await _userService.FindByIdAsync(currentUserId,
                CancellationToken.None);

            ArgumentNullException.ThrowIfNull(user);

            var viewModel = new AgreementViewModel
            {
                DossierId = dossier.Id,
                AuthorNames = dossier.Names,
                SignerNames = user.Names,
                Title = dossier.Title,
                CurrentDate = DateTime.Now.ToString("dd.MM.yyyy"),
                SiteLink = _config.ServerUrl,

                Layout = "/Views/Shared/Pdfs/_LayoutPdf.cshtml"
            };

            var htmlContent = await _viewRenderer.RenderViewToStringAsync(AgreementPdfViewName, viewModel);

            var pdfFile = GeneratePdfFile(htmlContent);
            var stampedPdfFile = StampPdf(pdfFile, viewModel.SignerNames);

            //return File(stampedPdfFile, MediaTypeNames.Application.Pdf, fileDownloadName: "test.pdf");

            await _dossierService.SaveAgreementAsync(dossier, stampedPdfFile);

            AddAlertMessage(LocalizationService["PublicationAgreement_SuccessMessage"], 
                type: AlertMessageType.Success);

            return Redirect("/");
        }

        private async Task<PhysicalFileResult> GetDocumentFile(int documentId, bool download = false)
        {
            var document = await _documentService.GetDocumentAsync(documentId);
            if (document is null)
            {
                return null;
            }

            var path = _documentService.GetDocumentFilePath(document);
            if (path is null)
            {
                return null;
            }

            PhysicalFileResult result;

            // don't append FileDownloadName by default since we do not want to force
            // browsers to download the file, but instead attempt to display it

            if (download)
            {
                result = new PhysicalFileResult(path, MediaTypeNames.Application.Octet)
                {
                    FileDownloadName = document.OriginalFileName
                };
            }
            else
            {
                result = new PhysicalFileResult(path, document.MimeType);
            }

            _logger.LogInformation("Sending document file {Path} with MimeType of {MimeType} and type {DocumentType} for download: {Download}",
                path, document.MimeType, document.DocumentType.ToString(), download);

            return result;
        }

        private byte[] GeneratePdfFile(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                DocumentTitle = "Publication Agreement"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var pdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var pdfFile = _converter.Convert(pdfDocument);

            return pdfFile;
        }

        private byte[] StampPdf(byte[] pdfContent, string signerNames)
        {
            using (var reader = new PdfReader(pdfContent))
            using (var stream = new MemoryStream())
            {
                using (var stamper = new PdfStamper(reader, stream))
                {
                    var info = reader.Info;
                    info["Author"] = $"{SiteConstant.AdministratorEmail}";
                    info["Creator"] = _config.ServerUrl;
                    info["Subject"] = signerNames;
                    stamper.MoreInfo = info;
                }

                return stream.ToArray();
            }
        }

    }
}
