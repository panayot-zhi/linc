using Microsoft.AspNetCore.Diagnostics;

namespace linc.Utility;

public static class HelperFunctions
{
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

    public static bool IsAjax(this HttpRequest request, string httpVerb = "")
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "Request object is Null.");
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

}