using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace linc.Utility;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        if (value is null)
            return null;

        // Slugify value with a special kebab case function
        return HelperFunctions.ToKebabCase(value.ToString());
    }
}