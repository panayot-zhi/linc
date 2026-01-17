using System.Net.Mime;
using System.Security.Claims;
using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Dossier;
using linc.Models.ViewModels.Emails;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace linc.Services
{
    public class DossierService : IDossierService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly ISiteEmailSender _emailSender;
        private readonly IApplicationUserStore _applicationUserStore;
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
            public const string DocumentDeleted = $"{Prefix}_{nameof(DocumentDeleted)}";
            public const string ClearedAssignment = $"{Prefix}_{nameof(ClearedAssignment)}";
        }

        public DossierService(ApplicationDbContext context, 
            IOptions<ApplicationConfig> configOptions,
            IApplicationUserStore applicationUserStore,
            IHttpContextAccessor httpContextAccessor, 
            ILocalizationService localizationService,
            ISiteEmailSender emailSender,
            ILogger<DossierService> logger, 
            LinkGenerator linkGenerator)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _linkGenerator = linkGenerator;
            _applicationUserStore = applicationUserStore;
            _config = configOptions.Value;
            _emailSender = emailSender;
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
                sortPropertyName = nameof(ApplicationDossier.LastUpdated);
                sortOrder = SiteSortOrder.Desc;
            }

            var dossiersDbSet = _context.Dossiers;
            var query = dossiersDbSet
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.Documents)
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

                Title = dossier.Title?.Trim(),
                FirstName = dossier.FirstName?.Trim(),
                LastName = dossier.LastName?.Trim(),
                Email = dossier.Email?.Trim(),

                Status = dossier.Status,
                SuperReviewed = dossier.SuperReviewed,

                AssigneeId = dossier.AssignedToId,
                AssigneeNames = dossier.AssignedTo != null ?
                    dossier.AssignedTo.Names :
                    string.Empty,

                Editors = GetEditors(dossier.AssignedToId),
                Reviewers = GetReviewers(),

                CanAttachAgreement = CanAttachAgreement(dossier),
                CanDeleteAgreement = CanDeleteAgreement(dossier)

            };

            return viewModel;
        }

        private static bool CanAttachAgreement(ApplicationDossier dossier)
        {
            // if we haven't got an agreement yet
            return dossier.Agreement is null &&
                   // and if the status of the dossier is not Accepted or AcceptedWithCorrections
                   // because in this status we send the declaration to the user for signing, so we await for it
                   dossier.Status is not ApplicationDossierStatus.Accepted or 
                       ApplicationDossierStatus.AcceptedWithCorrections;
        }

        private static bool CanDeleteAgreement(ApplicationDossier dossier)
        {
            // if we have an agreement
            return dossier.Agreement is not null &&
                   // and if the status of the dossier is not Accepted or AcceptedWithCorrections
                   // because in this status we send the declaration to the user for signing
                   // and a signed declaration from a user cannot be deleted
                   dossier.Status is not ApplicationDossierStatus.Accepted or
                       ApplicationDossierStatus.AcceptedWithCorrections;
        }

        public async Task<ApplicationDossier> GetDossierAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Documents);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            return dossier;
        }

        public async Task<int> CreateDossierAsync(DossierCreateViewModel input)
        {
            var currentUserId = GetCurrentUserId();
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var entry = new ApplicationDossier
            {
                Title = input.Title?.Trim(),
                FirstName = input.FirstName?.Trim(),
                LastName = input.LastName?.Trim(),
                Email = input.Email?.Trim(),

                Status = ApplicationDossierStatus.New,

                CreatedById = currentUserId
            };

            var entityEntry = await _context.Dossiers.AddAsync(entry);

            await _context.SaveChangesAsync();

            var original = await SaveDossierDocumentAsync(input.OriginalFile, entry.Id, ApplicationDocumentType.Original);

            entry.Documents.Add(original);

            entry.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.Created
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
        }

        private async Task<ApplicationDocument> SaveDossierDocumentAsync(IFormFile inputFile, int dossierId, ApplicationDocumentType type)
        {
            if (inputFile == null)
            {
                return null;
            }

            var fileExtension = inputFile.Extension();
            var fileName = Guid.NewGuid().ToString();

            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.DossiersFolderName, dossierId.ToString());
            var filePath = Path.Combine(rootFolderPath, $"{fileName}.{fileExtension}");

            Directory.CreateDirectory(rootFolderPath);

            var relativePath = Path.Combine(SiteConstant.DossiersFolderName, dossierId.ToString(), $"{fileName}.{fileExtension}");

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

            if (status is ApplicationDossierStatus.Accepted or ApplicationDossierStatus.AcceptedWithCorrections)
            {
                // check for agreement document
                var agreement = _context.Dossiers
                    .Include(x => x.Documents
                        .Where(document => document.DocumentType == ApplicationDocumentType.Agreement))
                    .FirstOrDefault(x => x.Id == dossier.Id)?.Agreement;

                if (agreement == null)
                {
                    // send publication agreement link
                    var emailDescriptor = new SiteEmailDescriptor<Agreement>()
                    {
                        Emails = new List<string>() { dossier.Email },
                        Subject = _localizationService["Email_Agreement_Subject"].Value,
                        ViewModel = new Agreement()
                        {
                            Names = dossier.Names,
                            DossierStatus = EnumHelper<ApplicationDossierStatus>.GetDisplayName(status),
                            AgreementLink = new LinkViewModel()
                            {
                                Text = _localizationService["Details_Label"].Value,
                                Url = _linkGenerator.GetUriByAction(
                                    _httpContextAccessor.HttpContext!,
                                    "Agreement",
                                    "Dossier",
                                    new { id = dossier.Id })
                            }
                        }
                    };

                    await _emailSender.SendEmailAsync(emailDescriptor);
                }
            }
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
                    var document = await SaveDossierDocumentAsync(input.Document, dossier.Id, ApplicationDocumentType.Anonymized);
                    
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
                    var anonymizedDocument = await SaveDossierDocumentAsync(input.Document, dossier.Id, ApplicationDocumentType.Anonymized);
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
                case ApplicationDossierStatus.InSuperReview:

                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow uploading a (super) review document
                    var documentType = dossier.Status == ApplicationDossierStatus.InReview
                        ? ApplicationDocumentType.Review
                        : ApplicationDocumentType.SuperReview;

                    var review = await SaveDossierDocumentAsync(input.Document, dossier.Id, documentType);
                    var userReviewerId = await FindReviewerAsync(input.ReviewerEmail, input.ReviewerFirstName, input.ReviewerLastName);
                    var dossierReview = new ApplicationDossierReview()
                    {
                        FirstName = input.ReviewerFirstName?.Trim(),
                        LastName = input.ReviewerLastName?.Trim(),
                        Email = input.ReviewerEmail?.Trim(),

                        ReviewerId = userReviewerId,

                        Dossier = dossier,
                        Review = review
                    };

                    dossier.Reviews.Add(dossierReview);

                    dossier.SuperReviewed = dossier.Status == ApplicationDossierStatus.InSuperReview;

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

                    // check if there are 2 or more reviews available
                    // if yes - advance dossier status
                    if (dossier.Reviews.Count >= 2)
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

                    var redactedDocument = await SaveDossierDocumentAsync(input.Document, dossier.Id, ApplicationDocumentType.Redacted);
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
                case ApplicationDossierStatus.Published:
                    // end of the line
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task UpdateAuthorAsync(ApplicationUser user)
        {
            var dossiers = _context.Dossiers
                .Where(x => x.AuthorId == null)
                .Where(x => x.Email == user.Email)
                .ToList();

            if (!dossiers.Any())
            {
                return;
            }

            if (!user.IsAuthor)
            {
                _context.Users.Attach(user);
                user.IsAuthor = true;
            }

            foreach (var dossier in dossiers)
            {
                _context.Dossiers.Attach(dossier);
                dossier.AuthorId = user.Id;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewerAsync(ApplicationUser user)
        {
            var dossierReviews = _context.DossierReviews
                .Where(x => x.ReviewerId == null)
                .Where(x => x.Email == user.Email)
                .ToList();

            if (!dossierReviews.Any())
            {
                return;
            }

            if (!user.IsAuthor)
            {
                _context.Users.Attach(user);
                user.IsAuthor = true;
            }

            foreach (var dossierReview in dossierReviews)
            {
                _context.DossierReviews.Attach(dossierReview);
                dossierReview.ReviewerId = user.Id;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveAgreementAsync(ApplicationDossier dossier, byte[] inputFile)
        {
            const string fileName = "publication_agreement.pdf";
            var rootFolderPath = Path.Combine(_config.RepositoryPath, SiteConstant.DossiersFolderName, dossier.Id.ToString());
            var filePath = Path.Combine(rootFolderPath, fileName);
            var currentUser = GetCurrentUser();
            var currentUserId = currentUser.GetUserId();

            Directory.CreateDirectory(rootFolderPath);

            var relativePath = Path.Combine(SiteConstant.DossiersFolderName, dossier.Id.ToString(), fileName);

            await File.WriteAllBytesAsync(filePath, inputFile);

            var entry = new ApplicationDocument()
            {
                DocumentType = ApplicationDocumentType.Agreement,
                Extension = "pdf",
                FileName = fileName,
                MimeType = MediaTypeNames.Application.Pdf,
                OriginalFileName = fileName,
                RelativePath = relativePath
            };

            dossier.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.DocumentUploaded,
                MessageArguments = new[]
                {
                    "DocumentType_Agreement"
                }
            });

            await _context.Documents.AddAsync(entry);

            _context.Dossiers.Attach(dossier);

            dossier.Documents.Add(entry);
            dossier.AuthorId = currentUserId;

            await _context.SaveChangesAsync();

            // send publication agreement document
            // - cc to editor (if any assigned)
            // - to the dossier author email

            var ccEMails = new List<string>();
            if (dossier.AssignedToId != null)
            {
                var editor = _context.Users
                    .First(x => x.Id == dossier.AssignedToId);
                ccEMails.Add(editor.Email);
            }

            var emailDescriptor = new SiteEmailDescriptor<AgreementReceived>()
            {
                CcEmails = ccEMails,
                Emails = new List<string>() { dossier.Email },
                Subject = _localizationService["Email_AgreementReceived_Subject"].Value,
                ViewModel = new AgreementReceived()
                {
                    Names = dossier.Names,
                    AgreementLink = new LinkViewModel()
                    {
                        Text = _localizationService["Details_Label"].Value,
                        Url = _linkGenerator.GetUriByAction(
                            _httpContextAccessor.HttpContext!,
                            "Agreement",
                            "Dossier",
                            new { id = dossier.Id })
                    }
                }
            };

            await _emailSender.SendEmailAsync(emailDescriptor);
        }

        public async Task DeleteAgreementAsync(ApplicationDossier dossier)
        {
            await DeleteDossierDocument(dossier, dossier.Agreement);

            var currentUserId = GetCurrentUserId();
            dossier.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.DocumentDeleted,
                MessageArguments = new[]
                {
                    "DocumentType_Agreement"
                }
            });

            await _context.SaveChangesAsync();
        }

        private async Task UpdateDossierPropertiesAsync(ApplicationDossier dossier, DossierEditViewModel input)
        {
            if (dossier.AssignedToId != input.AssigneeId)
            {
                await UpdateAssigneeAsync(dossier.Id, input.AssigneeId);
            }

            if (input.AgreementDocument is not null)
            {
                // allow uploading of an agreement document
                var document = await SaveDossierDocumentAsync(input.AgreementDocument, dossier.Id, ApplicationDocumentType.Agreement);
                
                dossier.Documents.Add(document);
                
                var currentUserId = GetCurrentUserId();
                dossier.Journals.Add(new DossierJournal
                {
                    PerformedById = currentUserId,
                    Message = JournalEntryKeys.DocumentUploaded,
                    MessageArguments = new[]
                    {
                        "DocumentType_Agreement"
                    }
                });
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
            var editors = _context.GetAllByRole(HelperFunctions.ToScreamingSnakeCase(SiteRole.Editor.ToString()));
            var headEditors = _context.GetAllByRole(HelperFunctions.ToScreamingSnakeCase(SiteRole.HeadEditor.ToString()));

            var selectList = new List<SelectListItem>()
            {
                new(_localizationService["DossierEdit_AssignEditor_Prompt"].Value, string.Empty)
            };

            selectList.AddRange(headEditors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));
            selectList.AddRange(editors.Select(x => new SelectListItem(text: x.Names, x.Id, x.Id == assigneeId)));

            return selectList;
        }

        private async Task<string> FindReviewerAsync(string inputEmail, string inputFirstName, string inputLastName)
        {
            var user = await _applicationUserStore.FindUserByEmailAsync(inputEmail);
            if (user == null)
            {
                // second try by names
                user = await _applicationUserStore.FindUserByNamesAsync(inputFirstName, inputLastName);
            }

            if (user is null)
            {
                // no luck
                return null;
            }

            _context.Users.Attach(user);

            user.IsReviewer = true;

            await _context.SaveChangesAsync();

            return user.Id;
        }
    }
}
