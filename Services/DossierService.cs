using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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
            private const string Prefix = "JournalDossier";

            public const string Created = $"{Prefix}_{nameof(Created)}";
            public const string AssignedTo = $"{Prefix}_{nameof(AssignedTo)}";
            public const string ReAssignedTo = $"{Prefix}_{nameof(ReAssignedTo)}";
            public const string StatusUpdated = $"{Prefix}_{nameof(StatusUpdated)}";
            //public const string PropertyUpdated = $"{Prefix}_{nameof(PropertyUpdated)}";

            public const string DocumentUploaded = $"{Prefix}_{nameof(DocumentUploaded)}";
            public const string DocumentReUploaded = $"{Prefix}_{nameof(DocumentReUploaded)}";
            public const string ClearedAssignment = $"{Prefix}_{nameof(ClearedAssignment)}";
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

        public async Task<ApplicationDocument> GetDossierDocumentAsync(int id, int documentId)
        {
            var query = _context.Dossiers
                .Include(x => x.Documents)
                .Include(x => x.Reviews)
                    .ThenInclude(x => x.Review)
                .Where(x => x.Id == id);

            var dossier = await query.FirstOrDefaultAsync();
            if (dossier == null)
            {
                return null;
            }

            var allDocuments = new List<ApplicationDocument>(dossier.Documents);
            allDocuments.AddRange(dossier.Reviews.Select(x => x.Review));

            return allDocuments.FirstOrDefault(x => x.Id == documentId);
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

            var currentUser = GetCurrentUser();

            ArgumentNullException.ThrowIfNull(currentUser);

            if (currentUser.Is(SiteRole.Editor))
            {
                query = query.Where(x => 
                    x.Status == ApplicationDossierStatus.New || 
                    x.AssignedToId == currentUser.GetUserId()
                );
            }

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
                .Include(x => x.Reviews)
                    .ThenInclude(x => x.Review)
                .Include(x => x.Reviews)
                    .ThenInclude(x => x.Reviewer)
                .Include(x => x.Documents)
                .Include(x => x.AssignedTo)
                .Include(x => x.Journals)
                    .ThenInclude(x => x.PerformedBy);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            var allDocuments = new List<ApplicationDocument>(dossier.Documents);
            allDocuments.AddRange(dossier.Reviews.Select(x => x.Review));

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

                DateCreated = dossier.DateCreated,
                LastUpdated = dossier.LastUpdated,

                // TODO: filter?
                Journals = dossier.Journals.ToList(),
                Reviews = dossier.Reviews.ToList(),
                Documents = allDocuments
                    .OrderBy(x => x.DocumentType)
                    .ToList()
            };

            return viewModel;
        }

        public async Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Reviews)
                    .ThenInclude(x => x.Review)
                .Include(x => x.Documents)
                .Include(x => x.AssignedTo);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            var allDocuments = new List<ApplicationDocument>(dossier.Documents);
            allDocuments.AddRange(dossier.Reviews.Select(x => x.Review));

            var viewModel = new DossierEditViewModel(allDocuments.ToList())
            {
                Id = dossier.Id,

                Title = dossier.Title,
                FirstName = dossier.FirstName,
                LastName = dossier.LastName,
                Email = dossier.Email,

                Status = dossier.Status,

                AssigneeId = dossier.AssignedToId,
                AssigneeNames = dossier.AssignedTo != null ?
                    dossier.AssignedTo.Names :
                    string.Empty,

                Editors = GetEditors(dossier.AssignedToId),
                Reviewers = GetReviewers(),

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
                PerformedById = currentUserId,
                Message = JournalEntryKeys.Created
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

        public async Task UpdateAssigneeAsync(int id, string targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            var targetUser = await _context.Users.FindAsync(targetUserId);
            var dossier = await _context.Dossiers.FindAsync(id);

            ArgumentNullException.ThrowIfNull(dossier);

            _context.Dossiers.Attach(dossier);

            if (dossier.AssignedToId != null)
            {
                var currentAssignee = await _context.Users.FindAsync(dossier.AssignedToId);

                ArgumentNullException.ThrowIfNull(currentAssignee);

                if (targetUser != null)
                {
                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.ReAssignedTo,
                        MessageArguments = new[]
                        {
                            currentAssignee.UserName,
                            targetUser.UserName
                        }
                    });
                }
                else
                {
                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.ClearedAssignment,
                        MessageArguments = new[]
                        {
                            currentAssignee.UserName
                        }
                    });
                }
            }
            else
            {
                if (targetUser != null)
                {
                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.AssignedTo,
                        MessageArguments = new[]
                        {
                            targetUser.UserName
                        }
                    });
                }
            }

            dossier.AssignedToId = targetUserId;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, ApplicationDossierStatus status)
        {
            var currentUserId = GetCurrentUserId();
            var dossier = await _context.Dossiers.FindAsync(id);

            ArgumentNullException.ThrowIfNull(dossier);

            if (status == ApplicationDossierStatus.AwaitingCorrections && 
                dossier.Status is ApplicationDossierStatus.Accepted or ApplicationDossierStatus.AcceptedWithCorrections)
            {
                // only Head Editor can return to this state
                var user = GetCurrentUser();
                if (!user.IsAtLeast(SiteRole.HeadEditor))
                {
                    throw new ApplicationException($"User {user.GetUserName()} is not allowed to return the dossier to the desired status.");
                }
            }

            _context.Dossiers.Attach(dossier);

            dossier.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.StatusUpdated,
                // NOTE: Refactoring ApplicationDossierStatus
                // will break this convention
                MessageArguments = new[]
                {
                    $"DossierStatus_{dossier.Status}",
                    $"DossierStatus_{status}"
                }
            });

            dossier.Status = status;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDossierAsync(DossierEditViewModel input)
        {
            // validation has succeeded, trust all the data and the state of integrity

            var currentUserId = GetCurrentUserId();
            var dossier = _context.Dossiers
                .Include(x => x.Reviews)
                .Include(x => x.Documents)
                .FirstOrDefault(x => x.Id == input.Id);

            ArgumentNullException.ThrowIfNull(dossier);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Dossiers.Attach(dossier);

            await UpdateDossierPropertiesAsync(dossier, input);

            // NOTE: Perform clearly defined actions based on the current dossier status

            switch (dossier.Status)
            {
                case ApplicationDossierStatus.New:
                    
                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow uploading an anonymized document
                    var document = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Anonymized);
                    
                    dossier.Documents.Add(document);

                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.DocumentUploaded,
                        MessageArguments = new[]
                        {
                            "DocumentType_Anonymized"
                        }
                    });

                    // update dossier status
                    await UpdateStatusAsync(dossier.Id, ApplicationDossierStatus.Prepared);

                    break;

                case ApplicationDossierStatus.Prepared:

                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow overriding of anonymized document
                    await DeleteDossierDocument(dossier, dossier.Anonymized);
                    var anonymizedDocument = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Anonymized);
                    dossier.Documents.Add(anonymizedDocument);

                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.DocumentReUploaded,
                        MessageArguments = new[]
                        {
                            "DocumentType_Anonymized"
                        }
                    });

                    break;

                case ApplicationDossierStatus.InReview:

                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow uploading a review document
                    var review = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Review);
                    var userReviewerId = await FindReviewerAsync(input.ReviewerEmail, input.ReviewerFirstName, input.ReviewerLastName);
                    var dossierReview = new ApplicationDossierReview()
                    {
                        FirstName = input.ReviewerFirstName,
                        LastName = input.ReviewerLastName,
                        Email = input.ReviewerEmail,

                        ReviewerId = userReviewerId,

                        Dossier = dossier,
                        Review = review
                    };

                    dossier.Reviews.Add(dossierReview);

                    dossier.Journals.Add(new DossierJournal
                    {
                        PerformedById = currentUserId,
                        Message = JournalEntryKeys.DocumentUploaded,
                        MessageArguments = new[]
                        {
                            "DocumentType_Review"
                        }
                    });

                    // create a checkpoint here
                    await _context.SaveChangesAsync();

                    // check if there are 2 reviews available
                    // if yes - advance dossier status
                    if (dossier.Reviews.Count == 2)
                    {
                        // update dossier status
                        await UpdateStatusAsync(dossier.Id, ApplicationDossierStatus.Reviewed);
                    }

                    break;

                case ApplicationDossierStatus.Reviewed:
                    // nothing to do here ...
                    // awaiting operator's command
                    break;

                case ApplicationDossierStatus.AwaitingCorrections:
                    
                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow uploading/overriding of a redacted version document
                    if (dossier.Redacted != null)
                    {
                        await DeleteDossierDocument(dossier, dossier.Redacted);

                        dossier.Journals.Add(new DossierJournal
                        {
                            PerformedById = currentUserId,
                            Message = JournalEntryKeys.DocumentReUploaded,
                            MessageArguments = new[]
                            {
                                "DocumentType_Redacted"
                            }
                        });
                    }
                    else
                    {
                        dossier.Journals.Add(new DossierJournal
                        {
                            PerformedById = currentUserId,
                            Message = JournalEntryKeys.DocumentUploaded,
                            MessageArguments = new[]
                            {
                                "DocumentType_Redacted"
                            }
                        });
                    }

                    var redactedDocument = await SaveDossierDocumentAsync(input.Document, ApplicationDocumentType.Redacted);
                    dossier.Documents.Add(redactedDocument);
                    
                    break;

                case ApplicationDossierStatus.AcceptedWithCorrections:
                    // partial success! pat yourself on the back!
                    break;

                case ApplicationDossierStatus.Accepted:
                    // full success! congratulations!
                    break;

                case ApplicationDossierStatus.Rejected:
                    // aw, out of luck
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private async Task UpdateDossierPropertiesAsync(ApplicationDossier dossier, DossierEditViewModel input)
        {
            if (dossier.AssignedToId != input.AssigneeId)
            {
                await UpdateAssigneeAsync(dossier.Id, input.AssigneeId);
            }

            // NOTE: if need be add statements for other properties here...

        }

        private ClaimsPrincipal GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        private string GetCurrentUserId()
        {
            return GetCurrentUser()?.GetUserId();
        }

        private List<SelectListItem> GetReviewers()
        {
            var selectList = new List<SelectListItem>()
            {
                new(_localizationService["DossierEdit_Reviewer_Prompt"].Value, string.Empty)
            };

            var reviewers = _context.Users
                .Where(x => x.IsReviewer)
                .Select(x => new SelectListItem(x.Names, x.Id))
                .ToList();

            selectList.AddRange(reviewers);

            return selectList;

        }

        private List<SelectListItem> GetEditors(string assigneeId)
        {
            var editors = _context.GetAllByRole(SiteRole.Editor.ToString());
            var headEditors = _context.GetAllByRole(SiteRole.HeadEditor.ToString());

            var selectList = new List<SelectListItem>()
            {
                new(_localizationService["DossierEdit_AssignEditor_Prompt"].Value, string.Empty)
            };

            selectList.AddRange(editors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));
            selectList.AddRange(headEditors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));

            return selectList;
        }

        private async Task<string> FindReviewerAsync(string inputEmail, string inputFirstName, string inputLastName)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == inputEmail);
            if (user == null)
            {
                // second try by names
                user = _context.Users
                    .FirstOrDefault(x =>
                        EF.Functions.Like(x.FirstName, $"{inputFirstName}")&&
                        EF.Functions.Like(x.LastName, $"{inputLastName}")
                    );
            }

            if (user is null)
            {
                return null;
            }

            _context.Users.Attach(user);

            user.IsReviewer = true;

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}
