using linc.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace linc.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(
            IDocumentService documentService,
            ILocalizationService localizationService, 
            ILogger<DocumentController> logger) 
            : base(localizationService)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [Route("document/{documentId:int}")]
        public async Task<IActionResult> LoadDocumentFile(int? documentId)
        {
            if (documentId == null)
            {
                return NotFound();
            }

            var document = await _documentService.GetDocumentAsync(documentId.Value);
            if (document is null)
            {
                return NotFound();
            }

            var path = _documentService.GetDocumentFilePath(document);
            if (path is null)
            {
                return NotFound();
            }

            _logger.LogInformation("Sending document file {FileName} with MimeType of {MimeType} and type {DocumentType}",
                document.FileName, document.MimeType, document.DocumentType.ToString());

            return new PhysicalFileResult(path, document.MimeType)
            {
                FileDownloadName = $"{document.FileName}.{document.Extension}"
            };
        }

        [Route("issue/{pdfId:int}")]
        public async Task<IActionResult> LoadIssuePdf(int? pdfId)
        {
            if (pdfId == null)
            {
                return NotFound();
            }

            var issuePdf = await _documentService.GetDocumentAsync(pdfId.Value);
            if (issuePdf is null)
            {
                return NotFound();
            }

            var issuePdfPath = _documentService.GetDocumentFilePath(issuePdf);
            if (issuePdfPath is null)
            {
                return NotFound();
            }

            _logger.LogInformation("Loading issue pdf {FileName}", issuePdf.FileName);

            // don't append FileDownloadName since we do not want to force
            // browsers to download the file, but instead attempt to display it
            return new PhysicalFileResult(issuePdfPath, issuePdf.MimeType);
        }

        [Route("source/{pdfId:int}")]
        public async Task<IActionResult> LoadSourcePdf(int? pdfId)
        {
            if (pdfId == null)
            {
                return NotFound();
            }

            var sourcePdf = await _documentService.GetDocumentAsync(pdfId.Value);
            if (sourcePdf is null)
            {
                return NotFound();
            }

            var sourcePdfPath = _documentService.GetDocumentFilePath(sourcePdf);
            if (sourcePdfPath is null)
            {
                return NotFound();
            }

            _logger.LogInformation("Loading source pdf {FileName}", sourcePdf.FileName);

            // don't append FileDownloadName since we do not want to force
            // browsers to download the file, but instead attempt to display it
            return new PhysicalFileResult(sourcePdfPath, sourcePdf.MimeType);
        }
    }
}
