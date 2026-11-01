using System.Text.RegularExpressions;
using linc.E2ETests.PageObjects;
using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Uploading, downloading and replacing dossier documents, as a head editor.
    /// </summary>
    [NonParallelizable]
    public class DossierDocumentsTests : TestBase
    {
        private DossierPages _dossiers = null!;

        [OneTimeSetUp]
        public void SetUpPages()
        {
            _dossiers = new DossierPages(BaseUri);
        }

        /// <summary>
        /// Document id and absolute download address of every document listed on the details page.
        /// </summary>
        private async Task<Dictionary<int, string>> ListDocumentsAsync(IPage page, int dossierId)
        {
            await page.GotoAsync(_dossiers.DetailsUrl(dossierId));

            var documents = new Dictionary<int, string>();
            foreach (var link in await DossierPages.DocumentLinks(page, dossierId).AllAsync())
            {
                var href = await link.GetAttributeAsync("href") ?? string.Empty;
                var match = Regex.Match(href, @"/document/(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    documents[int.Parse(match.Groups[1].Value)] = new Uri(BaseUri, href).ToString();
                }
            }

            return documents;
        }

        private async Task UploadOnEditAsync(IPage page, int dossierId)
        {
            await page.GotoAsync(_dossiers.EditUrl(dossierId));
            await page.SetInputFilesAsync("input#Document", FixturePath("test-document.docx"));
            var post = page.WaitForResponseAsync(r => r.Request.Method == "POST" && r.Url.Contains($"/dossier/edit/{dossierId}"));
            await DossierPages.SubmitFormAsync(page);
            var status = (await post).Status;

            if (CurrentPath(page) != $"/dossier/details/{dossierId}")
            {
                Assert.Fail($"Uploading on dossier {dossierId} answered HTTP {status} and ended on {page.Url}. " +
                    await FormErrors.DescribeAsync(page));
            }
        }

        /// <summary>
        /// A head editor creates a dossier with a Word file. The dossier page must list one document,
        /// and its download link must return the file.
        /// </summary>
        [Test]
        public async Task OriginalDocument_IsDownloadable()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var dossierId = await _dossiers.CreateAsync(page, $"Dossier documents {TestRun.NewToken()}",
                new[] { new AuthorInput("Document", $"Author {TestRun.NewToken()}", $"{TestRun.NewToken()}@test.com") },
                FixturePath("test-document.docx"));

            var documents = await ListDocumentsAsync(page, dossierId);
            Assert.That(documents, Has.Count.EqualTo(1));

            var response = await page.APIRequest.GetAsync(documents.Values.Single());
            Assert.That(response.Status, Is.EqualTo(200));
        }

        /// <summary>
        /// A head editor uploads an anonymized (names removed) version of the article, then uploads another one to replace it.
        /// Only the newest anonymized file must remain: the old download link must stop working (404) and the new one must work.
        /// Currently fails: the server crashes (500) on the second upload.
        /// </summary>
        [Test]
        public async Task ReplacingAnonymizedDocument_RemovesThePreviousOne()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            var dossierId = await _dossiers.CreateAsync(page, $"Dossier replace {TestRun.NewToken()}",
                new[] { new AuthorInput("Replace", $"Author {TestRun.NewToken()}", $"{TestRun.NewToken()}@test.com") },
                FixturePath("test-document.docx"));

            var original = await ListDocumentsAsync(page, dossierId);

            // status New: the upload is the anonymized document and the dossier becomes Prepared
            await UploadOnEditAsync(page, dossierId);
            var afterFirst = await ListDocumentsAsync(page, dossierId);
            var firstAnonymized = afterFirst.Keys.Except(original.Keys).Single();

            // status Prepared: a new upload replaces the anonymized document
            // KNOWN ISSUE: the server answers 500 here (DossierService.UpdateDossierAsync, case Prepared)
            await UploadOnEditAsync(page, dossierId);
            var afterSecond = await ListDocumentsAsync(page, dossierId);
            var secondAnonymized = afterSecond.Keys.Except(original.Keys).Single();

            Assert.That(secondAnonymized, Is.Not.EqualTo(firstAnonymized));
            Assert.That(afterSecond, Has.Count.EqualTo(original.Count + 1));

            var oldResponse = await page.APIRequest.GetAsync(afterFirst[firstAnonymized]);
            Assert.That(oldResponse.Status, Is.EqualTo(404), "The replaced document should be gone.");

            var newResponse = await page.APIRequest.GetAsync(afterSecond[secondAnonymized]);
            Assert.That(newResponse.Status, Is.EqualTo(200));
        }
    }
}
