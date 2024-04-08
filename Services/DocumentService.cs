using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using Microsoft.Extensions.Options;

namespace linc.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;

        public DocumentService(ApplicationDbContext context, 
            IOptions<ApplicationConfig> configOptions, 
            ILogger<DocumentService> logger)
        {
            _context = context;
            _logger = logger;
            _config = configOptions.Value;
        }

        public async Task<ApplicationDocument> GetDocumentAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<ApplicationDocument> GetDocumentWithContentAsync(int id)
        {
            var document = await GetDocumentAsync(id);
            if (document is null)
            {
                return null;
            }

            var filePath = GetDocumentFilePath(document);
            if (filePath is not null)
            {
                document.Content = await File.ReadAllBytesAsync(filePath);
            }

            return document;
        }

        public string GetDocumentFilePath(ApplicationDocument document)
        {
            var path = Path.Combine(_config.RepositoryPath, document.RelativePath);

            if (File.Exists(path))
            {
                return path;
            }

            _logger.LogWarning("Could not find physical file for document {@Document}", document);

            return null;
        }
    }
}
