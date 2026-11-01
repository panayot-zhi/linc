using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.PageObjects
{
    public record SourceInput(
        string Title,
        IReadOnlyList<AuthorInput> Authors,
        int IssueId,
        int LanguageId,
        string PdfFilePath,
        int PdfPage,
        string? AuthorsNotes = null,
        bool IsSection = false,
        string? Doi = null,
        int? DossierId = null);

    /// <summary>
    /// Source (index entry) pages. Every method takes the page of the user who should act.
    /// </summary>
    public class SourcePages
    {
        private readonly Uri _baseUri;

        public SourcePages(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public string Url(string relativePath) => new Uri(_baseUri, relativePath).ToString();

        public string EditUrl(int id) => Url($"/source/edit/{id}");

        /// <summary>
        /// Public library list filtered by a word in the author names.
        /// </summary>
        public string IndexByAuthorUrl(string filter) => Url($"/source?filter={Uri.EscapeDataString(filter)}");

        public string CreateUrl(int? issueId = null, int? dossierId = null)
        {
            var query = new List<string>();
            if (issueId.HasValue) query.Add($"issueId={issueId}");
            if (dossierId.HasValue) query.Add($"dossierId={dossierId}");
            return Url("/source/create" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty));
        }

        /// <summary>
        /// Creates a source (editor and above) and returns its id.
        /// The page number becomes part of the stored PDF file name, so use a unique one
        /// when the source will be deleted, otherwise the PDF may be shared with another source.
        /// </summary>
        public async Task<int> CreateAsync(IPage page, SourceInput input)
        {
            await page.GotoAsync(CreateUrl(input.IssueId, input.DossierId));
            await FillAsync(page, input);

            await page.ClickAsync("main form button[type='submit']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var match = Regex.Match(new Uri(page.Url).AbsolutePath, @"/source/edit/(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                Assert.Fail($"Creating source '{input.Title}' did not redirect to its edit page, ended on {page.Url}. " +
                    await FormErrors.DescribeAsync(page));
            }

            return int.Parse(match.Groups[1].Value);
        }

        public static async Task FillAsync(IPage page, SourceInput input)
        {
            await page.SelectOptionAsync("select#LanguageId", input.LanguageId.ToString());
            await page.SelectOptionAsync("select#IssueId", input.IssueId.ToString());
            await page.FillAsync("input#Title", input.Title);

            if (input.IsSection)
            {
                await page.CheckAsync("input#IsSection");
            }

            if (input.Doi != null)
            {
                await page.FillAsync("input#DOI", input.Doi);
            }

            if (input.AuthorsNotes != null)
            {
                await page.FillAsync("textarea#AuthorsNotes", input.AuthorsNotes);
            }

            // sources created from a dossier come with its authors already filled in
            var authors = new AuthorsEditor(page);
            foreach (var author in input.Authors)
            {
                if (await authors.IndexOfAsync(author.Names) < 0)
                {
                    await authors.AddAsync(author);
                }
            }

            await page.FillAsync("input#StartingPdfPage", input.PdfPage.ToString());
            await page.FillAsync("input#LastPdfPage", input.PdfPage.ToString());
            await page.SetInputFilesAsync("input#PdfFile", input.PdfFilePath);
        }

        /// <summary>
        /// Deletes a source from its edit page (administrator only).
        /// </summary>
        public async Task DeleteAsync(IPage page, int id)
        {
            await page.GotoAsync(EditUrl(id));
            await page.ClickAsync("a#submit-delete-form");
            await page.ClickAsync("button.swal2-confirm");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// The author names shown for the sources in a list page (library, issue details, admin).
        /// </summary>
        public static Task<IReadOnlyList<string>> ListedAuthorNamesAsync(IPage page) =>
            page.Locator("a[href*='/source?filter='], a[href*='/source/index?filter=']").AllInnerTextsAsync();
    }
}
