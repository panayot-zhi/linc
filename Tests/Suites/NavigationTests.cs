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

        /// <summary>
        /// Logs in as an editor and checks the user menu has the "List of indexes" and "List of dossiers"
        /// links with the right text, once in English and once in Bulgarian.
        /// </summary>
        [TestCase("en")]
        [TestCase("bg")]
        public async Task UserMenu_Editor_ShowsIndexesAndDossiers(string culture)
        {
            var page = await OpenAsAsync(TestRole.Editor);
            await SetPageCulture(page, culture);

            Assert.That((await MenuLink(page, "/source/admin").TextContentAsync())?.Trim(),
                Is.EqualTo(SiteText.Get("NavItem_SourceAdmin", culture)));
            Assert.That((await MenuLink(page, "/dossier").TextContentAsync())?.Trim(),
                Is.EqualTo(SiteText.Get("NavItem_Submissions", culture)));
        }

        /// <summary>
        /// Checks the old "edit mode" switch is gone from the menu for editors (it was removed on purpose).
        /// </summary>
        [Test]
        public async Task UserMenu_Editor_HasNoEditModeToggle()
        {
            var page = await OpenAsAsync(TestRole.Editor);
            await page.GotoAsync(BaseUri.ToString());

            await Assertions.Expect(page.Locator("a[href='/home/edit']")).ToHaveCountAsync(0);
        }

        /// <summary>
        /// Registers an ordinary user and checks their menu has no links to the editor pages
        /// (index list and dossier list).
        /// </summary>
        [Test]
        public async Task UserMenu_PlainUser_HasNoEditorLinks()
        {
            var user = await RegisterUserAsync("Navigation", "Plain");
            await user.Page.GotoAsync(BaseUri.ToString());

            await Assertions.Expect(MenuLink(user.Page, "/source/admin")).ToHaveCountAsync(0);
            await Assertions.Expect(MenuLink(user.Page, "/dossier")).ToHaveCountAsync(0);
        }

        /// <summary>
        /// A head editor opens the index admin list and must see the "Create issue" link.
        /// </summary>
        [Test]
        public async Task SourceAdmin_HeadEditor_ShowsCreateIssue()
        {
            var page = await OpenAsAsync(TestRole.HeadEditor);
            await page.GotoAsync(Url("/source/admin"));

            await Assertions.Expect(page.Locator(".list-operations a[href='/issue/create']")).ToHaveCountAsync(1);
        }

        /// <summary>
        /// An editor opens the index admin list and must NOT see the "Create issue" link,
        /// because they are not allowed to create issues and the link would only lead to an error.
        /// </summary>
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
