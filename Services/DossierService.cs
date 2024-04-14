using System.ComponentModel.DataAnnotations;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace linc.Services
{
    public class DossierService : IDossierService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DossierService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationConfig _config;

        public class JournalEntryKeys
        {
            public const string Created = "JournalDossier_Created";
            public const string AssignedTo = "JournalDossier_AssignedTo";
            public const string ReAssignedTo = "JournalDossier_ReassignedTo";
        }

        public DossierService(ApplicationDbContext context, 
            IOptions<ApplicationConfig> configOptions, 
            IHttpContextAccessor httpContextAccessor, 
            ILocalizationService localizationService, 
            ILogger<DossierService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _logger = logger;
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
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
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

        public async Task<DossierDetailsViewModel> GetDossierDetailsViewModelAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Documents)
                .Include(x => x.AssignedTo)
                .Include(x => x.Journals)
                    .ThenInclude(x => x.PerformedBy);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            var viewModel = new DossierDetailsViewModel()
            {
                Id = dossier.Id,
                Title = dossier.Title,
                AuthorNames = dossier.Names,
                AuthorEmail = dossier.Email,
                Status = dossier.Status,

                AssignedToId = dossier.AssignedToId,
                AssignedTo = dossier.AssignedTo != null ? 
                    dossier.AssignedTo.Names : 
                    string.Empty,

                // TODO: filter?
                Journals = dossier.Journals.ToList(),
                Documents = dossier.Documents
                    .OrderBy(x => x.DocumentType)
                    .ToList()
            };

            return viewModel;
        }

        public async Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Documents)
                .Include(x => x.AssignedTo);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            var editors = _context.GetAllByRole(SiteRole.Editor.ToString());
            var headEditors = _context.GetAllByRole(SiteRole.HeadEditor.ToString());

            var selectList = new List<SelectListItem>()
            {
                // localize
                new(_localizationService["DossierEdit_AssignEditor_Prompt"].Value, string.Empty)
            };

            selectList.AddRange(editors.Select(x => new SelectListItem(text: x.Names, x.Id)));
            selectList.AddRange(headEditors.Select(x => new SelectListItem(text: x.Names, x.Id)));

            if (dossier.AssignedToId != null)
            {
                var assignedTo = selectList.Find(x => x.Value.Equals(dossier.AssignedToId));
                assignedTo!.Selected = true;
            }

            var viewModel = new DossierEditViewModel(dossier.Documents.ToList())
            {
                Id = dossier.Id,

                Title = dossier.Title,
                FirstName = dossier.FirstName,
                LastName = dossier.LastName,
                Email = dossier.Email,

                Editors = selectList,

                Assignee = dossier.AssignedTo != null ?
                    dossier.AssignedTo.Names :
                    string.Empty

            };

            return viewModel;
        }

        public async Task<int> CreateDossierAsync(DossierCreateViewModel input)
        {
            var currentUserId = GetCurrentUserId();
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var original = await SaveDossierDocumentAsync(input.OriginalFile, ApplicationDocumentType.Original);
            var entry = new ApplicationDossier
            {
                Title = input.Title,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,

                Status = ApplicationDossierStatus.New,

                CreatedById = currentUserId
            };

            entry.Documents.Add(original);

            entry.Journals.Add(new DossierJournal
            {
                Message = JournalEntryKeys.Created,
                PerformedById = currentUserId
            });

            var entityEntry = await _context.Dossiers.AddAsync(entry);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
        }

        private async Task<ApplicationDocument> SaveDossierDocumentAsync(IFormFile inputFile, ApplicationDocumentType type)
        {
            if (inputFile == null)
            {
                return null;
            }

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

        private async Task DeleteDossierDocument(ApplicationDossier dossier, ApplicationDocument document)
        {
            var filePath = Path.Combine(_config.RepositoryPath, document.RelativePath);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning(
                    "Could not find a physical file path for dossier {DossierId} document {DocumentId}, deleting only from the database...",
                    dossier.Id, document.Id);
            }
            else
            {
                File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssigneeAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var dossier = await _context.Dossiers.FindAsync(id);

            ArgumentNullException.ThrowIfNull(dossier);

            _context.Dossiers.Attach(dossier);
            dossier.AssignedToId = currentUserId;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ApplicationDossierStatus status)
        {
            var dossier = await _context.Dossiers.FindAsync(id);

            ArgumentNullException.ThrowIfNull(dossier);

            _context.Dossiers.Attach(dossier);
            dossier.Status = status;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDossierAsync(DossierEditViewModel input)
        {
            // validation has succeeded, trust all the data and the state of integrity

            var dossier = _context.Dossiers
                .Include(x => x.Documents)
                .FirstOrDefault(x => x.Id == input.Id);

            ArgumentNullException.ThrowIfNull(dossier);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Dossiers.Attach(dossier);

            dossier.AssignedToId = input.AssigneeId;

            // NOTE: Perform clearly defined actions based on the current dossier status

            switch (dossier.Status)
            {
                case ApplicationDossierStatus.New:
                {
                    // allow uploading an anonymized document
                    var document = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Anonymized);
                    dossier.Documents.Add(document);

                    // update dossier status
                    await UpdateStatusAsync(dossier.Id, ApplicationDossierStatus.Prepared);
                    break;
                }
                case ApplicationDossierStatus.Prepared:
                {
                    // allow overriding of anonymized document
                    await DeleteDossierDocument(dossier, dossier.Anonymized);
                    var anonymizedDocument = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Anonymized);
                    dossier.Documents.Add(anonymizedDocument);
                    break;
                }
                case ApplicationDossierStatus.InReview:
                    // allow uploading a review document
                    break;
                case ApplicationDossierStatus.Reviewed:
                    break;
                case ApplicationDossierStatus.Accepted:
                    break;
                case ApplicationDossierStatus.AcceptedWithCorrections:
                    break;
                case ApplicationDossierStatus.AwaitingCorrections:
                    break;
                case ApplicationDossierStatus.Rejected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId();
        }
    }
}
