using NUnit.Framework;

namespace linc.E2ETests
{
    public class HomePageTests : BasePageTests
    {
        [Test]
        public async Task bg_HomePage_TitleIsCorrect()
        {
            await SetPageCulture("bg");
            await Page.GotoAsync(BaseUri.ToString());
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo("Начало - linc"));
        }

        [Test]
        public async Task en_HomePage_TitleIsCorrect()
        {
            await SetPageCulture("en");
            await Page.GotoAsync(BaseUri.ToString());
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo("Home - linc"));
        }
    }
}