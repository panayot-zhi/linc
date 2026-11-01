using System.Net;
using System.Text.RegularExpressions;
using linc.E2ETests.Configuration;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using NUnit.Framework;

namespace linc.E2ETests.Utility
{
    internal class EmailVerifier : IDisposable
    {
        // NOTE: the default email sender for Staging is no-reply-linc@uni-plovdiv.bg
        // NOTE: the default email sender for Development is pivanov@uni-plovdiv.bg,
        // because I cannot send mails from home from the above email address
        // NOTE: the default email receiver is pivanov@uni-plovdiv.bg
        // defined in EmailConfig.OverwriteRecipients, so the original recipient
        // is lost and messages must be matched by a unique token in their body

        private const int MaxRetryTimes = 12;

        private const string TestFolderName = "linc-test";

        private static readonly TimeSpan RetryWaitPeriod = TimeSpan.FromSeconds(5);

        // tolerated difference between the test machine clock and the Gmail receive time
        private static readonly TimeSpan ClockSkewTolerance = TimeSpan.FromMinutes(2);

        private static readonly Regex HrefRegex = new("href=\"([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly string _testGMailUserName;

        private readonly string _testGMailUserPassword;

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

        /// <summary>
        /// Waits for an unread message that contains <paramref name="bodyToken"/> in its body
        /// and was received after <paramref name="sentAfter"/>. Takes the newest match and
        /// marks only that message as read, so unrelated messages in the folder are left alone.
        /// </summary>
        /// <param name="bodyToken">A whole word unique to the test, for example a <see cref="TestRun"/> token.</param>
        /// <param name="sentAfter">The moment the test triggered the email.</param>
        /// <param name="predicate">Optional extra check on the message, for example its subject.</param>
        public async Task<MimeMessage> WaitForMessageAsync(string bodyToken, DateTimeOffset sentAfter,
            Func<MimeMessage, bool>? predicate = null)
        {
            if (string.IsNullOrEmpty(bodyToken))
            {
                throw new ArgumentException("A body token is required.", nameof(bodyToken));
            }

            var receivedAfter = sentAfter - ClockSkewTolerance;

            // IMAP SINCE only has day granularity, the exact time is checked below
            var query = SearchQuery.NotSeen
                .And(SearchQuery.BodyContains(bodyToken))
                .And(SearchQuery.DeliveredAfter(receivedAfter.UtcDateTime.Date.AddDays(-1)));

            for (var retryTimes = 1; retryTimes <= MaxRetryTimes; retryTimes++)
            {
                // let the server report messages
                // that arrived since the last command
                await _client.NoOpAsync();

                var uniqueIds = await _testFolder.SearchAsync(query);
                if (uniqueIds.Count > 0)
                {
                    var summaries = await _testFolder.FetchAsync(uniqueIds,
                        MessageSummaryItems.UniqueId | MessageSummaryItems.InternalDate);

                    var candidates = summaries
                        .Where(x => x.InternalDate >= receivedAfter)
                        .OrderByDescending(x => x.InternalDate);

                    foreach (var candidate in candidates)
                    {
                        var message = await _testFolder.GetMessageAsync(candidate.UniqueId);
                        if (predicate != null && !predicate(message))
                        {
                            continue;
                        }

                        // mark message read and processed
                        await _testFolder.AddFlagsAsync(candidate.UniqueId, MessageFlags.Seen, true);
                        await _testFolder.RemoveFlagsAsync(candidate.UniqueId, MessageFlags.Flagged, true);

                        return message;
                    }
                }

                if (retryTimes < MaxRetryTimes)
                {
                    await Task.Delay(RetryWaitPeriod);
                }
            }

            throw new TimeoutException($"Could not find an unread message containing '{bodyToken}' received after {sentAfter:u} " +
                                       $"in Gmail folder '{TestFolderName}' after {MaxRetryTimes} retries (with await time of {RetryWaitPeriod:g}).");
        }

        /// <summary>
        /// Asserts that a registration confirmation email reached the user whose names contain
        /// <paramref name="namesToken"/> and returns the confirmation link from it.
        /// </summary>
        public async Task<Uri> AssertRegisterConfirmationSent(string culture, string namesToken, DateTimeOffset sentAfter)
        {
            var expectedSubject = SiteText.EmailSubject("Email_ConfirmEmail_Subject", culture);

            var message = await WaitForMessageAsync(namesToken, sentAfter,
                x => x.Subject == expectedSubject);

            Assert.That(message.Subject, Is.EqualTo(expectedSubject));

            var confirmationLink = FindLink(message, "/identity/account/confirm-email");
            Assert.That(confirmationLink, Is.Not.Null, "The confirmation email does not contain a confirmation link.");

            return confirmationLink!;
        }

        /// <summary>
        /// Returns the first link in the HTML body whose address contains <paramref name="pathFragment"/>.
        /// </summary>
        public static Uri? FindLink(MimeMessage message, string pathFragment)
        {
            var html = message.HtmlBody ?? string.Empty;

            return HrefRegex.Matches(html)
                .Select(x => WebUtility.HtmlDecode(x.Groups[1].Value))
                .Where(x => x.Contains(pathFragment, StringComparison.OrdinalIgnoreCase))
                .Select(x => Uri.TryCreate(x, UriKind.Absolute, out var uri) ? uri : null)
                .FirstOrDefault(x => x != null);
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
