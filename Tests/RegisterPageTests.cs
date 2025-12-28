using NUnit.Framework;
using System.Security.Cryptography;

namespace linc.E2ETests
{
    [Parallelizable(ParallelScope.None)]
    public class RegisterPageTests : BasePageTests
    {
        protected Uri RegisterPageUrl { get; private set; } = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            RegisterPageUrl = new(BaseUri, "/identity/account/register");
        }

        [Test]
        public async Task en_Register_TestUser_ReceivesConfirmationEmail()
        {
            await SetPageCulture("en");
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

            await ConfirmSwalAlert();

            await Logout();

            // Verify email sent
            using var emailVerifier = new Utility.EmailVerifier(Config);
            await emailVerifier.AssertRegisterConfirmationSent("en");

        }

        [Test]
        public async Task bg_Register_TestUser_ReceivesConfirmationEmail()
        {
            await SetPageCulture("bg");
            await Page.GotoAsync(RegisterPageUrl.ToString());

            // Fill registration form fields
            var identifier = RandomNumberGenerator.GetInt32(100, 999);

            await Page.FillAsync("input[name='Input.FirstName']", "Тестов");
            await Page.FillAsync("input[name='Input.LastName']", "Потребител");
            await Page.FillAsync("input[name='Input.UserName']", $"тест-{identifier}");
            await Page.FillAsync("input[name='Input.Email']", $"test-{identifier}@test.com");
            await Page.FillAsync("input[name='Input.Password']", $"Test{identifier}!");
            await Page.FillAsync("input[name='Input.ConfirmPassword']", $"Test{identifier}!");
            await Page.CheckAsync("input[name='Input.PrivacyConsent']");

            // Submit the form
            await Page.ClickAsync("button#registerSubmit");

            await ConfirmSwalAlert();

            await Logout();

            // Verify email sent
            using var emailVerifier = new Utility.EmailVerifier(Config);
            await emailVerifier.AssertRegisterConfirmationSent("bg");

        }
    }
}
