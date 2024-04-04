using linc.Contracts;
using linc.Models.ConfigModels;
using linc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace linc.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly ApplicationConfig _config;

        public DocumentController(
            IDocumentService documentService,
            IOptions<ApplicationConfig> configOptions,
            ILocalizationService localizationService, 
            ILogger<DocumentController> logger) 
            : base(localizationService)
        {
            _documentService = documentService;
            _logger = logger;
            _config = configOptions.Value;
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadDocumentFile(int id)
        {
            var file = await _documentService.GetFileAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            var path = Path.Combine(_config.RepositoryPath, file.RelativePath);

            if (!System.IO.File.Exists(path))
            {
                _logger.LogWarning("Could not find physical file {@File}",
                    file);
                return NotFound();
            }

            return new PhysicalFileResult(path, file.MimeType)
            {
                FileDownloadName = $"{file.FileName}.{file.Extension}"
            };
        }
    }
}
