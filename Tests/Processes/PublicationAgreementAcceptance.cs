using linc.E2ETests.PageObjects;
using linc.E2ETests.Utility;
using Microsoft.Playwright;
using MimeKit;
using NUnit.Framework;

namespace linc.E2ETests.Processes;

/// <summary>
/// The publication agreement, one per author, from the acceptance email to signing and deleting it.
///
/// One dossier is shared by the whole fixture and the tests run in order:
///   - author "full":  a registered user with the same names and email, may sign
///   - author "email": a registered user with the same email but other names, may sign
///   - author "names": a registered user with the same names but another email, is refused
///   - a registered user matching no author, is refused
/// All email goes to the one test inbox (EmailConfig.OverwriteRecipients), so messages are matched
/// by the run token in the author names and by the agreement link they contain.
/// </summary>
[NonParallelizable]
public class PublicationAgreementAcceptance : TestBase
{
    private DossierPages _dossiers = null!;

    private RegisteredUser _fullMatchUser = null!;
    private RegisteredUser _emailMatchUser = null!;
    private RegisteredUser _namesMatchUser = null!;
    private RegisteredUser _noMatchUser = null!;

    private AuthorInput _fullMatchAuthor = null!;
    private AuthorInput _emailMatchAuthor = null!;
    private AuthorInput _namesMatchAuthor = null!;

    private int _dossierId;
    private readonly Dictionary<string, int> _authorIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Uri> _agreementLinks = new(StringComparer.OrdinalIgnoreCase);

    [OneTimeSetUp]
    public async Task CreateAcceptedDossier()
    {
        _dossiers = new DossierPages(BaseUri);

        // users first, so the dossier authors can be matched to them
        _fullMatchUser = await RegisterUserAsync("Agree", "Full");
        _emailMatchUser = await RegisterUserAsync("Other", "Person");
        _namesMatchUser = await RegisterUserAsync("Agree", "Names");
        _noMatchUser = await RegisterUserAsync("Agree", "Nobody");

        _fullMatchAuthor = new AuthorInput(_fullMatchUser.FirstName, _fullMatchUser.LastName, _fullMatchUser.Email);
        _emailMatchAuthor = new AuthorInput("Agree", $"Email {TestRun.NewToken()}", _emailMatchUser.Email);
        _namesMatchAuthor = new AuthorInput(_namesMatchUser.FirstName, _namesMatchUser.LastName, $"{TestRun.NewToken()}@test.com");

        var headEditor = await OpenAsAsync(TestRole.HeadEditor);
        _dossierId = await _dossiers.CreateAsync(headEditor, $"Dossier agreements {TestRun.NewToken()}",
            new[] { _fullMatchAuthor, _emailMatchAuthor, _namesMatchAuthor }, FixturePath("test-document.docx"));

        foreach (var author in await _dossiers.GetAuthorsAsync(headEditor, _dossierId))
        {
            _authorIds[author.Names] = author.Id!.Value;
        }

        // accepting the dossier sends one agreement email per author
        var acceptedAt = DateTimeOffset.UtcNow;
        await _dossiers.SetStatusAsync(headEditor, _dossierId, "Accepted");

        using var emailVerifier = new EmailVerifier(Config);
        foreach (var author in new[] { _fullMatchAuthor, _emailMatchAuthor, _namesMatchAuthor })
        {
            var authorId = _authorIds[author.Names];
            var linkPath = $"/dossier/{_dossierId}/agreement";

            var message = await emailVerifier.WaitForMessageAsync(LastWord(author.LastName), acceptedAt,
                m => AgreementLink(m, linkPath, authorId) != null);

            _agreementLinks[author.Names] = AgreementLink(message, linkPath, authorId)!;
        }
    }

    private static string LastWord(string value) => value.Split(' ').Last();

    private static Uri? AgreementLink(MimeMessage message, string linkPath, int authorId)
    {
        var link = EmailVerifier.FindLink(message, linkPath);
        return link != null && link.Query.Contains($"aid={authorId}") ? link : null;
    }

    /// <summary>
    /// The link from the email, but on the configured server (the email may carry another host).
    /// </summary>
    private string AgreementUrl(AuthorInput author) =>
        new Uri(BaseUri, _agreementLinks[author.Names].PathAndQuery).ToString();

    private static async Task SubmitAgreementAsync(IPage page)
    {
        await page.CheckAsync("#agreement-form input#agreement");
        await page.ClickAsync("#agreement-form input[type='submit']");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Test, Order(1)]
    public void EachAuthor_ReceivesOwnAgreementLink()
    {
        Assert.That(_agreementLinks.Keys, Is.EquivalentTo(new[]
        {
            _fullMatchAuthor.Names, _emailMatchAuthor.Names, _namesMatchAuthor.Names
        }));
    }

