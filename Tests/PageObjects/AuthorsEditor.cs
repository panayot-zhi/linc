using Microsoft.Playwright;

namespace linc.E2ETests.PageObjects
{
    public record AuthorInput(string FirstName, string LastName, string? Email = null, string? Notes = null)
    {
        /// <summary>
        /// The names as the app stores them: first name capitalised, last name upper case
        /// (ApplicationDbContext normalises authors on save).
        /// </summary>
        public string Names => $"{Capitalize(FirstName.Trim())} {LastName.Trim().ToUpperInvariant()}";

        private static string Capitalize(string value) =>
            value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
    }

    public record AuthorRow(int? Id, string FirstName, string LastName, string? Email, string? Notes, bool ReadOnly)
    {
        public string Names => $"{FirstName} {LastName}";
    }

    /// <summary>
    /// The author cards shared by the dossier forms (_CreateAuthors, _EditAuthors)
    /// and the source forms (_Authors). Sources keep the email in a hidden field.
    /// </summary>
    public class AuthorsEditor
    {
        private readonly IPage _page;

        public AuthorsEditor(IPage page)
        {
            _page = page;
        }

        public ILocator Cards => _page.Locator("#authors-list .author-card");

        public Task<int> CountAsync() => Cards.CountAsync();

        /// <summary>
        /// The dossier edit form keeps the authors in a collapsed panel; open it so the cards can be used.
        /// </summary>
        public async Task ExpandAsync()
        {
            var panel = _page.Locator("#authors-collapse");
            if (await panel.CountAsync() == 0 || await panel.IsVisibleAsync())
            {
                return;
            }

            await _page.ClickAsync("#authors-collapse-toggle");
            await _page.Locator("#authors-collapse.show").WaitForAsync();
        }

        /// <summary>
        /// Fills the first card when it is still empty (the create forms start with one),
        /// otherwise adds a new card. Returns the card index.
        /// </summary>
        public async Task<int> AddAsync(AuthorInput author)
        {
            await ExpandAsync();
            var count = await CountAsync();
            var reuseFirst = count == 1 &&
                             string.IsNullOrEmpty(await Cards.Nth(0).Locator(".author-first-name").InputValueAsync()) &&
                             string.IsNullOrEmpty(await Cards.Nth(0).Locator(".author-last-name").InputValueAsync());

            if (!reuseFirst)
            {
                await _page.ClickAsync("#add-author");
                count++;
            }

            var index = reuseFirst ? 0 : count - 1;
            await FillAsync(index, author);
            return index;
        }

        public async Task AddRangeAsync(IEnumerable<AuthorInput> authors)
        {
            foreach (var author in authors)
            {
                await AddAsync(author);
            }
        }

        public async Task FillAsync(int index, AuthorInput author)
        {
            await ExpandAsync();
            var card = Cards.Nth(index);
            await card.Locator(".author-first-name").FillAsync(author.FirstName);
            await card.Locator(".author-last-name").FillAsync(author.LastName);

            var email = card.Locator(".author-email");
            if (author.Email != null && await IsEditableInputAsync(email))
            {
                await email.FillAsync(author.Email);
            }

            var notes = card.Locator(".author-notes");
            if (author.Notes != null && await notes.CountAsync() > 0)
            {
                await notes.FillAsync(author.Notes);
            }
        }

        public async Task RemoveAsync(int index)
        {
            await ExpandAsync();
            await Cards.Nth(index).Locator(".remove-author").ClickAsync();
        }

        public async Task<int> IndexOfAsync(string names)
        {
            var rows = await ReadAsync();
            for (var i = 0; i < rows.Count; i++)
            {
                if (string.Equals(rows[i].Names, names, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Types into the card's search box and picks the suggestion containing <paramref name="suggestionText"/>.
        /// The search needs at least three characters and is filtered by the form's language.
        /// </summary>
        public async Task PickSearchResultAsync(int index, string query, string suggestionText)
        {
            await ExpandAsync();
            var card = Cards.Nth(index);
            var search = card.Locator(".search-authors");
            await search.FillAsync(string.Empty);
            await search.PressSequentiallyAsync(query, new LocatorPressSequentiallyOptions { Delay = 30 });

            var suggestion = card.Locator(".choose-author", new LocatorLocatorOptions { HasText = suggestionText });
            await suggestion.First.ClickAsync();
        }

        /// <summary>
        /// Searches and returns the suggestion labels shown, waiting a moment for the debounced request.
        /// </summary>
        public async Task<IReadOnlyList<string>> SearchAsync(int index, string query)
        {
            await ExpandAsync();
            var card = Cards.Nth(index);
            var search = card.Locator(".search-authors");
            await search.FillAsync(string.Empty);

            var response = _page.WaitForResponseAsync(r => r.Url.Contains("/author/search"));
            await search.PressSequentiallyAsync(query, new LocatorPressSequentiallyOptions { Delay = 30 });
            await response;

            // results are rendered right after the response
            await _page.WaitForTimeoutAsync(300);
            return await card.Locator(".choose-author").AllInnerTextsAsync();
        }

        public async Task<IReadOnlyList<AuthorRow>> ReadAsync()
        {
            var rows = new List<AuthorRow>();
            var count = await CountAsync();

            for (var i = 0; i < count; i++)
            {
                var card = Cards.Nth(i);
                var idInput = card.Locator(".author-id");
                var idValue = await idInput.CountAsync() > 0 ? await idInput.InputValueAsync() : null;

                var firstName = card.Locator(".author-first-name");
                var notes = card.Locator(".author-notes");
                var email = card.Locator(".author-email");

                rows.Add(new AuthorRow(
                    Id: int.TryParse(idValue, out var id) ? id : null,
                    FirstName: await firstName.InputValueAsync(),
                    LastName: await card.Locator(".author-last-name").InputValueAsync(),
                    Email: await email.CountAsync() > 0 ? await email.InputValueAsync() : null,
                    Notes: await notes.CountAsync() > 0 ? await notes.InputValueAsync() : null,
                    ReadOnly: await firstName.GetAttributeAsync("readonly") != null));
            }

            return rows;
        }

        private static async Task<bool> IsEditableInputAsync(ILocator input)
        {
            if (await input.CountAsync() == 0)
            {
                return false;
            }

            var type = await input.GetAttributeAsync("type");
            return !string.Equals(type, "hidden", StringComparison.OrdinalIgnoreCase) && await input.IsEditableAsync();
        }
    }
}
