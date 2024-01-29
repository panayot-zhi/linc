using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace linc.Contracts;

public abstract class BasePageModel : PageModel
{
    protected readonly ILocalizationService LocalizationService;

    protected BasePageModel(ILocalizationService localizationService)
    {
        LocalizationService = localizationService;
    }

    protected void AddAlertMessage(LocalizedHtmlString message,
        string title = "",
        string footer = "",
        string position = "center",
        string confirmButtonText = "OK",
        AlertMessageType type = AlertMessageType.Info) =>
        AddAlertMessage(message.Value,
            title,
            footer,
            position,
            confirmButtonText,
            type);

    protected void AddAlertMessage(string message,
        string title = "",
        string footer = "",
        string position = "center",
        string confirmButtonText = "OK",
        AlertMessageType type = AlertMessageType.Info) =>
        HelperFunctions.AddAlertMessage(TempData, LocalizationService,
            message, title, footer, position, confirmButtonText, type);
}