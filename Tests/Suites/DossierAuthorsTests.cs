using linc.E2ETests.PageObjects;
using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Creating and editing dossiers with several authors, as a head editor.
    /// Every dossier is new and named with a run token; the weekly restore removes them.
    /// </summary>
    [NonParallelizable]
    public class DossierAuthorsTests : TestBase
    {
        private DossierPages _dossiers = null!;

        [OneTimeSetUp]
        public void SetUpPages()
        {
            _dossiers = new DossierPages(BaseUri);
        }

        private static AuthorInput NewAuthor(string firstName)
        {
            var token = TestRun.NewToken();
            return new AuthorInput(firstName, $"Author {token}", $"{token}@test.com");
        }

        /// <summary>
        /// A head editor creates a dossier with two authors.
        /// Both authors (with their emails) must show on the dossier page, and both names must show
        /// next to the title in the dossier list.
        /// </summary>
        [Test]
        public async Task Create_WithTwoAuthors_ShowsBothOnDetailsAndList()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var title = $"Dossier two authors {TestRun.NewToken()}";
            var first = NewAuthor("First");
            var second = NewAuthor("Second");

            var id = await _dossiers.CreateAsync(page, title, new[] { first, second }, FixturePath("test-document.docx"));

            await page.GotoAsync(_dossiers.DetailsUrl(id));
            await Assertions.Expect(DossierPages.AuthorLine(page, first.Names)).ToHaveCountAsync(1);
            await Assertions.Expect(DossierPages.AuthorLine(page, second.Names)).ToHaveCountAsync(1);
            await Assertions.Expect(DossierPages.AuthorLine(page, first.Names)).ToContainTextAsync(first.Email!);

            // the list shows the authors under the title
            await page.GotoAsync(_dossiers.Url("/dossier?sort=DateCreated&order=Desc"));
            var row = page.Locator("table tbody tr", new PageLocatorOptions { HasText = title });
            await Assertions.Expect(row).ToContainTextAsync(first.Names);
            await Assertions.Expect(row).ToContainTextAsync(second.Names);
        }

        /// <summary>
        /// A head editor removes every author from the create form and saves.
        /// The dossier must not be created: the form comes back with an error, because a dossier needs at least one author.
        /// </summary>
        [Test]
        public async Task Create_WithoutAuthors_ShowsValidationError()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var title = $"Dossier no authors {TestRun.NewToken()}";

            var authors = await _dossiers.FillCreateFormAsync(page, title, Array.Empty<AuthorInput>(),
                FixturePath("test-document.docx"));
            await authors.RemoveAsync(0);
            Assert.That(await authors.CountAsync(), Is.EqualTo(0));

            await DossierPages.SubmitFormAsync(page);

            Assert.That(CurrentPath(page), Is.EqualTo("/dossier/create"));
            await Assertions.Expect(page.Locator(".validation-summary-errors li")).Not.ToHaveCountAsync(0);
        }

        /// <summary>
        /// A head editor adds an author without an email and saves.
        /// The form must come back with the email field marked red; the email is needed to send the agreement.
        /// </summary>
        [Test]
        public async Task Create_AuthorWithoutEmail_ShowsValidationError()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var title = $"Dossier missing email {TestRun.NewToken()}";
            var author = NewAuthor("NoEmail") with { Email = null };

            await _dossiers.FillCreateFormAsync(page, title, new[] { author }, FixturePath("test-document.docx"));
            await DossierPages.SubmitFormAsync(page);

            Assert.That(CurrentPath(page), Is.EqualTo("/dossier/create"));
            await Assertions.Expect(page.Locator("#authors-list .author-email.is-invalid")).ToHaveCountAsync(1);
        }

        /// <summary>
        /// Creates a dossier with two authors, then edits it: removes one author and adds a new one.
        /// The dossier page must show the kept and the new author, and no longer show the removed one.
        /// </summary>
        [Test]
        public async Task Edit_AddAndRemoveAuthors_ShowsNewAuthorsOnDetails()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var title = $"Dossier edit authors {TestRun.NewToken()}";
            var kept = NewAuthor("Kept");
            var removed = NewAuthor("Removed");
            var added = NewAuthor("Added");

            var id = await _dossiers.CreateAsync(page, title, new[] { kept, removed }, FixturePath("test-document.docx"));

            await page.GotoAsync(_dossiers.EditUrl(id));
            var authors = new AuthorsEditor(page);
            await authors.RemoveAsync(await authors.IndexOfAsync(removed.Names));
            await authors.AddAsync(added);
            await DossierPages.SubmitFormAsync(page);

            Assert.That(CurrentPath(page), Is.EqualTo($"/dossier/details/{id}"));
            await Assertions.Expect(DossierPages.AuthorLine(page, kept.Names)).ToHaveCountAsync(1);
            await Assertions.Expect(DossierPages.AuthorLine(page, added.Names)).ToHaveCountAsync(1);
            await Assertions.Expect(DossierPages.AuthorLine(page, removed.Names)).ToHaveCountAsync(0);
        }

        /// <summary>
        /// A head editor creates a dossier and assigns nobody. An editor then tries to edit it
        /// and must get the 403 page: editors may only edit the dossiers assigned to them.
        /// </summary>
        [Test]
        public async Task Edit_UnassignedEditor_IsForbidden()
        {
            var headEditor = await OpenAsAsync(TestRole.HeadEditor);
            var id = await _dossiers.CreateAsync(headEditor, $"Dossier unassigned {TestRun.NewToken()}",
                new[] { NewAuthor("Unassigned") }, FixturePath("test-document.docx"));

            // nobody is assigned yet, so a plain editor may not edit it
            var editor = await OpenAsAsync(TestRole.Editor);
            await editor.GotoAsync(_dossiers.EditUrl(id));
            Assert.That(CurrentPath(editor), Is.EqualTo("/home/error/403"));
        }

        /// <summary>
        /// On the create dossier form, types part of a seeded author's name into the author search
        /// and picks the suggestion. The author's names and email must be filled in automatically.
        /// </summary>
        [Test]
        public async Task AuthorSearch_PicksSeededAuthor_FillsCard()
        {
            // the seeded authors are in Bulgarian, the search is filtered by the form's language
            var page = await OpenAsAsync(TestRole.HeadEditor);
            await page.GotoAsync(_dossiers.Url("/dossier/create"));
            await page.SelectOptionAsync("select#LanguageId", SeedData.LanguageIdBg.ToString());

            var authors = new AuthorsEditor(page);
            await authors.PickSearchResultAsync(0, SeedData.AuthorToken1, SeedData.Author1Names);

            var rows = await authors.ReadAsync();
            Assert.That(rows[0].Names, Is.EqualTo(SeedData.Author1Names));
            Assert.That(rows[0].Email, Is.EqualTo(SeedData.Author1Email));
        }

        /// <summary>
        /// Same search, but with the dossier language set to English.
        /// The seeded author is Bulgarian, so nothing must be suggested: the search only offers authors in the dossier's language.
        /// </summary>
        [Test]
        public async Task AuthorSearch_OtherLanguage_FindsNothing()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            await page.GotoAsync(_dossiers.Url("/dossier/create"));
            await page.SelectOptionAsync("select#LanguageId", SeedData.LanguageIdEn.ToString());

            var suggestions = await new AuthorsEditor(page).SearchAsync(0, SeedData.AuthorToken1);
            Assert.That(suggestions, Has.None.Contains(SeedData.AuthorToken1));
        }
    }
}
