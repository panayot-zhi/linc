using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var document = _context.Documents
                .Include(x => x.Sources)
                .FirstOrDefault(x => x.Id == documentId);

            ArgumentNullException.ThrowIfNull(document);

            if (document.Sources.Count > 1)
            {
                _logger.LogWarning("Document is connected to more than one source. Skipping deletion.");
                return false;
            }

            var repositoryPath = _config.RepositoryPath;
            var filePath = Path.Combine(repositoryPath, document.RelativePath);

            File.Delete(filePath);

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
