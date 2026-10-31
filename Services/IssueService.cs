using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Issue;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

        public async Task<ApplicationIssue> GetIssueAsync(int id, int? sourcesLanguageId = null)
        {
            var query = _context.Issues
                .Include(x => x.Documents)
                .Where(x => x.Id == id);

            if (sourcesLanguageId.HasValue)
            {
                query = query
                    .Include(x => x.Sources
                            .Where(source => source.LanguageId == sourcesLanguageId.Value)
                            .OrderBy(source => source.StartingPdfPage)      // order first and foremost by the starting page number
                            .ThenByDescending(source => source.IsSection)   // some sections begin on the same pages, they should be displayed first
                            .ThenBy(source => source.DateCreated))          // order additionally by the date created
                        .ThenInclude(s => s.Authors)
                    .AsSplitQuery();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ApplicationDocument> GetIssueDocumentAsync(int id, int? documentId)
        {
            var query = _context.Issues
                .Include(x => x.Documents)
                .Where(x => x.Id == id);

            var issue = await query.FirstOrDefaultAsync();
            if (issue == null)
            {
                return null;
            }

            return documentId.HasValue ? 
                issue.Documents.FirstOrDefault(x => x.Id == documentId) :
                issue.Pdf;
        }

        public async Task<List<ApplicationIssue>> GetIssuesAsync()
        {
            return await _context.Issues
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();
        }

        public async Task<int> CreateIssueAsync(IssueCreateViewModel input)
        {
            // validation should guard these from being nulls
            var issueNumber = input.IssueNumber!.Value;
            var releaseDate = input.ReleaseDate!.Value;
            var releaseYear = releaseDate.Year;

            var pdf = await SaveIssuePdf(input.PdfFile, releaseYear, issueNumber);
            var cover = await SaveIssueCoverPage(input.CoverPage, releaseYear, issueNumber);
            var indexPages = await SaveIssueIndexPages(input.IndexPages.ToArray(), releaseYear, issueNumber);

            var entry = new ApplicationIssue()
            {
                IsAvailable = false,

                Description = input.Description,
                IssueNumber = input.IssueNumber.Value,
                ReleaseDate = DateOnly.FromDateTime(releaseDate)
            };

            entry.Documents.Add(pdf);
            entry.Documents.Add(cover);

            foreach (var indexPage in indexPages)
            {
                entry.Documents.Add(indexPage);
            }

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
            var fileExtension = inputFile.GetExtension();
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
                    fileName += number.ToString().PadLeft(2, '0');
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
                OriginalFileName = inputFile.FileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
