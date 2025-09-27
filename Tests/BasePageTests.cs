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
        protected IPlaywright Playwright = null!;
        protected IBrowserContext BrowserContext = null!;
        protected IBrowser Browser = null!;

        protected virtual IPage Page { get; set; }

        [OneTimeSetUp]
        public async Task GlobalOneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            Config = configuration.GetSection("TestConfig").Get<TestConfig>() ?? 
                throw new InvalidDataException("Could not load test configuration.");

            // Validate TestConfig data annotations
            Validator.ValidateObject(Config, new ValidationContext(Config), validateAllProperties: true);

            BaseUri = new Uri(Config.ServerBaseUrl);
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500
            });

            var browserContextOptions = new BrowserNewContextOptions();
            if (!string.IsNullOrEmpty(Config.BasicAuthUsername) && !string.IsNullOrEmpty(Config.BasicAuthPassword))
            {
                browserContextOptions.HttpCredentials = new HttpCredentials
                {
                    Username = Config.BasicAuthUsername,
                    Password = Config.BasicAuthPassword
                };
            }

            BrowserContext = await Browser.NewContextAsync(browserContextOptions);
            Page = await BrowserContext.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalOneTimeTearDown()
        {
            await Browser.CloseAsync();
            Playwright.Dispose();
        }
    }
}
