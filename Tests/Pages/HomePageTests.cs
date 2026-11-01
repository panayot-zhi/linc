using linc.E2ETests.Utility;
using NUnit.Framework;

namespace linc.E2ETests.Pages
{
    public class HomePageTests : TestBase
    {
        /// <summary>
        /// Opens the home page in Bulgarian and checks the browser tab title is "Начало - linc".
        /// If this fails, the site is down, the basic auth is wrong, or the Bulgarian texts are broken.
        /// </summary>
        [Test]
        public async Task bg_HomePage_TitleIsCorrect()
        {
            await SetPageCulture("bg");
            await Page.GotoAsync(BaseUri.ToString());
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo(SiteText.PageTitle("HomeIndex_Title", "bg")));
        }

        /// <summary>
        /// Opens the home page in English and checks the browser tab title is "Home - linc".
        /// If this fails, the site is down, the basic auth is wrong, or switching to English does not work.
        /// </summary>
        [Test]
        public async Task en_HomePage_TitleIsCorrect()
        {
            await SetPageCulture("en");
            await Page.GotoAsync(BaseUri.ToString());
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo(SiteText.PageTitle("HomeIndex_Title", "en")));
        }
    }
}