    [Test, Order(2)]
    public async Task EmptySubmission_ReturnsToFormWithWarning()
    {
        var page = _fullMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_fullMatchAuthor));

        // skip the browser's own check to reach the server-side one
        await page.EvalOnSelectorAsync("#agreement-form input#agreement", "e => e.removeAttribute('required')");
        await page.ClickAsync("#agreement-form input[type='submit']");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(CurrentPath(page), Is.EqualTo($"/dossier/{_dossierId}/agreement"));
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("warning"));
    }

    [Test, Order(3)]
    public async Task FullMatch_Signs_AndReceivesConfirmation()
    {
        var page = _fullMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_fullMatchAuthor));

        var signedAt = DateTimeOffset.UtcNow;
        await SubmitAgreementAsync(page);

        Assert.That(CurrentPath(page), Is.EqualTo("/"));
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("success"));

        var headEditor = await OpenAsAsync(TestRole.HeadEditor);
        await headEditor.GotoAsync(_dossiers.DetailsUrl(_dossierId));
        await Assertions.Expect(DossierPages.SignedAgreementLink(headEditor, _authorIds[_fullMatchAuthor.Names]))
            .ToHaveCountAsync(1);

        // signing sends the signed agreement back to the author
        using var emailVerifier = new EmailVerifier(Config);
        var confirmation = await emailVerifier.WaitForMessageAsync(LastWord(_fullMatchAuthor.LastName), signedAt);
        Assert.That(confirmation, Is.Not.Null);
    }

    [Test, Order(4)]
    public async Task EmailOnlyMatch_Signs()
    {
        var page = _emailMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_emailMatchAuthor));
        await SubmitAgreementAsync(page);

        Assert.That(CurrentPath(page), Is.EqualTo("/"));
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("success"));
    }

    [Test, Order(5)]
    public async Task NamesOnlyMatch_IsRedirectedWithWarning()
    {
        var page = _namesMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_namesMatchAuthor));

        Assert.That(CurrentPath(page), Is.EqualTo("/"));
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("warning"));
        await Assertions.Expect(page.Locator("#agreement-form")).ToHaveCountAsync(0);
    }

    [Test, Order(6)]
    public async Task NoMatch_IsRedirectedWithWarning()
    {
        var page = _noMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_namesMatchAuthor));

        Assert.That(CurrentPath(page), Is.EqualTo("/"));
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("warning"));
    }

    [Test, Order(7)]
    public async Task SignedAgreement_DownloadRights()
    {
        var url = AgreementUrl(_fullMatchAuthor);

        // the author linked to the signing user sees the signed document
        var own = await _fullMatchUser.Page.APIRequest.GetAsync(url);
        Assert.That(own.Status, Is.EqualTo(200));
        Assert.That(own.Headers["content-type"], Does.Contain("pdf"));

        // another plain user and an editor who is not assigned are refused
        var other = await _noMatchUser.Page.APIRequest.GetAsync(url);
        Assert.That(new Uri(other.Url).AbsolutePath, Is.EqualTo("/home/error/403").IgnoreCase);

        var editor = await OpenAsAsync(TestRole.Editor);
        var editorResponse = await editor.APIRequest.GetAsync(url);
        Assert.That(new Uri(editorResponse.Url).AbsolutePath, Is.EqualTo("/home/error/403").IgnoreCase);

        // a head editor downloads it from the dossier details
        var headEditor = await OpenAsAsync(TestRole.HeadEditor);
        await headEditor.GotoAsync(_dossiers.DetailsUrl(_dossierId));
        var href = await DossierPages.SignedAgreementLink(headEditor, _authorIds[_fullMatchAuthor.Names])
            .First.GetAttributeAsync("href");
        var download = await headEditor.APIRequest.GetAsync(new Uri(BaseUri, href).ToString());
        Assert.That(download.Status, Is.EqualTo(200));
    }

    [Test, Order(8)]
    public async Task SignedAuthor_IsLockedOnEditForm()
    {
        var headEditor = await OpenAsAsync(TestRole.HeadEditor);
        await headEditor.GotoAsync(_dossiers.EditUrl(_dossierId));

        var authors = new AuthorsEditor(headEditor);
        var index = await authors.IndexOfAsync(_fullMatchAuthor.Names);
        var rows = await authors.ReadAsync();

        Assert.That(rows[index].ReadOnly, Is.True, "A signed author's names should be read only.");
        await Assertions.Expect(authors.Cards.Nth(index).Locator(".remove-author")).ToHaveClassAsync(new System.Text.RegularExpressions.Regex(@"\bdisabled\b"));
    }

    [Test, Order(9)]
    public async Task HeadEditor_DeletesAgreement_AuthorSignsAgain()
    {
        var headEditor = await OpenAsAsync(TestRole.HeadEditor);
        await headEditor.GotoAsync(_dossiers.EditUrl(_dossierId));

        var authors = new AuthorsEditor(headEditor);
        var index = await authors.IndexOfAsync(_fullMatchAuthor.Names);
        await authors.ExpandAsync();
        await authors.Cards.Nth(index).Locator(".delete-agreement-button").ClickAsync();
        await ConfirmSwalAlert(headEditor);
        await headEditor.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await headEditor.GotoAsync(_dossiers.DetailsUrl(_dossierId));
        await Assertions.Expect(DossierPages.SignedAgreementLink(headEditor, _authorIds[_fullMatchAuthor.Names]))
            .ToHaveCountAsync(0);

        // the author gets the form again and can sign once more
        var page = _fullMatchUser.Page;
        await page.GotoAsync(AgreementUrl(_fullMatchAuthor));
        await Assertions.Expect(page.Locator("#agreement-form")).ToHaveCountAsync(1);

        await SubmitAgreementAsync(page);
        Assert.That(await GetSwalIconAsync(page), Is.EqualTo("success"));
    }
}
