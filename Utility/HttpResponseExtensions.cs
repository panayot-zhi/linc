using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace linc.Utility
{
    public static class HttpResponseExtensions
    {
        public static void SetCurrentLanguage(this HttpResponse response, string culture)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            response.Cookies.Append(
                SiteCookieName.Language,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(7)
                }
            );
        }

        public static void SetCurrentLanguage(this HttpResponse response, int languageId)
        {
            var culture = SiteConstant.SupportedCultures[languageId];
            SetCurrentLanguage(response, culture);
        }
    }
}
