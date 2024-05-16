using iTextSharp.text;
using iTextSharp.text.pdf;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Source;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace linc.Services
{
    public class SourceService : ISourceService
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<SourceService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;

        public SourceService(ApplicationDbContext context, 
            IOptions<ApplicationConfig> config, 
            ILogger<SourceService> logger,
            IDocumentService documentService)
        {
            _context = context;
            _documentService = documentService;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<ApplicationSource> GetSourceAsync(int id)
        {
            return await _context.Sources.FindAsync(id);
        }

        public async Task<SourceIndexViewModel> GetSourcesPagedAsync(string filter, int languageId, int? year, int? issueId, int? pageIndex, int pageSize = 10)
        {
            if (!pageIndex.HasValue)
            {
                pageIndex = 1;
            }

            var sourcesDbSet = _context.Sources;
            var query = sourcesDbSet
                .Include(x => x.Issue)
                //.Where(x => x.LanguageId == languageId)
                .AsQueryable();

            if (issueId.HasValue)
            {
                query = query.Where(x => x.IssueId == issueId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(x => x.DateCreated.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                if ("*".Equals(filter))
                {
                    // skip
                }
                else if (filter.Length > 1)
                {
                    // perform search by
                    query = query.Where(x =>
                        EF.Functions.Like(x.AuthorNames, $"%{filter}%") ||
                        EF.Functions.Like(x.AuthorNotes, $"%{filter}%")
                    );
                }
                else
                {
                    query = query.Where(x => x.LastName.StartsWith(filter));
                    query = query.OrderBy(x => x.LastName);
                }
            }
            else
            {
                // orders the items such that the items with LanguageId equal to the specifiedLanguageId come first.
                // The ternary operator assigns 0 to items with the specific LanguageId and 1 to others,
                // effectively pushing the desired LanguageId to the top.
                query = query.OrderBy(x => x.LanguageId == languageId ? 0 : 1)
                    .ThenBy(x => x.LanguageId)
                        .ThenBy(x => x.Issue.ReleaseDate)
                            .ThenBy(x => x.StartingPage);
            }

            var count = await query.CountAsync();

            var sources = query
                .Skip((pageIndex.Value - 1) * pageSize)
                .Take(pageSize);

            return new SourceIndexViewModel(count, pageIndex.Value, pageSize)
            {
                Records = await sources.ToListAsync()
            };
        }

        public async Task<List<SourceCountByYears>> GetSourcesCountByYears()
        {
            return await _context.Sources
                .Include(x => x.Issue)
                .GroupBy(x => x.Issue.ReleaseDate.Year)
                .Select(x => new SourceCountByYears()
                {
                    Year = x.Key,
                    Count = x.Count()

                }).ToListAsync();
        }

        public async Task<List<SourceCountByIssues>> GetSourcesCountByIssues()
        {
            return await _context.Sources
                .GroupBy(source => source.IssueId)
                .Select(group => new SourceCountByIssues
                {
                    IssueId = group.Key,
                    IssueTitle = $"{group.First().Issue.IssueNumber}/{group.First().Issue.DateCreated.Year}",
                    Count = group.Count()
                }).ToListAsync();
        }

        public async Task<int> CreateSourceAsync(SourceCreateViewModel input)
        {
            // validation should guard these from being nulls
            var startingPage = input.StartingPage!.Value;
            var lastPage = input.LastPage!.Value;
            var issueId = input.IssueId!.Value;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var issue = _context.Issues
                .Include(x => x.Files)
                .First(x => x.Id == issueId);

            var authorId = await FindAuthorByNamesAsync(input.FirstName, input.LastName);

            ApplicationDocument pdf;
            if (input.PdfFile != null)
            {
                // pdf file was provided, just save it and continue
                pdf = await SaveSourcePdf(input.PdfFile, startingPage, issue.ReleaseYear, issue.IssueNumber);
            }
            else
            {
                // no pdf file was provided, generate it from the issue pdf
                pdf = await GenerateSourcePdf(issue, startingPage, lastPage);
            }

            var entity = new ApplicationSource
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                AuthorNotes = input.AuthorNotes,

                StartingPage = startingPage,
                LastPage = lastPage,

                Title = input.Title,
                TitleNotes = input.TitleNotes,

                LanguageId = input.LanguageId,
                IssueId = issueId,
                AuthorId = authorId,

                PdfId = pdf.Id
            };

            var entityEntry = await _context.Sources.AddAsync(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
        }

        private async Task<ApplicationDocument> GenerateSourcePdf(ApplicationIssue issue, int startingPage, int lastPage)
        {
            var issuePdf = await _documentService.GetDocumentAsync(issue.Pdf.Id);
            var issuePdfPath = _documentService.GetDocumentFilePath(issuePdf);
            var issueNumber = issue.IssueNumber;
            var releaseYear = issue.ReleaseYear;

            const ApplicationDocumentType type = ApplicationDocumentType.SourcePdf;
            var fileName = $"{issue.ReleaseYear}-{issueNumber.ToString().PadLeft(3, '0')}-{HelperFunctions.ToKebabCase(type.ToString())}-{startingPage}";

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.IssuesFolderName);
            var directoryPath = Path.Combine(rootFolderPath, releaseYear.ToString());
            var filePath = Path.Combine(directoryPath, $"{fileName}.pdf");

            if (!File.Exists(filePath))
            {
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document();
                    var issuePdfReader = new PdfReader(issuePdfPath);
                    var pdfWriter = new PdfCopy(document, stream);

                    document.Open();

                    // Loop through the specified range and add the pages to the new document
                    for (var pageNumber = startingPage; pageNumber <= lastPage; pageNumber++)
                    {
                        var page = pdfWriter.GetImportedPage(issuePdfReader, pageNumber);
                        pdfWriter.AddPage(page);
                    }

                    document.Close();
                    pdfWriter.Close();
                    issuePdfReader.Close();
                }
            }
            else
            {
                _logger.LogWarning("SourcePdf could not be generated, because the file at the specified location already exists {FilePath}.{NewLine}" +
                                   "This can occur when two sources point to the same part of the pdf.",
                    filePath, Environment.NewLine);
            }

            var relativePath = Path.Combine(SiteConstant.IssuesFolderName, releaseYear.ToString(), $"{fileName}.pdf");

            var entry = new ApplicationDocument()
            {
                DocumentType = type,
                Extension = "pdf",
                FileName = fileName,
                MimeType = MediaTypeNames.Application.Pdf,
                OriginalFileName = fileName,
                RelativePath = relativePath
            };

            var entityEntry = await _context.Documents.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }

        private async Task<ApplicationDocument> SaveSourcePdf(IFormFile inputFile, int startingPage, int releaseYear, int issueNumber)
        {
            var fileExtension = inputFile.Extension();
            const ApplicationDocumentType type = ApplicationDocumentType.SourcePdf;
            var fileName = $"{releaseYear}-{issueNumber.ToString().PadLeft(3, '0')}-{HelperFunctions.ToKebabCase(type.ToString())}-{startingPage}";

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.IssuesFolderName);
            var directoryPath = Path.Combine(rootFolderPath, releaseYear.ToString());
            var filePath = Path.Combine(directoryPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(directoryPath);

            var relativePath = Path.Combine(SiteConstant.IssuesFolderName, releaseYear.ToString(),
                $"{fileName}.{fileExtension}");

            if (!File.Exists(filePath))
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await inputFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                _logger.LogWarning("SourcePdf could not be saved, because the file at the specified location already exists {FilePath}.{NewLine}" +
                                   "This can occur when two sources point to the same part of the pdf.",
                    filePath, Environment.NewLine);
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


        private async Task<string> FindAuthorByNamesAsync(string inputFirstName, string inputLastName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    EF.Functions.Like(x.FirstName, $"{inputFirstName}") &&
                    EF.Functions.Like(x.LastName, $"{inputLastName}")
                );

            if (user is null)
            {
                return null;
            }

            _context.Users.Attach(user);
            
            user.IsAuthor = true;

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}
