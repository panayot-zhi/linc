using linc.Contracts;
using linc.Data;
using linc.Models.ViewModels.Issue;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace linc.Services
{
    public class SourceService : ISourceService
    {
        private readonly ApplicationDbContext _context;

        public SourceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ApplicationSource> GetSourceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationIssue>> GetIssuesAsync()
        {
            return await _context.Issues.ToListAsync();
        }

        public async Task<int> CreateSourceAsync(SourceCreateViewModel input)
        {
            var authorId = await FindUserByNamesAsync(input.FirstName, input.LastName);
            var entity = new ApplicationSource()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                AuthorNotes = input.AuthorNotes,
                StartingPage = input.StartingPage.Value,

                Title = input.Title,
                TitleNotes = input.TitleNotes,

                LanguageId = input.LanguageId,
                IssueId = input.IssueId,
                AuthorId = authorId
            };

            var entityEntry = await _context.Sources.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        private async Task<string> FindUserByNamesAsync(string inputFirstName, string inputLastName)
        {
            return (await _context.Users
                        .FirstOrDefaultAsync(x =>
                            x.FirstName.ToUpper() == inputFirstName.ToUpper() &&
                            x.LastName.ToUpper() == inputLastName.ToUpper()
                        ))?.Id;
        }
    }
}
