using linc.Contracts;
using linc.Data;
using linc.Models.ConfigModels;
using linc.Models.Enumerations;
using linc.Models.ViewModels;
using linc.Models.ViewModels.Dossier;
using linc.Models.ViewModels.Emails;
using linc.Utility;
using Microsoft.EntityFrameworkCore;

namespace linc.Services
{
    public partial class DossierService : IDossierService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly ISiteEmailSender _emailSender;
        private readonly IAuthorService _authorService;
        private readonly IDocumentService _documentService;
        private readonly IApplicationUserStore _applicationUserStore;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DossierService> _logger;
        private readonly ApplicationDbContext _context;

        public class JournalEntryKeys
        {
            private const string Prefix = "JournalDossier";

            public const string Created = $"{Prefix}_{nameof(Created)}";
            public const string AssignedTo = $"{Prefix}_{nameof(AssignedTo)}";
            public const string ReAssignedTo = $"{Prefix}_{nameof(ReAssignedTo)}";
            public const string StatusUpdated = $"{Prefix}_{nameof(StatusUpdated)}";
            //public const string PropertyUpdated = $"{Prefix}_{nameof(PropertyUpdated)}";

            public const string DocumentUploaded = $"{Prefix}_{nameof(DocumentUploaded)}";
            public const string DocumentUploadedFor = $"{Prefix}_{nameof(DocumentUploadedFor)}";
            public const string DocumentReUploaded = $"{Prefix}_{nameof(DocumentReUploaded)}";
            public const string DocumentDeleted = $"{Prefix}_{nameof(DocumentDeleted)}";
            public const string DocumentDeletedFor = $"{Prefix}_{nameof(DocumentDeletedFor)}";
            public const string ClearedAssignment = $"{Prefix}_{nameof(ClearedAssignment)}";
        }

        public DossierService(ApplicationDbContext context,
            IApplicationUserStore applicationUserStore,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IDocumentService documentService,
            IAuthorService authorService, 
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
            _emailSender = emailSender;
            _authorService = authorService;
            _documentService = documentService;
        }

