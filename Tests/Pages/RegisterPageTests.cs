using linc.E2ETests.Utility;
using NUnit.Framework;

namespace linc.E2ETests.Pages
{
    [Parallelizable(ParallelScope.None)]
    public class RegisterPageTests : TestBase
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

            // the token is a separate word in the names,
            // the confirmation email greets the user by names
            var token = TestRun.NewToken();
            var startedAt = DateTimeOffset.UtcNow;

            await Page.FillAsync("input[name='Input.FirstName']", "Test");
            await Page.FillAsync("input[name='Input.LastName']", $"User {token}");
            await Page.FillAsync("input[name='Input.UserName']", $"user-{token}");
            await Page.FillAsync("input[name='Input.Email']", $"{token}@test.com");
            await Page.FillAsync("input[name='Input.Password']", $"Test{token}!");
            await Page.FillAsync("input[name='Input.ConfirmPassword']", $"Test{token}!");
            await Page.CheckAsync("input[name='Input.PrivacyConsent']");

            // Submit the form
            await Page.ClickAsync("button#registerSubmit");

            await ConfirmSwalAlert();

            await Logout();

            // Verify email sent
            using var emailVerifier = new EmailVerifier(Config);
            await emailVerifier.AssertRegisterConfirmationSent("en", token, startedAt);
        }

        [Test]
        public async Task bg_Register_TestUser_ReceivesConfirmationEmail()
        {
            await SetPageCulture("bg");
            await Page.GotoAsync(RegisterPageUrl.ToString());

            // the token stays ASCII, email body search is less reliable with Cyrillic
            var token = TestRun.NewToken();
            var startedAt = DateTimeOffset.UtcNow;

            await Page.FillAsync("input[name='Input.FirstName']", "Тестов");
            await Page.FillAsync("input[name='Input.LastName']", $"Потребител {token}");
            await Page.FillAsync("input[name='Input.UserName']", $"тест-{token}");
            await Page.FillAsync("input[name='Input.Email']", $"{token}@test.com");
            await Page.FillAsync("input[name='Input.Password']", $"Test{token}!");
            await Page.FillAsync("input[name='Input.ConfirmPassword']", $"Test{token}!");
            await Page.CheckAsync("input[name='Input.PrivacyConsent']");

            // Submit the form
            await Page.ClickAsync("button#registerSubmit");

            await ConfirmSwalAlert();

            await Logout();

            // Verify email sent
            using var emailVerifier = new EmailVerifier(Config);
            await emailVerifier.AssertRegisterConfirmationSent("bg", token, startedAt);
        }
    }
}
