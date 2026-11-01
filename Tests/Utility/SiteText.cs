using System.Globalization;
using System.Resources;

namespace linc.E2ETests.Utility
{
    /// <summary>
    /// The app's own texts (Resources/SharedResource*.resx, linked into this project),
    /// so tests compare against what the site shows instead of copies of it.
    /// The neutral file is Bulgarian, SharedResource.en.resx is English; like the site,
    /// any other culture falls back to Bulgarian.
    /// </summary>
    public static class SiteText
    {
        private static readonly ResourceManager Resources =
            new("linc.E2ETests.Resources.SharedResource", typeof(SiteText).Assembly);

        public static string Get(string name, string culture) =>
            Resources.GetString(name, CultureInfo.GetCultureInfo(culture))
            ?? throw new KeyNotFoundException($"Resource '{name}' was not found in SharedResource.");

        /// <summary>
        /// An email subject as SiteEmailSender sends it: "{Logo_Short} - {subject}".
        /// </summary>
        public static string EmailSubject(string subjectKey, string culture) =>
            $"{Get("Logo_Short", culture)} - {Get(subjectKey, culture)}";

        /// <summary>
        /// A browser tab title as the layout renders it: "{page title} - linc".
        /// </summary>
        public static string PageTitle(string titleKey, string culture) =>
            $"{Get(titleKey, culture)} - linc";
    }
}
