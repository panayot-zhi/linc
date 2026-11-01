using linc.E2ETests.Utility;
using Microsoft.Playwright;
using NUnit.Framework;

namespace linc.E2ETests.Suites
{
    /// <summary>
    /// Links in the user menu and on the source admin page.
    /// </summary>
    [NonParallelizable]
    public class NavigationTests : TestBase
    {
        private static ILocator MenuLink(IPage page, string href) =>
            page.Locator($"#navbar li.dropdown a[href='{href}']");

        [TestCase("en", "List of indexes", "List of dossiers")]
        [TestCase("bg", "Списък съдържания", "Списък досиета")]
        public async Task UserMenu_Editor_ShowsIndexesAndDossiers(string culture, string indexesLabel, string dossiersLabel)
        {
            var page = await OpenAsAsync(TestRole.Editor);
            await SetPageCulture(page, culture);

            Assert.That((await MenuLink(page, "/source/admin").TextContentAsync())?.Trim(), Is.EqualTo(indexesLabel));
            Assert.That((await MenuLink(page, "/dossier").TextContentAsync())?.Trim(), Is.EqualTo(dossiersLabel));
        }

        [Test]
        public async Task UserMenu_Editor_HasNoEditModeToggle()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            await page.GotoAsync(BaseUri.ToString());

            await Assertions.Expect(page.Locator("a[href='/home/edit']")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task UserMenu_PlainUser_HasNoEditorLinks()
        {
            var user = await RegisterUserAsync("Navigation", "Plain");
            await user.Page.GotoAsync(BaseUri.ToString());

            await Assertions.Expect(MenuLink(user.Page, "/source/admin")).ToHaveCountAsync(0);
            await Assertions.Expect(MenuLink(user.Page, "/dossier")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task SourceAdmin_HeadEditor_ShowsCreateIssue()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            await page.GotoAsync(Url("/source/admin"));

            await Assertions.Expect(page.Locator(".list-operations a[href='/issue/create']")).ToHaveCountAsync(1);
        }

        [Test]
        public async Task SourceAdmin_Editor_HidesCreateIssue()
        {
            // issue creation is limited to head editors, so editors should not be offered the link
            var page = await OpenAsAsync(TestRole.Editor);
            await page.GotoAsync(Url("/source/admin"));

            await Assertions.Expect(page.Locator(".list-operations a[href='/issue/create']")).ToHaveCountAsync(0);
        }
    }
}
