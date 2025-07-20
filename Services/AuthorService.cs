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

            if (user != null)
            {
                _context.Users.Attach(user);
                user.IsAuthor = true;
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(int languageId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<SourceAuthorViewModel>();
            }

            searchTerm = searchTerm.Trim();
            return await _context.Authors
                .Include(a => a.User)
                .Where(a => a.LanguageId == languageId)
                .Where(a => a.Names.ToLower().Contains(searchTerm.ToLower()))
                .Select(a => new SourceAuthorViewModel()
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,

                    UserId = a.UserId,
                    UserName = a.User.UserName,
                })
                .ToListAsync();
        }

        public async Task<List<ApplicationAuthor>> CreateAuthorsAsync(List<SourceAuthorViewModel> authors, int languageId)
        {
            var result = new List<ApplicationAuthor>();
            foreach (var authorViewModel in authors)
            {
                authorViewModel.FirstName = authorViewModel.FirstName.Trim();
                authorViewModel.LastName = authorViewModel.LastName.Trim();

                ApplicationUser user;

                if (!string.IsNullOrEmpty(authorViewModel.UserId))
                {
                    user = await _context.Users.FindAsync(authorViewModel.UserId);
                }
                else
                {
                    user = await _applicationUserStore.FindUserByNamesAsync(authorViewModel.FirstName, authorViewModel.LastName);
                }

                if (user != null)
                {
                    _context.Users.Attach(user);
                    user.IsAuthor = true;
                    await _context.SaveChangesAsync();
                }

                var appAuthor = new ApplicationAuthor
                {
                    FirstName = authorViewModel.FirstName,
                    LastName = authorViewModel.LastName,
                    Email = authorViewModel.Email,
                    UserId = user?.Id,
                    LanguageId = languageId
                };

                result.Add(appAuthor);
            }

            return result;
        }
    }
}
