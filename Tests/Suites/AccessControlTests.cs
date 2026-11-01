using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Who may open which page. Anonymous users are sent to the login page,
    /// signed in users without the role to the 403 error page.
    /// Only reads data: every check uses the seeded read-only rows.
    /// </summary>
    [NonParallelizable]
    public class AccessControlTests : TestBase
    {
        private const string LoginPath = "/identity/account/login";
        private const string AccessDeniedPath = "/home/error/403";

        private RegisteredUser _plainUser = null!;

        [OneTimeSetUp]
        public async Task RegisterPlainUser()
        {
            _plainUser = await RegisterUserAsync("Access", "Plain");
        }

        private static async Task<string> OpenAsync(IPage page, string url)
        {
            await page.GotoAsync(url);
            return CurrentPath(page);
        }

        // ---------- issues ----------

        /// <summary>
        /// An editor opens the "create issue" page and must get the "access denied" (403) page.
        /// Only head editors and administrators may create issues.
        /// </summary>
        [Test]
        public async Task CreateIssue_Editor_IsForbidden()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/issue/create")), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// A head editor opens the "create issue" page and must see the form (the issue number field).
        /// </summary>
        [Test]
        public async Task CreateIssue_HeadEditor_SeesForm()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            Assert.That(await OpenAsync(page, Url("/issue/create")), Is.EqualTo("/issue/create"));
            await Assertions.Expect(page.Locator("input#IssueNumber")).ToBeVisibleAsync();
        }

        /// <summary>
        /// A visitor who is not logged in opens the seeded hidden issue and must get "not found" (404).
        /// Hidden issues are drafts and must not leak to the public.
        /// </summary>
        [Test]
        public async Task HiddenIssue_Anonymous_IsNotFound()
        {
            var page = await OpenAnonymousAsync();
            var response = await page.GotoAsync(Url($"/issue/details/{SeedData.IssueId}"));
            Assert.That(response?.Status, Is.EqualTo(404));
        }

        /// <summary>
        /// An editor opens the same hidden issue and must see it, so editors can prepare it before release.
        /// </summary>
        [Test]
        public async Task HiddenIssue_Editor_IsVisible()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            var response = await page.GotoAsync(Url($"/issue/details/{SeedData.IssueId}"));
            Assert.That(response?.Status, Is.EqualTo(200));
        }

        // ---------- dossiers ----------

        /// <summary>
        /// A visitor who is not logged in opens the dossier list and must be sent to the login page.
        /// </summary>
        [Test]
        public async Task DossierList_Anonymous_RedirectsToLogin()
        {
            var page = await OpenAnonymousAsync();
            Assert.That(await OpenAsync(page, Url("/dossier")), Is.EqualTo(LoginPath));
        }

        /// <summary>
        /// A logged in user with no role (an ordinary reader) opens the dossier list and must get the 403 page.
        /// Dossiers hold unpublished articles, only editors may see them.
        /// </summary>
        [Test]
        public async Task DossierList_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url("/dossier")), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// An editor opens the dossier list and must stay on it.
        /// </summary>
        [Test]
        public async Task DossierList_Editor_IsAllowed()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/dossier")), Is.EqualTo("/dossier"));
        }

        /// <summary>
        /// An editor opens the "create dossier" page and must get the 403 page.
        /// Only head editors and administrators may create dossiers.
        /// </summary>
        [Test]
        public async Task CreateDossier_Editor_IsForbidden()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/dossier/create")), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// A head editor opens the "create dossier" page and must see the form (the title field).
        /// </summary>
        [Test]
        public async Task CreateDossier_HeadEditor_SeesForm()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            Assert.That(await OpenAsync(page, Url("/dossier/create")), Is.EqualTo("/dossier/create"));
            await Assertions.Expect(page.Locator("input#Title")).ToBeVisibleAsync();
        }

        /// <summary>
        /// An ordinary logged in user opens the seeded dossier and must get the 403 page.
        /// </summary>
        [Test]
        public async Task SeededDossierDetails_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url($"/dossier/details/{SeedData.DossierId}")),
                Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// The test editor is assigned to the seeded dossier, so they must be able to open its edit page.
        /// </summary>
        [Test]
        public async Task SeededDossierEdit_AssignedEditor_IsAllowed()
        {
            // the seeded dossier is assigned to test-editor
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url($"/dossier/edit/{SeedData.DossierId}")),
                Is.EqualTo($"/dossier/edit/{SeedData.DossierId}"));
        }

        // ---------- dossier documents ----------

        /// <summary>
        /// A visitor who is not logged in follows the download link of the seeded dossier's Word document
        /// and must be sent to the login page instead of getting the file.
        /// </summary>
        [Test]
        public async Task DossierDocument_Anonymous_RedirectsToLogin()
        {
            var page = await OpenAnonymousAsync();
            var url = await SeededDocumentUrlAsync();
            Assert.That(await OpenDownloadAsync(page, url), Is.EqualTo(LoginPath));
        }

        /// <summary>
        /// An ordinary logged in user follows the same download link and must get the 403 page, not the file.
        /// </summary>
        [Test]
        public async Task DossierDocument_PlainUser_IsForbidden()
        {
            var url = await SeededDocumentUrlAsync();
            Assert.That(await OpenDownloadAsync(_plainUser.Page, url), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// An editor follows the same download link and must get the file.        
        /// </summary>
        [Test]
        public async Task DossierDocument_Editor_Downloads()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            var url = await SeededDocumentUrlAsync();

            var response = await page.APIRequest.GetAsync(url);
            Assert.That(response.Status, Is.EqualTo(200),
                "The seeded dossier document could not be downloaded.");
        }

        // ---------- sources ----------

        /// <summary>
        /// An ordinary logged in user opens the index admin list (/source/admin) and must get the 403 page.
        /// </summary>
        [Test]
        public async Task SourceAdmin_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url("/source/admin")), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// An editor opens the index admin list and must stay on it.
        /// </summary>
        [Test]
        public async Task SourceAdmin_Editor_IsAllowed()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/source/admin")), Is.EqualTo("/source/admin"));
        }

        /// <summary>
        /// An ordinary logged in user opens the edit page of the seeded index entry and must get the 403 page.
        /// </summary>
        [Test]
        public async Task SourceEdit_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url($"/source/edit/{SeedData.SourceId}")), Is.EqualTo(AccessDeniedPath));
        }

        /// <summary>
        /// An editor opens the edit page of the seeded index entry: they must see the form,
        /// but not the Delete button, because only administrators may delete entries.
        /// </summary>
        [Test]
        public async Task SourceEdit_Editor_IsAllowed_WithoutDelete()
        {
            // editors may open and save the form, only administrators may delete
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url($"/source/edit/{SeedData.SourceId}")),
                Is.EqualTo($"/source/edit/{SeedData.SourceId}"));

            await Assertions.Expect(page.Locator("input#Title")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("#submit-delete-form")).ToHaveCountAsync(0);
        }

        /// <summary>
        /// An administrator opens the same edit page and must see the Delete button.
        /// </summary>
        [Test]
        public async Task SourceEdit_Administrator_SeesDelete()
        {
            var page = await OpenAsAsync(TestRole.Administrator);
            await page.GotoAsync(Url($"/source/edit/{SeedData.SourceId}"));

            await Assertions.Expect(page.Locator("#submit-delete-form")).ToHaveCountAsync(1);
        }

        // ---------- helpers ----------

        private async Task<string> SeededDocumentUrlAsync()
        {
            // read the download link from the details page, as an editor would click it
            var editor = await OpenAsAsync(TestRole.Editor);
            await editor.GotoAsync(Url($"/dossier/details/{SeedData.DossierId}"));

            var link = editor.Locator($"a[href*='/dossier/{SeedData.DossierId}/document/{SeedData.DossierOriginalDocumentId}']");
            await link.First.WaitForAsync();

            var href = await link.First.GetAttributeAsync("href");
            return new Uri(BaseUri, href).ToString();
        }

        /// <summary>
        /// Follows a download link with the page's cookies and returns the path it ended on.
        /// </summary>
        private static async Task<string> OpenDownloadAsync(IPage page, string url)
        {
            var response = await page.APIRequest.GetAsync(url);
            return new Uri(response.Url).AbsolutePath.ToLowerInvariant();
        }
    }
}
