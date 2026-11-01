using System.Security.Cryptography;
using linc.E2ETests.PageObjects;
using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Index entries (sources) with several authors. New sources go into the hidden seeded issue,
    /// so they never show on the public home page; the weekly restore removes them.
    /// </summary>
    [NonParallelizable]
    public class SourceAuthorsTests : TestBase
    {
        private SourcePages _sources = null!;
        private DossierPages _dossiers = null!;

        [OneTimeSetUp]
        public void SetUpPages()
        {
            _sources = new SourcePages(BaseUri);
            _dossiers = new DossierPages(BaseUri);
        }

        /// <summary>
        /// The page number is part of the stored PDF name; a random one keeps sources from sharing a file.
        /// </summary>
        private static int UniquePdfPage() => RandomNumberGenerator.GetInt32(1000, 1_000_000);

        private SourceInput NewSource(string titlePrefix, params AuthorInput[] authors) => new(
            Title: $"{titlePrefix} {TestRun.NewToken()}",
            Authors: authors,
            IssueId: SeedData.IssueId,
            LanguageId: SeedData.LanguageIdBg,
            PdfFilePath: FixturePath("test-source.pdf"),
            PdfPage: UniquePdfPage());

        private static AuthorInput NewAuthor(string firstName, string? notes = null)
        {
            var token = TestRun.NewToken();
            return new AuthorInput(firstName, $"Author {token}", Notes: notes);
        }

        [Test]
        public async Task Create_WithTwoAuthors_ShowsBothInLibraryAndIssue()
        {
            var editor = await OpenAsAsync(TestRole.Editor);
            var first = NewAuthor("First", "First author note.");
            var second = NewAuthor("Second", "Second author note.");
            var input = NewSource("Source two authors", first, second) with { AuthorsNotes = "Shared authors note." };

            await _sources.CreateAsync(editor, input);

            // the library list, filtered by one author, shows the entry with both authors
            await editor.GotoAsync(_sources.IndexByAuthorUrl(first.LastName));
            var listed = await SourcePages.ListedAuthorNamesAsync(editor);
            Assert.That(listed.Select(x => x.Trim()), Does.Contain(first.Names).And.Contain(second.Names));

            // the hidden issue is visible to editors and lists the entry too
            await editor.GotoAsync(_sources.Url($"/issue/details/{SeedData.IssueId}"));
            var issueAuthors = await SourcePages.ListedAuthorNamesAsync(editor);
            Assert.That(issueAuthors.Select(x => x.Trim()), Does.Contain(first.Names).And.Contain(second.Names));
        }

        [Test]
        public async Task Create_Editor_LandsOnEditForm()
        {
            var editor = await OpenAsAsync(TestRole.Editor);
            var id = await _sources.CreateAsync(editor, NewSource("Source editor redirect", NewAuthor("Redirect")));

            Assert.That(CurrentPath(editor), Is.EqualTo($"/source/edit/{id}"));
            await Assertions.Expect(editor.Locator("input#Title")).ToBeVisibleAsync();
        }

        [Test]
        public async Task Create_KeepsAuthorNotesOnEditForm()
        {
            var editor = await OpenAsAsync(TestRole.Editor);
            var author = NewAuthor("Noted", "A note for this author only.");

            var id = await _sources.CreateAsync(editor, NewSource("Source notes", author));

            await editor.GotoAsync(_sources.EditUrl(id));
            var rows = await new AuthorsEditor(editor).ReadAsync();

            Assert.That(rows.Single(x => x.Names == author.Names).Notes, Is.EqualTo(author.Notes));
        }

        [Test]
        public async Task Edit_Editor_ReplacesAuthor()
        {
            var editor = await OpenAsAsync(TestRole.Editor);
            var kept = NewAuthor("Kept");
            var removed = NewAuthor("Removed");
            var added = NewAuthor("Added");

            var id = await _sources.CreateAsync(editor, NewSource("Source edit authors", kept, removed));

            await editor.GotoAsync(_sources.EditUrl(id));
            var authors = new AuthorsEditor(editor);
            await authors.RemoveAsync(await authors.IndexOfAsync(removed.Names));
            await authors.AddAsync(added);
            await editor.ClickAsync("main form button[type='submit']");
            await editor.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // saving returns to the source admin list
            Assert.That(CurrentPath(editor), Is.EqualTo("/source/admin"));

            await editor.GotoAsync(_sources.IndexByAuthorUrl(kept.LastName));
            var listed = (await SourcePages.ListedAuthorNamesAsync(editor)).Select(x => x.Trim()).ToList();
            Assert.That(listed, Does.Contain(kept.Names).And.Contain(added.Names));
            Assert.That(listed, Does.Not.Contain(removed.Names));
        }

        [Test]
        public async Task Delete_Administrator_RemovesFromLibrary()
        {
            var editor = await OpenAsAsync(TestRole.Editor);
            var author = NewAuthor("Deleted");
            var id = await _sources.CreateAsync(editor, NewSource("Source deleted", author));

            var admin = await OpenAsAsync(TestRole.Administrator);
            await _sources.DeleteAsync(admin, id);

            await admin.GotoAsync(_sources.IndexByAuthorUrl(author.LastName));
            var listed = await SourcePages.ListedAuthorNamesAsync(admin);
            Assert.That(listed.Select(x => x.Trim()), Does.Not.Contain(author.Names));
        }

        [Test]
        public async Task CreateFromDossier_PrefillsDossierAuthors_AndAllowsOneSourcePerLanguage()
        {
            var headEditor = await OpenAsAsync(TestRole.HeadEditor);
            var first = new AuthorInput("Dossier", $"Author {TestRun.NewToken()}", $"{TestRun.NewToken()}@test.com");
            var second = new AuthorInput("Dossier", $"Author {TestRun.NewToken()}", $"{TestRun.NewToken()}@test.com");
            var dossierId = await _dossiers.CreateAsync(headEditor, $"Dossier with sources {TestRun.NewToken()}",
                new[] { first, second }, FixturePath("test-document.docx"), SeedData.LanguageIdBg);

            // the create form takes title and authors from the dossier
            await headEditor.GotoAsync(_sources.CreateUrl(SeedData.IssueId, dossierId));
            var prefilled = await new AuthorsEditor(headEditor).ReadAsync();
            Assert.That(prefilled.Select(x => x.Names), Is.EquivalentTo(new[] { first.Names, second.Names }));

            // one source per language may point to the same dossier
            var bg = NewSource("Source bg") with { DossierId = dossierId, LanguageId = SeedData.LanguageIdBg };
            var en = NewSource("Source en") with { DossierId = dossierId, LanguageId = SeedData.LanguageIdEn };
            await _sources.CreateAsync(headEditor, bg);
            await _sources.CreateAsync(headEditor, en);

            await headEditor.GotoAsync(_sources.IndexByAuthorUrl(first.LastName));
            var listed = await SourcePages.ListedAuthorNamesAsync(headEditor);
            Assert.That(listed.Select(x => x.Trim()), Does.Contain(first.Names));
        }
    }
}
