using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Packaging;

namespace linc.Services
{
    public class IssueService : IIssueService
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;

        public IssueService(ApplicationDbContext context, IOptions<ApplicationConfig> configOptions)
        {
            _context = context;
            _config = configOptions.Value;
        }

        public async Task<ApplicationDocument> GetFileAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<ApplicationIssue> GetIssueAsync(int id)
        {
            // var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            // var languageId = SiteConstant.SupportedCultures.First(x =>
            //     x.Value == currentCulture).Key;

            return await _context.Issues
                .Include(x => x.Files)
                .Include(x => x.Sources)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ApplicationIssue>> GetIssuesAsync()
        {
            return await _context.Issues.ToListAsync();
        }

        public async Task<int> CreateIssueAsync(IssueCreateViewModel input)
        {
            var issueNumber = input.IssueNumber.Value;
            var releaseYear = input.ReleaseYear.Value;

            var pdf = await SaveIssuePdf(input.PdfFile, releaseYear, issueNumber);
            var cover = await SaveIssueCoverPage(input.CoverPage, releaseYear, issueNumber);
            var indexPages = await SaveIssueIndexPages(input.IndexPages.ToArray(), releaseYear, issueNumber);

            var entry = new ApplicationIssue()
            {
                IsAvailable = false,

                Description = input.Description,
                IssueNumber = input.IssueNumber.Value,
                ReleaseYear = input.ReleaseYear.Value
            };

            entry.Files.Add(pdf);
            entry.Files.Add(cover);
            entry.Files.AddRange(indexPages);

            var entityEntry = await _context.Issues.AddAsync(entry);
            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        private async Task<ApplicationDocument> SaveIssuePdf(IFormFile inputPdfFile, int releaseYear, int issueNumber)
        {
            return await SaveIssuePage(inputPdfFile, ApplicationDocumentType.IssuePdf, releaseYear, issueNumber);
        }

        private async Task<ApplicationDocument> SaveIssueCoverPage(IFormFile inputFile, int releaseYear, int issueNumber)
        {
            return await SaveIssuePage(inputFile, ApplicationDocumentType.CoverPage, releaseYear, issueNumber);
        }

        private async Task<ApplicationDocument[]> SaveIssueIndexPages(IEnumerable<IFormFile> inputFiles, int releaseYear, int issueNumber)
        {
            var result = new List<ApplicationDocument>();

            foreach (var inputFile in inputFiles)
            {
                var entry = await SaveIssuePage(inputFile, ApplicationDocumentType.IndexPage, releaseYear, issueNumber);
                result.Add(entry);
            }

            return result.ToArray();
        }

        private async Task<ApplicationDocument> SaveIssuePage(IFormFile inputFile, ApplicationDocumentType type, 
            int releaseYear, int issueNumber)
        {
            var fileExtension = inputFile.Extension();
            var fileName = $"{releaseYear}-{issueNumber.ToString().PadLeft(3, '0')}-{HelperFunctions.ToKebabCase(type.ToString())}";
            if (type == ApplicationDocumentType.IndexPage)
            {
                fileName = fileName + "-" + "1".PadLeft(2, '0');
            }

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.IssuesFolderName);
            var directoryPath = Path.Combine(rootFolderPath, releaseYear.ToString());
            var filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(directoryPath);
            if (type == ApplicationDocumentType.IndexPage && File.Exists(filePath))
            {
                var number = 2;
                while (File.Exists(filePath))
                {
                    fileName = fileName.Remove(fileName.Length - 2, 2);
                    fileName = fileName + number.ToString().PadLeft(2, '0');
                    filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");
                    number++;
                }
            }

            var relativePath = Path.Combine(SiteConstant.IssuesFolderName, releaseYear.ToString(),
                $"{fileName}.{fileExtension}");

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
                Title = inputFile.FileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
