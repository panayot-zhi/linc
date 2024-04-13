using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Models.ViewModels.Source;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace linc.Services
{
    public class DossierService : IDossierService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;

        public DossierService(ApplicationDbContext context, IOptions<ApplicationConfig> configOptions)
        {
            _context = context;
            _config = configOptions.Value;
        }

        public async Task<DossierIndexViewModel> GetDossiersPagedAsync(int? pageIndex, string sortPropertyName, SiteSortOrder sortOrder, int pageSize = 10)
        {
            if (!pageIndex.HasValue)
            {
                pageIndex = 1;
            }

            if (string.IsNullOrEmpty(sortPropertyName))
            {
                sortPropertyName = nameof(ApplicationDossier.Status);
                sortOrder = SiteSortOrder.Asc;
            }

            var dossiersDbSet = _context.Dossiers;
            var query = dossiersDbSet
                // .Include(x => x.Documents)
                .OrderBy(sortPropertyName, sortOrder)
                .AsQueryable();

            var count = await query.CountAsync();

            var dossiers = query
                .Skip((pageIndex.Value - 1) * pageSize)
                .Take(pageSize);

            return new DossierIndexViewModel(count, pageIndex.Value, pageSize)
            {
                Records = await dossiers.ToListAsync()
            };
        }


        public async Task<int> CreateDossierAsync(DossierCreateViewModel input, string currentUserId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var original = await SaveDossierDocumentAsync(input.OriginalFile, ApplicationDocumentType.Original);
            var entry = new ApplicationDossier()
            {
                Title = input.Title,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,

                Status = ApplicationDossierStatus.New,

                CreatedById = currentUserId
            };

            entry.Documents.Add(original);

            // TODO: journal entry

            var entityEntry = await _context.Dossiers.AddAsync(entry);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
        }

        private async Task<ApplicationDocument> SaveDossierDocumentAsync(IFormFile inputFile, ApplicationDocumentType type)
        {
            var fileExtension = inputFile.Extension();
            var fileName = Guid.NewGuid().ToString();

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.DossiersFolderName);
            var filePath = Path.Combine(rootFolderPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(rootFolderPath);

            var relativePath = Path.Combine(SiteConstant.DossiersFolderName, $"{fileName}.{fileExtension}");

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await inputFile.CopyToAsync(fileStream);
            }

            var entry = new ApplicationDocument()
            {
                DocumentType = type,
                Extension = fileExtension,
                FileName = fileName,
                MimeType = inputFile.ContentType,
                OriginalFileName = inputFile.FileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
