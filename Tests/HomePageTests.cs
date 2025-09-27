using NUnit.Framework;

namespace linc.E2ETests
{
    public class HomePageTests : BasePageTests
    {
        [Test]
        public async Task HomePage_TitleIsCorrect_ForBulgarianCulture()
        {
            var urlBg = new Uri(BaseUri, "/?culture=bg").ToString();
            await Page.GotoAsync(urlBg);
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo("Начало - linc"));
        }

        [Test]
        public async Task HomePage_TitleIsCorrect_ForEnglishCulture()
        {
            var urlEn = new Uri(BaseUri, "/?culture=en").ToString();
            await Page.GotoAsync(urlEn);
            var title = await Page.TitleAsync();
            Assert.That(title, Is.EqualTo("Home - linc"));
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await Page.CloseAsync();
        }
    }
}