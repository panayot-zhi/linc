using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.PageObjects
{
    /// <summary>
    /// Dossier pages. Every method takes the page of the user who should act.
    /// </summary>
    public class DossierPages
    {
        private readonly Uri _baseUri;

        public DossierPages(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public string Url(string relativePath) => new Uri(_baseUri, relativePath).ToString();

        public string DetailsUrl(int id) => Url($"/dossier/details/{id}");

        public string EditUrl(int id) => Url($"/dossier/edit/{id}");

        public string AgreementUrl(int id, int authorId) => Url($"/dossier/{id}/agreement?aid={authorId}");

        /// <summary>
        /// Opens the create form and fills it, without submitting.
        /// </summary>
        public async Task<AuthorsEditor> FillCreateFormAsync(IPage page, string title, IEnumerable<AuthorInput> authors,
            string originalFilePath, int? languageId = null)
        {
            await page.GotoAsync(Url("/dossier/create"));

            // the language filters the author search, set it first
            if (languageId.HasValue)
            {
                await page.SelectOptionAsync("select#LanguageId", languageId.Value.ToString());
            }

            await page.FillAsync("input#Title", title);

            var authorsEditor = new AuthorsEditor(page);
            await authorsEditor.AddRangeAsync(authors);

            await page.SetInputFilesAsync("input#OriginalFile", originalFilePath);
            return authorsEditor;
        }

        public static async Task SubmitFormAsync(IPage page)
        {
            await page.ClickAsync("main form button[type='submit']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Creates a dossier (head editor and above) and returns its id.
        /// </summary>
        public async Task<int> CreateAsync(IPage page, string title, IEnumerable<AuthorInput> authors,
            string originalFilePath, int? languageId = null)
        {
            await FillCreateFormAsync(page, title, authors, originalFilePath, languageId);
            await SubmitFormAsync(page);

            if (new Uri(page.Url).AbsolutePath.ToLowerInvariant().Contains("/dossier/create"))
            {
                Assert.Fail($"Creating dossier '{title}' failed, the form was shown again. {await FormErrors.DescribeAsync(page)}");
            }

            return await FindIdByTitleAsync(page, title);
        }

        /// <summary>
        /// Finds a dossier on the first page of the list, newest first.
        /// </summary>
        public async Task<int> FindIdByTitleAsync(IPage page, string title)
        {
            await page.GotoAsync(Url("/dossier?sort=DateCreated&order=Desc"));

            var link = page.Locator("table a[href*='/dossier/details/']", new PageLocatorOptions { HasText = title });
            await link.First.WaitForAsync();

            var href = await link.First.GetAttributeAsync("href") ?? string.Empty;
            var match = Regex.Match(href, @"/dossier/details/(\d+)", RegexOptions.IgnoreCase);
            Assert.That(match.Success, Is.True, $"Could not read the id of dossier '{title}' from '{href}'.");

            return int.Parse(match.Groups[1].Value);
        }

        /// <summary>
        /// Reads the authors from the edit form, including their ids.
        /// </summary>
        public async Task<IReadOnlyList<AuthorRow>> GetAuthorsAsync(IPage page, int id)
        {
            await page.GotoAsync(EditUrl(id));
            return await new AuthorsEditor(page).ReadAsync();
        }

        public async Task<int> GetAuthorIdAsync(IPage page, int id, string names)
        {
            var authors = await GetAuthorsAsync(page, id);
            var author = authors.FirstOrDefault(x => string.Equals(x.Names, names, StringComparison.OrdinalIgnoreCase));

            Assert.That(author?.Id, Is.Not.Null, $"Author '{names}' was not found on dossier {id}.");
            return author!.Id!.Value;
        }

        /// <summary>
        /// Changes the status the same way the confirmed status links on the edit page do.
        /// Accepted and AcceptedWithCorrections send the publication agreement emails.
        /// </summary>
        public async Task SetStatusAsync(IPage page, int id, string status)
        {
            await page.GotoAsync(Url($"/dossier/update-status/{id}?status={status}"));
            Assert.That(new Uri(page.Url).AbsolutePath.ToLowerInvariant(), Does.Contain($"/dossier/details/{id}"),
                $"Setting dossier {id} to {status} did not return to its details page.");
        }

        /// <summary>
        /// The author lines in the details page authors card.
        /// </summary>
        public static ILocator AuthorLines(IPage page) => page.Locator("dl dd li").Filter(new LocatorFilterOptions
        {
            Has = page.Locator("a[href^='mailto:']")
        });

        /// <summary>
        /// The author line on the details page that contains <paramref name="names"/>.
        /// </summary>
        public static ILocator AuthorLine(IPage page, string names) =>
            AuthorLines(page).Filter(new LocatorFilterOptions { HasText = names });

        /// <summary>
        /// The agreement link the details page shows next to an author who signed.
        /// </summary>
        public static ILocator SignedAgreementLink(IPage page, int authorId) =>
            page.Locator($"dl dd li a[href*='/author/{authorId}']");

        /// <summary>
        /// Download links in the details page documents table: /dossier/{id}/document/{documentId}.
        /// </summary>
        public static ILocator DocumentLinks(IPage page, int dossierId) =>
            page.Locator($"a[href*='/dossier/{dossierId}/document/']");
    }
}
