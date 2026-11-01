using linc.E2ETests.PageObjects;
using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Linking authors to user accounts.
    ///
    /// Not covered: changing the names in the profile does not update linked authors today,
    /// the profile page never calls the author sync, so there is nothing to test yet.
    /// </summary>
    [NonParallelizable]
    public class AccountAuthorSyncTests : TestBase
    {
        private DossierPages _dossiers = null!;

        [OneTimeSetUp]
        public void SetUpPages()
        {
            _dossiers = new DossierPages(BaseUri);
        }

        /// <summary>
        /// A head editor creates a dossier with an author whose email has no account yet.
        /// Then someone registers with that email (under other names) and clicks the link in the confirmation email.
        /// The dossier page must now show that user's account next to the author: the two are linked by email.
        /// </summary>
        [Test]
        public async Task ConfirmingEmail_LinksUserToAuthorWithSameEmail()
        {
            // an author whose email has no account yet
            var authorToken = TestRun.NewToken();
            var author = new AuthorInput("Sync", $"Author {authorToken}", $"{authorToken}@test.com");

            var headEditor = await OpenAsAsync(TestRole.HeadEditor);
            var dossierId = await _dossiers.CreateAsync(headEditor, $"Dossier account sync {TestRun.NewToken()}",
                new[] { author }, FixturePath("test-document.docx"));

            // a user registers later with that email, under other names
            var user = await RegisterUserAsync("Other", "Person", email: author.Email);

            await headEditor.GotoAsync(_dossiers.DetailsUrl(dossierId));
            await Assertions.Expect(DossierPages.AuthorLine(headEditor, author.Names)).Not.ToContainTextAsync(user.UserName);

            // confirming the email links the account to the author
            using (var emailVerifier = new EmailVerifier(Config))
            {
                var confirmationLink = await emailVerifier.AssertRegisterConfirmationSent("en", user.Token, user.RegisteredAt);
                await user.Page.GotoAsync(new Uri(BaseUri, confirmationLink.PathAndQuery).ToString());
                await user.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }

            await headEditor.GotoAsync(_dossiers.DetailsUrl(dossierId));
            await Assertions.Expect(DossierPages.AuthorLine(headEditor, author.Names)).ToContainTextAsync(user.UserName);
        }

        /// <summary>
        /// A user registers first. A head editor then creates a dossier with an author with exactly the same names
        /// (but another email). The dossier page must show the user's account next to the author: they are linked by names.
        /// </summary>
        [Test]
        public async Task NewAuthor_WithNamesOfExistingUser_IsLinkedOnCreate()
        {
            var user = await RegisterUserAsync("Linked", "Names");
            var author = new AuthorInput(user.FirstName, user.LastName, $"{TestRun.NewToken()}@test.com");

            var headEditor = await OpenAsAsync(TestRole.HeadEditor);
            var dossierId = await _dossiers.CreateAsync(headEditor, $"Dossier names link {TestRun.NewToken()}",
                new[] { author }, FixturePath("test-document.docx"));

            await headEditor.GotoAsync(_dossiers.DetailsUrl(dossierId));
            await Assertions.Expect(DossierPages.AuthorLine(headEditor, author.Names)).ToContainTextAsync(user.UserName);
        }
    }
}
