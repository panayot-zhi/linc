using linc.Contracts;
using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace linc.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IDossierService _dossierService;
        private readonly ISourceService _sourceService;
        private readonly IAuthorService _authorService;
        private readonly IIssueService _issueService;

        public DocumentController(
            IIssueService issueService,
            IAuthorService authorService,
            ISourceService sourceService,
            IDossierService dossierService,
            IDocumentService documentService,
            ILocalizationService localizationService, 
            ILogger<DocumentController> logger) 
            : base(localizationService)
        {
            _logger = logger;
            _authorService = authorService;
            _documentService = documentService;
            _dossierService = dossierService;
            _issueService = issueService;
            _sourceService = sourceService;
        }

        [HttpGet("issue/{issueId:int}/document/{documentId:int?}", Name = nameof(LoadIssueDocument))]
        [AllowAnonymous]
        public async Task<IActionResult> LoadIssueDocument(int issueId, int? documentId, bool download = false)
        {
            // NOTE: documentId can be nullable here in order to return the pdf as the default document if no documentId is passed
            var issueDocument = await _issueService.GetIssueDocumentAsync(issueId, documentId);
            if (issueDocument is null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(issueDocument.Id, download);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("source/{sourceId:int}/document")]
        [AllowAnonymous]
        public async Task<IActionResult> LoadSourceDocument(int sourceId, bool download = false)
        {
            var source = await _sourceService.GetSourceAsync(sourceId);
            if (source is null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(source.PdfId, download);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("dossier/{dossierId:int}/document/{documentId:int}")]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> LoadDossierDocument(int dossierId, int documentId, bool download = true)
        {
            // NOTE: most of the time one would wish to download the dossier file so download is true by default

            var dossierDocument = await _dossierService.GetDossierDocumentAsync(dossierId, documentId);
            if (dossierDocument == null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(documentId, download);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet("dossier/{dossierId:int}/author/{authorId:int}")]
        [SiteAuthorize(SiteRole.Editor)]
        public async Task<IActionResult> LoadAuthorAgreement(int dossierId, int authorId, bool download = true)
        {
            // NOTE: most of the time one would wish to download the dossier file so download is true by default

            var dossier = await _dossierService.GetDossierAsync(dossierId);
            if (dossier == null)
            {
                return NotFound();
            }

            var author = await _authorService.GetAuthorAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            if (!author.AgreementId.HasValue)
            {
                return BadRequest("There is no publication agreement attached for this user and dossier.");
            }

            var result = await GetDocumentFile(author.AgreementId.Value, download);
            if (result is null)
            {
                return NotFound();
            }

            return result;
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
    }
}
