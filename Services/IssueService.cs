using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
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
            var fileName = $"{releaseYear}-{issueNumber.ToString().PadLeft(3, '0')}";
            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.IssuesFolderName);
            var directoryPath = Path.Combine(rootFolderPath, releaseYear.ToString());
            var filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");
            var relativePath = Path.Combine(SiteConstant.IssuesFolderName, releaseYear.ToString(),
                $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(directoryPath);

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
