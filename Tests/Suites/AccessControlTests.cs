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

        [Test]
        public async Task CreateIssue_Editor_IsForbidden()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/issue/create")), Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task CreateIssue_HeadEditor_SeesForm()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            Assert.That(await OpenAsync(page, Url("/issue/create")), Is.EqualTo("/issue/create"));
            await Assertions.Expect(page.Locator("input#IssueNumber")).ToBeVisibleAsync();
        }

        [Test]
        public async Task HiddenIssue_Anonymous_IsNotFound()
        {
            var page = await OpenAnonymousAsync();
            var response = await page.GotoAsync(Url($"/issue/details/{SeedData.IssueId}"));
            Assert.That(response?.Status, Is.EqualTo(404));
        }

        [Test]
        public async Task HiddenIssue_Editor_IsVisible()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            var response = await page.GotoAsync(Url($"/issue/details/{SeedData.IssueId}"));
            Assert.That(response?.Status, Is.EqualTo(200));
        }

        // ---------- dossiers ----------

        [Test]
        public async Task DossierList_Anonymous_RedirectsToLogin()
        {
            var page = await OpenAnonymousAsync();
            Assert.That(await OpenAsync(page, Url("/dossier")), Is.EqualTo(LoginPath));
        }

        [Test]
        public async Task DossierList_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url("/dossier")), Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task DossierList_Editor_IsAllowed()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/dossier")), Is.EqualTo("/dossier"));
        }

        [Test]
        public async Task CreateDossier_Editor_IsForbidden()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/dossier/create")), Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task CreateDossier_HeadEditor_SeesForm()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            Assert.That(await OpenAsync(page, Url("/dossier/create")), Is.EqualTo("/dossier/create"));
            await Assertions.Expect(page.Locator("input#Title")).ToBeVisibleAsync();
        }

        [Test]
        public async Task SeededDossierDetails_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url($"/dossier/details/{SeedData.DossierId}")),
                Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task SeededDossierEdit_AssignedEditor_IsAllowed()
        {
            // the seeded dossier is assigned to test-editor
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url($"/dossier/edit/{SeedData.DossierId}")),
                Is.EqualTo($"/dossier/edit/{SeedData.DossierId}"));
        }

        // ---------- dossier documents ----------

        [Test]
        public async Task DossierDocument_Anonymous_RedirectsToLogin()
        {
            var page = await OpenAnonymousAsync();
            var url = await SeededDocumentUrlAsync();
            Assert.That(await OpenDownloadAsync(page, url), Is.EqualTo(LoginPath));
        }

        [Test]
        public async Task DossierDocument_PlainUser_IsForbidden()
        {
            var url = await SeededDocumentUrlAsync();
            Assert.That(await OpenDownloadAsync(_plainUser.Page, url), Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task DossierDocument_Editor_Downloads()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            var url = await SeededDocumentUrlAsync();

            var response = await page.APIRequest.GetAsync(url);
            Assert.That(response.Status, Is.EqualTo(200),
                "The seeded dossier document could not be downloaded. " +
                "Check that backup.sh copied the fixture files into /srv/www/test.linc/test/.");
        }

        // ---------- sources ----------

        [Test]
        public async Task SourceAdmin_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url("/source/admin")), Is.EqualTo(AccessDeniedPath));
        }

        [Test]
        public async Task SourceAdmin_Editor_IsAllowed()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            Assert.That(await OpenAsync(page, Url("/source/admin")), Is.EqualTo("/source/admin"));
        }

        [Test]
        public async Task SourceEdit_PlainUser_IsForbidden()
        {
            Assert.That(await OpenAsync(_plainUser.Page, Url($"/source/edit/{SeedData.SourceId}")), Is.EqualTo(AccessDeniedPath));
        }

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
