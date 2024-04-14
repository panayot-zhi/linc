using linc.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace linc.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly ISourceService _sourceService;
        private readonly IIssueService _issueService;

        public DocumentController(
            IIssueService issueService,
            ISourceService sourceService,
            IDocumentService documentService,
            ILocalizationService localizationService, 
            ILogger<DocumentController> logger) 
            : base(localizationService)
        {
            _documentService = documentService;
            _logger = logger;
            _issueService = issueService;
            _sourceService = sourceService;
        }

        public async Task<IActionResult> DownloadDocumentFile(int? id, bool useOriginalFileName = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentService.GetDocumentAsync(id.Value);
            if (document is null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(document.Id);
            if (result is null)
            {
                return NotFound();
            }

            result.FileDownloadName = !useOriginalFileName ? 
                $"{document.FileName}.{document.Extension}" : 
                document.OriginalFileName;

            return result;
        }

        public async Task<IActionResult> LoadDocumentFile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(id.Value);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        public async Task<IActionResult> LoadIssuePdf(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var issue = await _issueService.GetIssueAsync(id.Value);
            if (issue is null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(issue.Pdf.Id);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        public async Task<IActionResult> LoadSourcePdf(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var source = await _sourceService.GetSourceAsync(id.Value);
            if (source is null)
            {
                return NotFound();
            }

            var result = await GetDocumentFile(source.PdfId);
            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        private async Task<PhysicalFileResult> GetDocumentFile(int documentId)
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

            _logger.LogInformation("Sending document file {FileName} with MimeType of {MimeType} and type {DocumentType}",
                document.FileName, document.MimeType, document.DocumentType.ToString());

            // don't append FileDownloadName since we do not want to force
            // browsers to download the file, but instead attempt to display it
            return new PhysicalFileResult(path, document.MimeType);
        }
    }
}
