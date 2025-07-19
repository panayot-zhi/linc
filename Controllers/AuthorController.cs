using linc.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace linc.Controllers
{
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;

        public AuthorController(ILocalizationService localizationService, IAuthorService authorService)
            : base(localizationService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int? languageId, string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest($"Search term '{nameof(q)}' is required.");
            }

            if (q.Length < 3)
            {
                return BadRequest($"Search term '{nameof(q)}' must be at least 3 characters long.");
            }

            languageId ??= LocalizationService.GetCurrentLanguageId();

            var authors = await _authorService.SearchAuthorsAsync(languageId.Value, q);

            return Json(authors);
        }
    }
}
