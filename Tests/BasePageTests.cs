using linc.E2ETests.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace linc.E2ETests
{
    public abstract class BasePageTests
    {
        protected Uri BaseUri = null!;
        protected TestConfig Config = null!;
        protected IBrowserContext BrowserContext = null!;

        protected virtual IPage Page { get; set; } = null!;

        private IPlaywright? _playwright;
        private IBrowser? _browser;

        [OneTimeSetUp]
        public async Task GlobalOneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            // load test configuration to object instance of TestConfig
            Config = configuration.GetSection("TestConfig").Get<TestConfig>() ??
                throw new InvalidDataException("Could not load test configuration.");

            // validate TestConfig data annotations
            Validator.ValidateObject(Config, new ValidationContext(Config), validateAllProperties: true);

            // initialize common configuration
            BaseUri = new Uri(Config.ServerBaseUrl);

            // define private internal fields
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                // NOTE: use these for debugging locally
                // Headless = false,
                // SlowMo = 500
            });

            // define usable protected properties for children pages
            var browserContextOptions = new BrowserNewContextOptions();
            if (!string.IsNullOrEmpty(Config.BasicAuthUsername) && !string.IsNullOrEmpty(Config.BasicAuthPassword))
            {
                browserContextOptions.HttpCredentials = new HttpCredentials
                {
                    Username = Config.BasicAuthUsername,
                    Password = Config.BasicAuthPassword
                };
            }

            BrowserContext = await _browser.NewContextAsync(browserContextOptions);
            Page = await BrowserContext.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalOneTimeTearDown()
        {
            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }

            _playwright?.Dispose();
        }
    }
}
