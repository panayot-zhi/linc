using linc.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace linc.Utility
{
    [HtmlTargetElement("*", Attributes = DatabaseLocalizationKeyName)]
    public class DatabaseLocalizationTagHelper : TagHelper
    {
        private const string DatabaseLocalizationKeyName = "contenteditable-for";

        [HtmlAttributeName(DatabaseLocalizationKeyName)]
        public string DatabaseLocalizationKey { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private readonly ILocalizationService _localizationService;

        public DatabaseLocalizationTagHelper(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("id", DatabaseLocalizationKey);
            if (ViewContext.HttpContext.User.IsAtLeast(SiteRole.Editor))
            {
                output.Attributes.Add("contenteditable", "true");
            }

            var resource = _localizationService[DatabaseLocalizationKey];
            if (resource.IsResourceNotFound)
            {
                output.Content.SetHtmlContent("Lorem ipsum dolor sit amet");
            }
            else
            {
                output.Content.SetHtmlContent(resource);
            }
        }
    }
}
