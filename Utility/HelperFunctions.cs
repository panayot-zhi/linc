using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using linc.Contracts;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace linc.Utility;

public static class HelperFunctions
{
    private static readonly Random RandomNumberGenerator = new();

    public static string GatherInternals(this Exception ex, int introspectionLevel = 3)
    {
        if (ex == null)
        {
            return string.Empty;
        }

        if (introspectionLevel == 0)
        {
            return "...";
        }

        return ex.Message + " > " + GatherInternals(ex.InnerException, --introspectionLevel);
    }

    public static string GetFullPath(this IStatusCodeReExecuteFeature me)
    {
        return $"{me.OriginalPathBase}{me.OriginalPath}{me.OriginalQueryString}";
    }

    public static bool HasProperty(this EntityEntry entry, string property)
    {
        return entry.Properties.Any(x => x.Metadata.Name == property);
    }

    public static string Extension(this IFormFile formFile)
    {
        return GetFileExtension(formFile.FileName);
    }

    public static bool IsAjax(this HttpRequest request, string httpVerb = "")
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!string.IsNullOrEmpty(httpVerb))
        {
            if (request.Method != httpVerb)
            {
                return false;
            }
        }

        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    internal static string Md5Hash(string input)
    {
        using (var md5 = MD5.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);

            var builder = new StringBuilder();
            foreach (var c in hash)
            {
                builder.Append(c.ToString("X2"));
            }

            return builder.ToString();
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = RandomNumberGenerator.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        return fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
    }

    public static string GetMimeType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }


    public static T Get<T>(this ViewDataDictionary viedDataDictionary, string key)
    {
        if(viedDataDictionary.TryGetValue(key, out var value))
        {
            return (T) value;
        }

        return default;
    }

    public static void AddIdentityError(this ModelStateDictionary modelState, IdentityError error)
    {
        modelState.AddModelError(error.Code, error.Description);
    }

    public static void AddIdentityErrors(this ModelStateDictionary modelState, IEnumerable<IdentityError> errors)
    {
        foreach (var identityError in errors)
        {
            modelState.AddIdentityError(identityError);
        }
    }

    public static void AddAlertMessage(
        ITempDataDictionary tempData,
        ILocalizationService localizationService,
        string message,
        string title = "",
        string footer = "",
        string position = "center",
        string confirmButtonText = "OK",
        AlertMessageType type = AlertMessageType.Info)
    {
        if (string.IsNullOrEmpty(title))
        {
            title = localizationService[$"AlertMessage_{type}_Title"].Value;
        }

        if (type == AlertMessageType.Error && string.IsNullOrEmpty(footer))
        {
            footer = localizationService["AlertMessage_Error_Footer", 
                SiteConstant.AdministratorEmail].Value;
        } 
        else if (type == AlertMessageType.Warning && string.IsNullOrEmpty(footer))
        {
            footer = localizationService["AlertMessage_Warning_Footer", 
                SiteConstant.AdministratorEmail].Value;
        }

        var alertMessage = JsonConvert.SerializeObject(new
        {
            html = message,
            icon = type.ToString().ToLowerInvariant(),
            title,
            footer,
            position,
            confirmButtonText
        });

        tempData["AlertMessage"] = alertMessage;
    }
}