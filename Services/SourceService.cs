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
        private readonly IApplicationUserStore _applicationUserStore;
        private readonly IDocumentService _documentService;
        private readonly ILogger<SourceService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;
        private readonly IAuthorService _authorService;

        public SourceService(ApplicationDbContext context, 
            IOptions<ApplicationConfig> config, 
            ILogger<SourceService> logger,
            IApplicationUserStore applicationUserStore,
            IDocumentService documentService,
            IAuthorService authorService)
        {
            _context = context;
            _documentService = documentService;
            _applicationUserStore = applicationUserStore;
            _config = config.Value;
            _logger = logger;
            _authorService = authorService;
        }

        public async Task<ApplicationSource> GetSourceAsync(int id)
        {
            return await _context.Sources
                .Include(x => x.Authors)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
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
                .Include(x => x.Authors)
                .Where(x => !x.IsSection)
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
                        x.Authors.Any(a => EF.Functions.Like(a.FirstName, $"%{filter}%") || EF.Functions.Like(a.LastName, $"%{filter}%")) ||
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
                // Orders the items such that the items with LanguageId equal to the specifiedLanguageId come first.
                // The ternary operator assigns 0 to items with the specific LanguageId and 1 to others,
                // effectively pushing the desired LanguageId to the top.
                // ThenBy(x => x.LanguageId) is added to maintain the order of the remaining LanguageIds
                // after the specific one is ordered first.
                query = query.OrderBy(x => x.LanguageId == languageId ? 0 : 1)
                    .ThenBy(x => x.LanguageId)
                        .ThenBy(x => x.Issue.ReleaseDate)
                            .ThenBy(x => x.StartingPdfPage);
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

        public async Task<SourceIndexViewModel> GetAdminSourcesPagedAsync(int languageId, int? pageIndex, int pageSize = 15)
        {
            if (!pageIndex.HasValue)
            {
                pageIndex = 1;
            }

            var sourcesDbSet = _context.Sources;
            var query = sourcesDbSet
                .Include(x => x.Issue)
                .Include(x => x.Authors)
                    .ThenInclude(x => x.User)
                .Where(x => x.LanguageId == languageId)
                .AsQueryable();

            query = query
                .OrderByDescending(x => x.Issue.ReleaseDate)
                .ThenBy(source => source.StartingPdfPage)       // order first and foremost by the starting page number
                .ThenByDescending(source => source.IsSection)   // some sections begin on the same pages, they should be displayed first
                .ThenBy(source => source.DateCreated);          // order additionally by the date created

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
                    IssueTitle = IIssueService.DisplayIssueLabelInformation(group.First().Issue.IssueNumber, group.First().Issue.DateCreated.Year),
                    Count = group.Count()
                }).ToListAsync();
        }

        public async Task<int> CreateSourceAsync(SourceCreateViewModel input)
        {
            // validation should guard these from being nulls
            var startingPage = input.StartingPdfPage!.Value;
            var lastPage = input.LastPdfPage!.Value;
            var issueId = input.IssueId!.Value;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var issue = _context.Issues
                .Include(x => x.Documents)
                .First(x => x.Id == issueId);

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
                AuthorNotes = input.AuthorNotes?.Trim(),
                DOI = input.DOI?.Trim(),
                IsSection = input.IsSection,
                IsTheme = input.IsTheme,
                IssueId = issueId,
                LanguageId = input.LanguageId,
                LastPdfPage = lastPage,
                PdfId = pdf.Id,
                StartingIndexPage = input.StartingIndexPage,
                StartingPdfPage = startingPage,
                Title = input.Title?.Trim(),
                TitleNotes = input.TitleNotes?.Trim()
            };

            var authors = await _authorService.CreateAuthorsAsync(input.Authors, input.LanguageId);
            foreach (var author in authors)
            {
                entity.Authors.Add(author);
            }

            var entityEntry = await _context.Sources.AddAsync(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
        }

        public async Task UpdateSourceAsync(SourceUpdateViewModel input)
        {
            var source = await _context.Sources.FindAsync(input.Id);

            ArgumentNullException.ThrowIfNull(source);

            _context.Sources.Attach(source);

            source.AuthorNotes = input.AuthorNotes?.Trim();
            source.DOI = input.DOI?.Trim();
            source.IsSection = input.IsSection;
            source.IsTheme = input.IsTheme;
            source.LanguageId = input.LanguageId;
            source.StartingIndexPage = input.StartingIndexPage;
            source.Title = input.Title?.Trim();
            source.TitleNotes = input.TitleNotes?.Trim();

            await _authorService.UpdateAuthorsAsync(source, input.Authors);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSourceAsync(int id)
        {
            var source = await _context.Sources.FindAsync(id);

            ArgumentNullException.ThrowIfNull(source);

            // If this is the last PDF document connected to a source it
            // will automatically delete the source because of cascading deletion:
            // no source can exist without a corresponding pdf file
            var documentDeleted = await _documentService.DeleteDocumentAsync(source.PdfId);
            if (!documentDeleted)
            {
                // If the document was not deleted, because it is connected to other sources
                // we need to still perform remove of the source from the database
                _context.Sources.Remove(source);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<ApplicationDocument> GenerateSourcePdf(ApplicationIssue issue, int startingPage, int lastPage)
        {
            var issuePdf = await _documentService.GetDocumentAsync(issue.Pdf.Id);
            var issuePdfPath = _documentService.GetDocumentFilePath(issuePdf);
            var issueNumber = issue.IssueNumber;
            var releaseYear = issue.ReleaseYear;

            const ApplicationDocumentType type = ApplicationDocumentType.SourcePdf;
            var fileName = $"{issue.ReleaseYear}-{issueNumber.ToString().PadLeft(3, '0')}-{HelperFunctions.ToKebabCase(type.ToString())}-{startingPage}-{lastPage}";

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.IssuesFolderName);
            var directoryPath = Path.Combine(rootFolderPath, releaseYear.ToString());
            var filePath = Path.Combine(directoryPath, $"{fileName}.pdf");

            var relativePath = Path.Combine(SiteConstant.IssuesFolderName, releaseYear.ToString(), $"{fileName}.pdf");

            if (File.Exists(filePath))
            {
                _logger.LogWarning(
                    "SourcePdf could not be generated, because the file at the specified location already exists {FilePath}.{NewLine}" +
                    "This can occur when two sources point to the same part of the pdf. Linking the same database document...",
                    filePath, Environment.NewLine);

                return _context.Documents.First(x =>
                    x.RelativePath == relativePath);

            }

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

            if (File.Exists(filePath))
            {
                _logger.LogWarning(
                    "SourcePdf could not be saved, because the file at the specified location already exists {FilePath}.{NewLine}" +
                    "This can occur when two sources point to the same part of the pdf. Linking the same database document...",
                    filePath, Environment.NewLine);

                return _context.Documents.First(x => 
                    x.RelativePath == relativePath);
            }

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

        public async Task UpdateAuthorAsync(ApplicationUser user)
        {
            var userProfiles = _context.UserProfiles
                .Where(x => x.UserId == user.Id)
                .ToList();

            var dbSources = _context.Sources
                .Where(x => x.AuthorId == null)
                .Where(x => x.FirstName != null)
                .Where(x => x.LastName != null)
                .ToList();

            foreach (var userProfile in userProfiles)
            {
                var sources = dbSources
                    .Where(x => string.Equals(x.FirstName, userProfile.FirstName, StringComparison.CurrentCultureIgnoreCase))
                    .Where(x => string.Equals(x.LastName, userProfile.LastName, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

                if (!sources.Any())
                {
                    continue;
                }

                if (!user.IsAuthor)
                {
                    _context.Users.Attach(user);
                    user.IsAuthor = true;
                }

                foreach (var source in sources)
                {
                    _context.Sources.Attach(source);
                    source.AuthorId = user.Id;
                }

                await _context.SaveChangesAsync();
            }
        }

    }
}