        public async Task<int> CreateDossierAsync(DossierCreateViewModel input)
        {
            var currentUserId = GetCurrentUserId();
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var entry = new ApplicationDossier
            {
                Title = input.Title?.Trim(),
                Status = ApplicationDossierStatus.New,
                CreatedById = currentUserId,
                LanguageId = input.LanguageId
            };

            var authors = await _authorService.CreateDossierAuthorsAsync(entry.LanguageId, input.Authors, entry.Id);
            foreach (var author in authors)
            {
                entry.Authors.Add(author);
            }

            var entityEntry = await _context.Dossiers.AddAsync(entry);
            await _context.SaveChangesAsync();

            var originalDocumentDescriptor = await input.OriginalFile.ToDescriptorAsync(ApplicationDocumentType.Original);
            var originalDocument = await _documentService.SaveDossierDocumentAsync(entry.Id, originalDocumentDescriptor);

            entry.Documents.Add(originalDocument);
            entry.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.Created
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entityEntry.Entity.Id;
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
                await _context.Entry(dossier).Collection(s => s.Authors).LoadAsync();

                foreach (var author in dossier.Authors)
                {
                    if (author.AgreementId.HasValue)
                    {
                        _logger.LogInformation(
                            "Dossier {DossierId} author {AuthorId} already signed a publication agreement, skipping email sending...",
                            dossier.Id, author.Id);
                        continue;
                    }

                    // send publication agreement link
                    var emailDescriptor = new SiteEmailDescriptor<Agreement>()
                    {
                        Emails = new List<string>() { author.Email },
                        Subject = _localizationService["Email_Agreement_Subject"].Value,
                        ViewModel = new Agreement()
                        {
                            Names = author.Names,
                            DossierStatus = EnumHelper<ApplicationDossierStatus>.GetDisplayName(status),
                            AgreementLink = new LinkViewModel()
                            {
                                Text = _localizationService["Details_Label"].Value,
                                Url = _linkGenerator.GetUriByAction(
                                    _httpContextAccessor.HttpContext!,
                                    "Agreement",
                                    "Dossier",
                                    new
                                    {
                                        id = dossier.Id,
                                        aid = author.Id
                                    })
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
                .Include(x => x.Authors)
                .Include(x => x.Reviews)
                .Include(x => x.Documents)
                .FirstOrDefault(x => x.Id == input.Id);

            ArgumentNullException.ThrowIfNull(dossier);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Dossiers.Attach(dossier);

            // Update dossier properties
            if (dossier.AssignedToId != input.AssigneeId)
            {
                await UpdateAssigneeAsync(dossier.Id, input.AssigneeId);
            }

            // Update authors
            await _authorService.UpdateDossierAuthorsAsync(dossier, input.Authors);

            // Update author language and agreement document (if any)
            foreach (var author in dossier.Authors)
            {
                author.LanguageId = dossier.LanguageId;

                var authorViewModel = input.Authors.FirstOrDefault(x => x.Id == author.Id);
                if (authorViewModel?.AgreementDocument is not null)
                {
                    var agreementFileDescriptor = await authorViewModel.AgreementDocument
                        .ToDescriptorAsync(ApplicationDocumentType.Agreement);

                    await SaveAgreementAsync(dossier, author, agreementFileDescriptor);
                }
            }

            // NOTE: Perform clearly defined actions based on the current dossier status

            switch (dossier.Status)
            {
                case ApplicationDossierStatus.New:

                    if (input.Document == null)
                    {
                        break;
                    }

                    // allow uploading an anonymized document
                    var documentDescriptor = await input.Document.ToDescriptorAsync(ApplicationDocumentType.Anonymized);
                    var document = await _documentService.SaveDossierDocumentAsync(dossier.Id, documentDescriptor);

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
                    await _documentService.DeleteDocumentAsync(dossier.Anonymized.Id);
                    var anonymizedDocumentDescriptor = await input.Document.ToDescriptorAsync(ApplicationDocumentType.Anonymized);
                    var anonymizedDocument = await _documentService.SaveDossierDocumentAsync(dossier.Id, anonymizedDocumentDescriptor);
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

                    var reviewDocumentDescriptor = await input.Document.ToDescriptorAsync(documentType);
                    var reviewDocument = await _documentService.SaveDossierDocumentAsync(dossier.Id, reviewDocumentDescriptor);
                    var userReviewerId = await FindReviewerAsync(input.ReviewerEmail, input.ReviewerFirstName, input.ReviewerLastName);
                    var dossierReview = new ApplicationDossierReview()
                    {
                        FirstName = input.ReviewerFirstName?.Trim(),
                        LastName = input.ReviewerLastName?.Trim(),
                        Email = input.ReviewerEmail?.Trim(),

                        ReviewerId = userReviewerId,

                        Dossier = dossier,
                        Review = reviewDocument
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
                        await _documentService.DeleteDocumentAsync(dossier.Redacted.Id);

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

                    var redactedDocumentDescriptor = await input.Document.ToDescriptorAsync(ApplicationDocumentType.Redacted);
                    var redactedDocument = await _documentService.SaveDossierDocumentAsync(dossier.Id, redactedDocumentDescriptor);
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

        public async Task SaveAgreementAsync(ApplicationDossier dossier, ApplicationAuthor author, ApplicationDocumentDescriptor documentDescriptor)
        {
            var entry = await _documentService.SaveDossierDocumentAsync(dossier.Id, documentDescriptor);

            dossier.Journals.Add(new DossierJournal
            {
                PerformedById = GetCurrentUserId(),
                Message = JournalEntryKeys.DocumentUploadedFor,
                MessageArguments = new[]
                {
                    "DocumentType_Agreement",
                    author.Names
                }
            });

            _context.Dossiers.Attach(dossier);
            _context.Authors.Attach(author);

            author.UserId = author.UserId;
            author.Agreement = entry;

            await _context.SaveChangesAsync();

            // send publication agreement document
            // - cc to editor (if any assigned)
            // - to the dossier author email

            var ccEmails = new List<string>();
            if (dossier.AssignedToId != null)
            {
                var editor = _context.Users
                    .First(x => x.Id == dossier.AssignedToId);
                ccEmails.Add(editor.Email);
            }

            // var language = SiteConstant.SupportedCultures.First(x =>
                // x.Key == dossier.LanguageId).Value;

            // todo: rewrite SiteEmailDescriptor and models to pass message keys and not the direct message
            // this way we could pass a language and it would apply to the whole model
            // uncomment language setting then

            var emailDescriptor = new SiteEmailDescriptor<AgreementReceived>()
            {
                CcEmails = ccEmails,
                Emails = new List<string>() { author.Email },
                Subject = _localizationService["Email_AgreementReceived_Subject"].Value,
                ViewModel = new AgreementReceived()
                {
                    // Language = language,
                    Names = author.Names,
                    AgreementLink = new LinkViewModel()
                    {
                        Text = _localizationService["Details_Label"].Value,
                        Url = _linkGenerator.GetUriByAction(
                            _httpContextAccessor.HttpContext!,
                            "Agreement",
                            "Dossier",
                            new
                            {
                                id = dossier.Id,
                                aid = author.Id
                            })
                    }
                }
            };

            await _emailSender.SendEmailAsync(emailDescriptor);
        }

        public async Task DeleteAgreementAsync(ApplicationDossier dossier, ApplicationAuthor author)
        {
            ArgumentNullException.ThrowIfNull(author.AgreementId);

            await _documentService.DeleteDocumentAsync(author.AgreementId.Value);

            var currentUserId = GetCurrentUserId();
            dossier.Journals.Add(new DossierJournal
            {
                PerformedById = currentUserId,
                Message = JournalEntryKeys.DocumentDeletedFor,
                MessageArguments = new[]
                {
                    "DocumentType_Agreement",
                    author.Names
                }
            });

            _context.Attach(dossier);

            await _context.SaveChangesAsync();
        }
    }
}
