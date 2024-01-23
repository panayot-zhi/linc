using Microsoft.AspNetCore.Diagnostics;

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
}