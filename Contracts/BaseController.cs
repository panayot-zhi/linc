using linc.Models.Enumerations;
using linc.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json;

namespace linc.Contracts;

public abstract class BaseController : Controller
{
    protected readonly ILocalizationService LocalizationService;

    protected BaseController(ILocalizationService localizationService)
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