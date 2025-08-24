using linc.Contracts;
using linc.Data;
using linc.Models.ViewModels.Author;
using Microsoft.EntityFrameworkCore;

namespace linc.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationUserStore _applicationUserStore;

        public AuthorService(ApplicationDbContext context, IApplicationUserStore applicationUserStore)
        {
            _context = context;
            _applicationUserStore = applicationUserStore;
        }

        private async Task<ApplicationUser> FindApplicationUser(string userId, string firstName, string lastName)
        {
            ApplicationUser user;

            if (!string.IsNullOrEmpty(userId))
            {
                user = await _context.Users.FindAsync(userId);
            }
            else
            {
                user = await _applicationUserStore.FindUserByNamesAsync(firstName, lastName);
            }

            return user;
        }

        public async Task<ApplicationAuthor> GetAuthorAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(int languageId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<SourceAuthorViewModel>();
            }

            searchTerm = searchTerm.Trim().ToLower();
            var authors = await _context.Authors
                .Include(a => a.User)
                .Where(a => a.LanguageId == languageId)
                .Where(a => a.Names.ToLower().Contains(searchTerm))
                .GroupBy(a => a.Names)
                .Select(g => g.OrderByDescending(a => a.Email).First())
                .Take(10)
                .ToListAsync();

            return authors.Select(a => new SourceAuthorViewModel()
            {
                // Do not return ID
                // all records MUST be new
                // Id = a.Id,

                Names = a.Names,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,

                UserId = a.UserId,
                UserName = a.User?.UserName

            }).ToList();
        }

        public async Task<List<ApplicationAuthor>> CreateSourceAuthorsAsync(int languageId, List<SourceAuthorViewModel> authors)
        {
            var result = new List<ApplicationAuthor>();
            foreach (var authorViewModel in authors)
            {
                authorViewModel.FirstName = authorViewModel.FirstName.Trim();
                authorViewModel.LastName = authorViewModel.LastName.Trim();
                authorViewModel.Notes = authorViewModel.Notes?.Trim();

                var user = await FindApplicationUser(authorViewModel.UserId, authorViewModel.FirstName, authorViewModel.LastName);

                var author = new ApplicationAuthor
                {
                    FirstName = authorViewModel.FirstName,
                    LastName = authorViewModel.LastName,
                    Notes = authorViewModel.Notes,
                    Email = authorViewModel.Email,
                    LanguageId = languageId,
                    UserId = user?.Id
                };

                result.Add(author);
            }

            return result;
        }

        public async Task<List<ApplicationAuthor>> CreateDossierAuthorsAsync(int languageId, List<DossierAuthorViewModel> authors, int dossierId)
        {
            var result = new List<ApplicationAuthor>();
            foreach (var authorViewModel in authors)
            {
                authorViewModel.FirstName = authorViewModel.FirstName.Trim();
                authorViewModel.LastName = authorViewModel.LastName.Trim();
                authorViewModel.Email = authorViewModel.Email?.Trim();

                var user = await FindApplicationUser(authorViewModel.UserId, authorViewModel.FirstName, authorViewModel.LastName);

                var author = new ApplicationAuthor
                {
                    FirstName = authorViewModel.FirstName.Trim(),
                    LastName = authorViewModel.LastName.Trim(),
                    Email = authorViewModel.Email?.Trim(),
                    LanguageId = languageId,
                    UserId = user?.Id,
                    DossierId = dossierId
                };

                result.Add(author);
            }
            return result;
        }

        public async Task UpdateSourceAuthorsAsync(ApplicationSource source, List<SourceAuthorViewModel> newAuthors)
        {
            // Load existing authors for the source
            await _context.Entry(source).Collection(s => s.Authors).LoadAsync();
            var existingAuthors = source.Authors.ToList();

            // Remove authors not in new list
            var newAuthorIds = newAuthors.Where(a => a.Id.HasValue).Select(a => a.Id.Value).ToHashSet();
            var toRemove = existingAuthors.Where(ea => !newAuthorIds.Contains(ea.Id)).ToList();
            _context.Authors.RemoveRange(toRemove);

            // Update existing authors
            foreach (var existing in existingAuthors)
            {
                var updated = newAuthors.FirstOrDefault(a => a.Id == existing.Id);
                if (updated is null)
                {
                    // NOTE: Author is brand new
                    // not an existing one to be updated
                    continue;
                }

                var firstName = updated.FirstName.Trim();
                var lastName = updated.LastName.Trim();
                var notes = updated.Notes?.Trim();
                var email = updated.Email;

                var user = await FindApplicationUser(updated.UserId, updated.FirstName, updated.LastName);

                var changed = false;
                if (!string.Equals(existing.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
                {
                    existing.FirstName = firstName;
                    changed = true;
                }

                if (!string.Equals(existing.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    existing.LastName = lastName;
                    changed = true;
                }

                if (!string.Equals(existing.Notes, notes, StringComparison.OrdinalIgnoreCase))
                {
                    existing.Notes = notes;
                    changed = true;
                }

                if (!string.Equals(existing.Email, email, StringComparison.Ordinal))
                {
                    existing.Email = email;
                    changed = true;
                }

                if (existing.UserId != user?.Id)
                {
                    existing.UserId = user?.Id;
                    changed = true;
                }

                if (changed)
                {
                    _context.Authors.Update(existing);
                }
            }

            // Add new authors
            var newToAdd = newAuthors.Where(a => !a.Id.HasValue).ToList();
            var addedAuthors = await CreateSourceAuthorsAsync(source.LanguageId, newToAdd);
            foreach (var author in addedAuthors)
            {
                source.Authors.Add(author);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDossierAuthorsAsync(ApplicationDossier dossier, List<DossierAuthorViewModel> newAuthors)
        {
            await _context.Entry(dossier).Collection(d => d.Authors).LoadAsync();
            var existingAuthors = dossier.Authors.ToList();

            // Remove authors not in new list
            var newAuthorIds = newAuthors.Where(a => a.Id.HasValue).Select(a => a.Id.Value).ToHashSet();
            var toRemove = existingAuthors.Where(ea => !newAuthorIds.Contains(ea.Id)).ToList();

            _context.Authors.RemoveRange(toRemove);

            // Update existing authors
            foreach (var existing in existingAuthors)
            {
                var updated = newAuthors.FirstOrDefault(a => a.Id == existing.Id);
                if (updated is null)
                {
                    continue;
                }

                var firstName = updated.FirstName.Trim();
                var lastName = updated.LastName.Trim();
                var email = updated.Email?.Trim();
                var changed = false;

                if (!string.Equals(existing.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
                {
                    existing.FirstName = firstName;
                    changed = true;
                }

                if (!string.Equals(existing.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    existing.LastName = lastName;
                    changed = true;
                }

                if (!string.Equals(existing.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    existing.Email = email;
                    changed = true;
                }

                if (changed)
                {
                    var user = await FindApplicationUser(updated.UserId, updated.FirstName, updated.LastName);
                    existing.UserId = user?.Id;

                    _context.Authors.Update(existing);
                }
            }

            // Add new authors
            var newToAdd = newAuthors.Where(a => !a.Id.HasValue).ToList();
            foreach (var authorViewModel in newToAdd)
            {
                var user = await FindApplicationUser(authorViewModel.UserId, authorViewModel.FirstName, authorViewModel.LastName);

                var author = new ApplicationAuthor
                {
                    FirstName = authorViewModel.FirstName.Trim(),
                    LastName = authorViewModel.LastName.Trim(),
                    Email = authorViewModel.Email?.Trim(),
                    LanguageId = dossier.LanguageId,
                    UserId = user?.Id,
                    DossierId = dossier.Id
                };

                dossier.Authors.Add(author);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuthorsUserAsync(ApplicationUser user)
        {
            var authors = _context.Authors
                .Where(x => x.UserId == null)
                .Where(x => x.Email == user.Email)
                .ToList();

            if (!authors.Any())
            {
                return;
            }

            foreach (var author in authors)
            {
                _context.Authors.Attach(author);
                author.UserId = user.Id;
            }

            await _context.SaveChangesAsync();
        }

    }
}
