using linc.Data;
using Microsoft.AspNetCore.Mvc.Localization;

namespace linc.Contracts
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>The string resource as a <see cref="LocalizedHtmlString"/>.</returns>
        LocalizedHtmlString this[string name] { get; }

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments. The arguments will
        /// be HTML encoded.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource as a <see cref="LocalizedHtmlString"/>.</returns>
        LocalizedHtmlString this[string name, params object[] arguments] { get; }

        Task SetStringResource(string resourceKey, string value, string userId);
    }
}
