using linc.Contracts;
using linc.Data;
using linc.Models.ViewModels.Author;
using linc.Models.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace linc.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SourceAuthorViewModel>> SearchAuthorsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<SourceAuthorViewModel>();
            }

            searchTerm = searchTerm.Trim();
            return await _context.Authors
                .Include(a => a.User)
                .Where(a => a.Names.ToLower().Contains(searchTerm.ToLower()))
                .Select(a => new SourceAuthorViewModel()
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,

                    UserId = a.UserId,
                    UserName = a.User.UserName,
                })
                .ToListAsync();
        }
    }
}
