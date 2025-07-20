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

        private async Task<ApplicationUser> FindApplicationUser(SourceAuthorViewModel authorViewModel)
        {
            ApplicationUser user;

            if (!string.IsNullOrEmpty(authorViewModel.UserId))
            {
                user = await _context.Users.FindAsync(authorViewModel.UserId);
            }
            else
            {
                user = await _applicationUserStore.FindUserByNamesAsync(authorViewModel.FirstName, authorViewModel.LastName);
            }

            return user;
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
                .Select(g => g.OrderBy(a => a.Id).First())
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

        public async Task<List<ApplicationAuthor>> CreateAuthorsAsync(int languageId, List<SourceAuthorViewModel> authors)
        {
            var result = new List<ApplicationAuthor>();
            foreach (var authorViewModel in authors)
            {
                authorViewModel.FirstName = authorViewModel.FirstName.Trim();
                authorViewModel.LastName = authorViewModel.LastName.Trim();

                var user = await FindApplicationUser(authorViewModel);

                var author = new ApplicationAuthor
                {
                    FirstName = authorViewModel.FirstName,
                    LastName = authorViewModel.LastName,
                    // Email = authorViewModel.Email,
                    LanguageId = languageId,
                    UserId = user?.Id
                };

                result.Add(author);
            }

            return result;
        }

        public async Task UpdateAuthorsAsync(ApplicationSource source, List<SourceAuthorViewModel> newAuthors)
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
                // var email = updated.Email?.Trim();

                // var user = await FindApplicationUser(updated);

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

                // if (!string.Equals(existing.Email, email, StringComparison.Ordinal))
                // {
                //     existing.Email = email;
                //     changed = true;
                // }

                // if (existing.UserId != user?.Id)
                // {
                //     existing.UserId = user?.Id;
                //     changed = true;
                // }

                if (changed)
                {
                    _context.Authors.Update(existing);
                }
            }

            // Add new authors
            var newToAdd = newAuthors.Where(a => !a.Id.HasValue).ToList();
            var addedAuthors = await CreateAuthorsAsync(source.LanguageId, newToAdd);
            foreach (var author in addedAuthors)
            {
                source.Authors.Add(author);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuthorUserAsync(ApplicationUser user)
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
