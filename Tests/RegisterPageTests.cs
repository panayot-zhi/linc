using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using NUnit.Framework;
using System.Security.Cryptography;

namespace linc.E2ETests
{
    public class RegisterPageTests : BasePageTests
    {
        protected Uri RegisterPageUrl { get; private set; } = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            RegisterPageUrl = new(BaseUri, "/identity/account/register");
        }

        [Test]
        public async Task Register_TestUser_ReceivesConfirmationEmail()
        {
            await Page.GotoAsync(RegisterPageUrl.ToString());

            // Fill registration form fields
            var identifier = RandomNumberGenerator.GetInt32(100, 999);

            await Page.FillAsync("input[name='Input.FirstName']", "Test");
            await Page.FillAsync("input[name='Input.LastName']", "User");
            await Page.FillAsync("input[name='Input.UserName']", $"test-{identifier}");
            await Page.FillAsync("input[name='Input.Email']", $"test-{identifier}@test.com");
            await Page.FillAsync("input[name='Input.Password']", $"Test{identifier}!");
            await Page.FillAsync("input[name='Input.ConfirmPassword']", $"Test{identifier}!");
            await Page.CheckAsync("input[name='Input.PrivacyConsent']");

            // Submit the form
            await Page.ClickAsync("button#registerSubmit");

            using var emailVerifier = new Utility.EmailVerifier(Config);
            await emailVerifier.AssertRegisterConfirmationSent();
        }
    }
}
