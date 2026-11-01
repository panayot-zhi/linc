using System.Collections.Concurrent;
using linc.E2ETests.Configuration;
using linc.E2ETests.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace linc.E2ETests
{
    public abstract class TestBase
    {
        protected Uri BaseUri = null!;
        protected TestConfig Config = null!;
        protected IBrowserContext BrowserContext = null!;

        protected virtual IPage Page { get; set; } = null!;

        private IPlaywright? _playwright;
        private IBrowser? _browser;

        // every context opened by this fixture, closed in the tear down
        private readonly List<IBrowserContext> _contexts = new();

        // one login per role for the whole test run, shared between fixtures
        private static readonly ConcurrentDictionary<TestRole, string> StorageStates = new();

        [OneTimeSetUp]
        public async Task GlobalOneTimeSetUp()
        {
            // locally the values come from appsettings.Staging.json (gitignored),
            // in CI the file does not exist and the workflow passes TestConfig__* environment variables
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Staging.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            // load test configuration to object instance of TestConfig
            Config = configuration.GetSection("TestConfig").Get<TestConfig>() ??
                throw new InvalidDataException("Could not load test configuration: " +
                    "add a TestConfig section to appsettings.Staging.json in the repository root, " +
                    "or pass TestConfig__* environment variables.");

            // validate TestConfig data annotations
            Validator.ValidateObject(Config, new ValidationContext(Config), validateAllProperties: true);

            // initialize common configuration
            BaseUri = new Uri(Config.ServerBaseUrl);

            // define private internal fields
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Config.PlaywrightHeadlessMode,
                SlowMo = Config.PlaywrightSlowMo
            });

            // define usable protected properties for children pages
            BrowserContext = await NewContextAsync();
            Page = await BrowserContext.NewPageAsync();
        }

        /// <summary>
        /// Absolute address on the test server for a site relative path.
        /// </summary>
        protected string Url(string relativePath) => new Uri(BaseUri, relativePath).ToString();

        /// <summary>
        /// A new, isolated browser context (own cookies), with basic auth when configured.
        /// </summary>
        protected async Task<IBrowserContext> NewContextAsync(string? storageState = null)
        {
            var options = new BrowserNewContextOptions
            {
                StorageState = storageState,

                // the site picks its language from the Accept-Language header until a culture cookie is set;
                // pin it to the site default, so lists filtered by language (library, author search) are stable
                Locale = "bg-BG"
            };

            if (!string.IsNullOrEmpty(Config.BasicAuthUsername) && !string.IsNullOrEmpty(Config.BasicAuthPassword))
            {
                options.HttpCredentials = new HttpCredentials
                {
                    Username = Config.BasicAuthUsername,
                    Password = Config.BasicAuthPassword
                };
            }

            var context = await _browser!.NewContextAsync(options);
            _contexts.Add(context);
            return context;
        }

        /// <summary>
        /// A page in a fresh context without any login.
        /// </summary>
        protected async Task<IPage> OpenAnonymousAsync()
        {
            var context = await NewContextAsync();
            return await context.NewPageAsync();
        }

        /// <summary>
        /// A page logged in as one of the seeded privileged accounts.
        /// The test is skipped when the account password is not configured.
        /// </summary>
        protected async Task<IPage> OpenAsAsync(TestRole role)
        {
            var account = TestAccount.For(Config, role);
            if (string.IsNullOrEmpty(account.Password))
            {
                Assert.Ignore($"No password configured for the seeded {role} account " +
                              $"(TestConfig:{role}Password), the test needs it.");
            }

            if (StorageStates.TryGetValue(role, out var storageState))
            {
                var cachedContext = await NewContextAsync(storageState);
                return await cachedContext.NewPageAsync();
            }

            var context = await NewContextAsync();
            var page = await context.NewPageAsync();
            await LoginAsync(page, account.UserName, account.Password!);

            StorageStates[role] = await context.StorageStateAsync();
            return page;
        }

        protected async Task LoginAsync(IPage page, string userName, string password)
        {
            await page.GotoAsync(Url("/identity/account/login"));
            await page.FillAsync("input[name='Input.UserName']", userName);
            await page.FillAsync("input[name='Input.Password']", password);
            await page.ClickAsync("button#login-submit");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            if (new Uri(page.Url).AbsolutePath.EndsWith("/account/login", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail($"Could not log in as '{userName}'. Check the seeded accounts and their passwords.");
            }
        }

        /// <summary>
        /// Registers a plain user through the UI in a fresh context and keeps it signed in.
        /// The user is signed in right after registration, without confirming the email.
        /// The token is a separate word in the last name, so emails to this user can be found.
        /// </summary>
        protected async Task<RegisteredUser> RegisterUserAsync(string firstName, string lastNamePrefix,
            string? email = null, string culture = "en")
        {
            var token = TestRun.NewToken();
            var page = await OpenAnonymousAsync();
            await SetPageCulture(page, culture);

            var user = new RegisteredUser(
                Token: token,
                FirstName: firstName,
                LastName: $"{lastNamePrefix} {token}",
                Email: email ?? $"{token}@test.com",
                UserName: $"user-{token}",
                Password: $"Test{token}!",
                RegisteredAt: DateTimeOffset.UtcNow,
                Page: page);

            await page.GotoAsync(Url("/identity/account/register"));
            await page.FillAsync("input[name='Input.FirstName']", user.FirstName);
            await page.FillAsync("input[name='Input.LastName']", user.LastName);
            await page.FillAsync("input[name='Input.UserName']", user.UserName);
            await page.FillAsync("input[name='Input.Email']", user.Email);
            await page.FillAsync("input[name='Input.Password']", user.Password);
            await page.FillAsync("input[name='Input.ConfirmPassword']", user.Password);
            await page.CheckAsync("input[name='Input.PrivacyConsent']");
            await page.ClickAsync("button#registerSubmit");

            await ConfirmSwalAlert(page);
            return user;
        }

        protected async Task SetPageCulture(string culture) => await SetPageCulture(Page, culture);

        protected async Task SetPageCulture(IPage page, string culture)
        {
            await page.GotoAsync(BaseUri.ToString());
            await page.ClickAsync($"button#language_{culture}");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        protected async Task ConfirmSwalAlert() => await ConfirmSwalAlert(Page);

        protected static async Task ConfirmSwalAlert(IPage page)
        {
            await page.ClickAsync("button.swal2-confirm");
        }

        protected async Task DismissSwalAlert()
        {
            await Page.ClickAsync("button.swal2-cancel");
        }

        /// <summary>
        /// Waits for a SweetAlert popup and returns its icon type, such as "success" or "warning".
        /// </summary>
        protected static async Task<string> GetSwalIconAsync(IPage page)
        {
            var icon = page.Locator(".swal2-popup .swal2-icon.swal2-icon-show");
            await icon.WaitForAsync();

            var classes = await icon.GetAttributeAsync("class") ?? string.Empty;
            foreach (var type in new[] { "success", "warning", "error", "info", "question" })
            {
                if (classes.Split(' ').Contains($"swal2-{type}"))
                {
                    return type;
                }
            }

            return classes;
        }

        protected async Task Logout()
        {
            await Page.ClickAsync("a[href='#logoutForm']");
        }

        /// <summary>
        /// Path of the page the browser ended on, lower case, without the query string.
        /// </summary>
        protected static string CurrentPath(IPage page) => new Uri(page.Url).AbsolutePath.ToLowerInvariant();

        protected static string FixturePath(string fileName) =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", fileName);

        [OneTimeTearDown]
        public async Task GlobalOneTimeTearDown()
        {
            foreach (var context in _contexts)
            {
                await context.CloseAsync();
            }

            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();
        }
    }
}
