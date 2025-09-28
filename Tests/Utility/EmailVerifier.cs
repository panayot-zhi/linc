using linc.E2ETests.Configuration;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using NUnit.Framework;

namespace linc.E2ETests.Utility
{
    internal class EmailVerifier : IDisposable
    {
        // NOTE: the default email sender for Testing is no-reply-linc@uni-plovdiv.bg
        // NOTE: the default email sender for Development is pivanov@uni-plovdiv.bg,
        // because I cannot send mails from home from the above email address
        // NOTE: the default email receiver is pivanov@uni-plovdiv.bg
        // defined in EmailConfig.OverwriteRecipients

        private readonly string _testGMailUserName;
        private readonly string _testGMailUserPassword;

        private const string TestFolderName = "linc-test";

        private IMailFolder _testFolder = null!;

        private readonly ImapClient _client;

        public EmailVerifier(TestConfig testConfig)
        {
            _testGMailUserName = testConfig.TestGMailUserName;
            _testGMailUserPassword = testConfig.TestGMailUserPassword;

            _client = new ImapClient();

            ConnectAsync().Wait();
        }

        private async Task ConnectAsync()
        {
            await _client.ConnectAsync("imap.gmail.com", 993, true);
            await _client.AuthenticateAsync(_testGMailUserName, _testGMailUserPassword);

            _testFolder = await _client.GetFolderAsync(TestFolderName);
            await _testFolder.OpenAsync(FolderAccess.ReadWrite);
        }

        private async Task<UniqueId> GetLastUnseenMessage()
        {
            const int maxRetryTimes = 12;
            var retryWaitPeriod = TimeSpan.FromSeconds(5);
            var retryTimes = 0;

            do
            {
                var query = SearchQuery.Flagged.And(SearchQuery.NotSeen);
                var uniqueIds = await _testFolder.SearchAsync(query);

                if (uniqueIds == null)
                {
                    throw new InvalidOperationException($"Could not retrieve messages from Gmail inbox test folder '{TestFolderName}'.");
                }

                if (uniqueIds.Count > 1)
                {
                    throw new InvalidOperationException($"Retrieved more than 1 messages from Gmail inbox test folder '{TestFolderName}', please clean your test folder.");
                }

                if (uniqueIds.Count == 1)
                {
                    // mark message read and processed

                    var messageId = uniqueIds.Single();

                    await _testFolder.AddFlagsAsync(messageId, MessageFlags.Seen, true);
                    await _testFolder.RemoveFlagsAsync(messageId, MessageFlags.Flagged, true);

                    return messageId;
                }

                retryTimes++;

                if (retryTimes == maxRetryTimes)
                {
                    break;
                }

                await Task.Delay(retryWaitPeriod);

            } while (retryTimes < maxRetryTimes);

            throw new TimeoutException($"Could not retrieve message from Gmail inbox test folder '{TestFolderName}' after {maxRetryTimes} retries (with await time of {retryWaitPeriod:g}).");
        }


        public async Task AssertRegisterConfirmationSent()
        {
            var messageId = await GetLastUnseenMessage();
            var message = await _testFolder.GetMessageAsync(messageId);

            // todo: test this for multiple cultures
            Assert.That(message.Subject.Equals("LInC - Confirm email"));

            // todo verify that there's a link containing /identity/account/confirm-email?
        }


        private async Task DisconnectAsync()
        {
            if (_client.IsConnected)
            {
                await _client.DisconnectAsync(true);
            }
        }

        public void Dispose()
        {
            DisconnectAsync().Wait();

            _client.Dispose();
        }
    }
}
