using linc.Data;
using linc.Models.ViewModels;
using linc.Utility;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;

namespace linc.Services
{
    public class ContentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly LinkGenerator _linkGenerator;
        private readonly IMemoryCache _cache; 

        public ContentService(ApplicationDbContext dbContext, LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _dbContext = dbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public List<SuggestionsViewModel> GetSuggestions(int count = 5)
        {
            var result = new List<SuggestionsViewModel>();

            // for (var i = 1; i <= 5; i++)
            // {
            //     result.Add(new()
            //     {
            //         Content = "Брой №" + i,
            //         Href = "#"
            //     });
            // }
            //
            // result.Shuffle();

            return result;
        }
    }
}
