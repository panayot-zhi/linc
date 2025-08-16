using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Utility;
using Microsoft.AspNetCore.Components.Forms;
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

        public async Task<ApplicationDocument> SaveDossierDocumentAsync(int dossierId, ApplicationDocumentDescriptor descriptor)
        {
            var fileName = Guid.NewGuid().ToString();
            var fileExtension = descriptor.Extension;

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.DossiersFolderName, dossierId.ToString());
            var filePath = Path.Combine(rootFolderPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(rootFolderPath);

            var relativePath = Path.Combine(SiteConstant.DossiersFolderName, dossierId.ToString(), $"{fileName}.{fileExtension}");

            await File.WriteAllBytesAsync(filePath, descriptor.Content);

            var entry = new ApplicationDocument()
            {
                DocumentType = descriptor.Type,
                Extension = fileExtension,
                FileName = fileName,
                MimeType = descriptor.ContentType,
                OriginalFileName = descriptor.FileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
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

            if (!File.Exists(filePath))
            {
                _logger.LogWarning(
                    "Could not find a physical file path for document {DocumentId}, deleting only from the database...",
                    document.Id);
            }
            else
            {
                File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
