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
        public async Task OneTimeSetUp()
        {
            RegisterPageUrl = new(BaseUri, "/identity/account/register");
        }

        [Test]
        public async Task Register_TestUser_ReceivesConfirmationEmail()
        {
            var testGMailUserName = Config.TestGMailUserName;
            var testGMailUserPassword = Config.TestGMailUserPassword;

            await Page.GotoAsync(RegisterPageUrl.ToString());

            // Fill registration form fields
            var identifier = RandomNumberGenerator.GetInt32(100, 999);

            await Page.FillAsync("input[name='Input.FirstName']", $"Test");
            await Page.FillAsync("input[name='Input.LastName']", $"User");
            await Page.FillAsync("input[name='Input.UserName']", $"test-{identifier}");
            await Page.FillAsync("input[name='Input.Email']", testGMailUserName);
            await Page.FillAsync("input[name='Input.Password']", $"Test{identifier}!");
            await Page.FillAsync("input[name='Input.ConfirmPassword']", $"Test{identifier}!");
            await Page.CheckAsync("input[name='Input.PrivacyConsent']");

            // Submit the form
            await Page.ClickAsync("button#registerSubmit");

            // Wait for email delivery
            await Task.Delay(TimeSpan.FromMinutes(1));

            // Connect to Gmail via IMAP and search for the confirmation email
            using var client = new ImapClient();

            try
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                await client.AuthenticateAsync(testGMailUserName, testGMailUserPassword);

                var testFolder = await client.GetFolderAsync("linc-test");
                await testFolder.OpenAsync(FolderAccess.ReadWrite);

                var query = SearchQuery.Flagged.And(SearchQuery.NotSeen);
                var uniqueIds = await testFolder.SearchAsync(query);

                Assert.That(uniqueIds, Is.Not.Null, "Could not retrieve messages from Gmail inbox.");
                Assert.That(uniqueIds.Count, Is.Not.Zero, "Retrieved 0 messages from Gmail inbox.");
                Assert.That(uniqueIds.Count, Is.EqualTo(1), "Retrieved more than 1 messages from Gmail inbox.");

                var messageId = uniqueIds.Single();

                await testFolder.AddFlagsAsync(messageId, MessageFlags.Seen, true);
                await testFolder.RemoveFlagsAsync(messageId, MessageFlags.Flagged, true);

                var message = await testFolder.GetMessageAsync(messageId);

                Console.WriteLine("Subject: " + message.Subject);
                Console.WriteLine("From: " + message.From);
                Console.WriteLine("Text Body: " + message.TextBody);
                Console.WriteLine("HTML Body: " + message.HtmlBody);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
