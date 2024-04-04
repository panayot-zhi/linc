using linc.Contracts;
using linc.Data;
using linc.Models.ViewModels.Home;
using linc.Models.ViewModels.Source;
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

        public async Task<SourceIndexViewModel> GetSourcesPagedAsync(string filter, int languageId, int? year, int? issueId, int? pageIndex, int pageSize = 10)
        {
            if (!pageIndex.HasValue)
            {
                pageIndex = 1;
            }

            var sourcesDbSet = _context.Sources;
            var query = sourcesDbSet
                .Include(x => x.Issue)
                .Where(x => x.LanguageId == languageId)
                .AsQueryable();

            if (issueId.HasValue)
            {
                query = query.Where(x => x.IssueId == issueId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(x => x.DateCreated.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                if ("*".Equals(filter))
                {
                    // skip
                }
                else if (filter.Length > 1)
                {
                    // perform search by
                    query = query.Where(x =>
                        EF.Functions.Like(x.LastName, $"%{filter}%") ||
                        EF.Functions.Like(x.FirstName, $"%{filter}%") ||
                        EF.Functions.Like(x.AuthorNotes, $"%{filter}%") ||
                        EF.Functions.Like(x.Title, $"%{filter}%") ||
                        EF.Functions.Like(x.TitleNotes, $"%{filter}%")
                    );
                }
                else
                {
                    query = query.Where(x => x.LastName.StartsWith(filter));
                    query = query.OrderBy(x => x.LastName);
                }
            }
            else
            {
                query = query.OrderBy(x => x.DateCreated).
                    ThenByDescending(x => x.LastUpdated)
                    .ThenBy(x => x.StartingPage);
            }

            var count = await query.CountAsync();

            var sources = query
                .Skip((pageIndex.Value - 1) * pageSize)
                .Take(pageSize);

            return new SourceIndexViewModel(count, pageIndex.Value, pageSize)
            {
                Records = await sources.ToListAsync()
            };
        }

        public async Task<List<SourceCountByYears>> GetSourcesCountByYears()
        {
            return await _context.Sources
                .Include(x => x.Issue)
                .GroupBy(x => x.Issue.ReleaseYear)
                .Select(x => new SourceCountByYears()
                {
                    Year = x.Key,
                    Count = x.Count()

                }).ToListAsync();
        }

        public async Task<List<SourceCountByIssues>> GetSourcesCountByIssues()
        {
            return await _context.Sources
                .Where(source => source.IssueId.HasValue)
                .GroupBy(source => source.IssueId)
                .Select(group => new SourceCountByIssues
                {
                    IssueId = group.Key.Value,
                    IssueTitle = $"{group.First().Issue.IssueNumber}/{group.First().Issue.DateCreated.Year}",
                    Count = group.Count()
                }).ToListAsync();
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
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.FirstName.ToUpper() == inputFirstName.ToUpper() &&
                    x.LastName.ToUpper() == inputLastName.ToUpper()
                );

            return user?.Id;
        }
    }
}
