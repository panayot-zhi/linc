using linc.Data;
using linc.Models.Enumerations;
using linc.Models.ViewModels.Dossier;
using linc.Utility;
using Microsoft.EntityFrameworkCore;
using linc.Models.ViewModels.Author;

namespace linc.Services
{
    public partial class DossierService
    {
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
                .Include(x => x.Authors)
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
                .Include(x => x.Authors)
                    .ThenInclude(a => a.User)
                .Include(x => x.Authors)
                    .ThenInclude(a => a.Agreement)
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
                Status = dossier.Status,
                AssignedToId = dossier.AssignedToId,
                AssignedTo = dossier.AssignedTo != null ?
                    dossier.AssignedTo.Names :
                    string.Empty,
                DateCreated = dossier.DateCreated,
                LastUpdated = dossier.LastUpdated,
                Journals = dossier.Journals.ToList(),
                Reviews = dossier.Reviews.ToList(),
                Documents = allDocuments.OrderBy(x => x.DocumentType).ToList(),
                Authors = dossier.Authors.Select(a => new DossierAuthorViewModel
                {
                    Id = a.Id,
                    DossierId = a.DossierId!.Value,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    UserId = a.UserId,
                    UserName = a.User?.UserName,
                    AgreementId = a.AgreementId,
                    Agreement = a.AgreementId.HasValue ?
                        a.Agreement :
                        null

                }).ToList()
            };
            return viewModel;
        }

        public async Task<DossierEditViewModel> GetDossierEditViewModelAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Authors)
                    .ThenInclude(a => a.User)
                .Include(x => x.Authors)
                    .ThenInclude(a => a.Agreement)
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
                Status = dossier.Status,
                SuperReviewed = dossier.SuperReviewed,
                AssigneeId = dossier.AssignedToId,
                AssigneeNames = dossier.AssignedTo != null ?
                    dossier.AssignedTo.Names :
                    string.Empty,
                Editors = GetEditors(dossier.AssignedToId),
                Reviewers = GetReviewers(),

                Authors = dossier.Authors.Select(a => new DossierAuthorViewModel
                {
                    Id = a.Id,
                    DossierId = a.DossierId!.Value,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    UserId = a.UserId,
                    UserName = a.User?.UserName,
                    AgreementId = a.AgreementId,
                    Agreement = a.AgreementId.HasValue ? 
                        a.Agreement : 
                        null
                }).ToList()
            };
            return viewModel;
        }

        public async Task<ApplicationDossier> GetDossierAsync(int id)
        {
            var query = _context.Dossiers
                .Include(x => x.Authors)
                    .ThenInclude(a => a.User)
                .Include(x => x.Documents);

            var dossier = await query.FirstOrDefaultAsync(x => x.Id == id);
            if (dossier is null)
            {
                return null;
            }

            return dossier;
        }
    }
}
