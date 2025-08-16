using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using linc.Contracts;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Primitives;

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

    public static bool HasProperty(this EntityEntry entry, string property)
    {
        return entry.Properties.Any(x => x.Metadata.Name == property);
    }

    public static string GetFullPath(this IStatusCodeReExecuteFeature me)
    {
        return $"{me.OriginalPathBase}{me.OriginalPath}{me.OriginalQueryString}";
    }

    public static string GetIp(HttpContext httpContext)
    {
        var result = string.Empty;

        // first try to get IP address from the forwarded header
        if (httpContext.Request.Headers.Any())
        {
            // the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
            // connecting to a web server through an HTTP proxy or load balancer

            var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"];
            if (!StringValues.IsNullOrEmpty(forwardedHeader))
            {
                result = forwardedHeader.FirstOrDefault();
            }
        }

        // if this header not exists try get connection remote IP address
        if (string.IsNullOrEmpty(result) && httpContext.Connection.RemoteIpAddress != null)
        {
            result = httpContext.Connection.RemoteIpAddress.ToString();
        }

        return result;
    }

    internal static string ToKebabCase(string input)
    {
        if (input == null)
        {
            return null;
        }

        return Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLower();
    }

    public static string ToSnakeCase(string input)
    {
        // Return the original string if it's null
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Return the lowercased version if
        // the string has less than 2 characters
        if (input.Length < 2)
        {
            return input.ToLowerInvariant();
        }

        var builder = new StringBuilder();

        // Append the first character in
        // lowercase without adding an underscore
        builder.Append(char.ToLowerInvariant(input[0]));

        // Process the rest of the characters
        // starting from the second one
        for (var i = 1; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                // Check for word boundaries:
                // 1. Previous character is lowercase or
                // 2. Next character is lowercase (typical for camelCase or end of an acronym)
                if (char.IsLower(input[i - 1]) || (i + 1 < input.Length && char.IsLower(input[i + 1])))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    public static string ToScreamingSnakeCase(string input)
    {
        return ToSnakeCase(input).ToUpperInvariant();
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

        var alertMessage = JsonSerializer.Serialize(new
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