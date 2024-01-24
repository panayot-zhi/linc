using linc.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("id", DatabaseLocalizationKey);

            //var editable = ViewContext.ViewData.Get<bool?>("Editable") == true;
            var editable = ViewContext.TempData.ContainsKey("Editable");
            if (ViewContext.HttpContext.User.IsAtLeast(SiteRole.Editor) && editable)
            {
                output.Attributes.Add("contenteditable", "true");
                
                var classAttributeValue = "content-editable";
                var classAttribute = output.Attributes["class"];
                if (classAttribute is { Value: { } })
                {
                    classAttributeValue += " " + classAttribute.Value;
                    output.Attributes.SetAttribute("class", classAttributeValue);
                }
                else
                {
                    output.Attributes.Add("class", classAttributeValue);
                }
            }

            var resource = _localizationService[DatabaseLocalizationKey];
            if (!resource.IsResourceNotFound)
            {
                output.Content.SetHtmlContent(resource);
                return;
                
            }

            if (!output.Content.IsEmptyOrWhiteSpace)
            {
                 await base.ProcessAsync(context, output);
                 return;
            }

            var childContent = await output.GetChildContentAsync();
            if (!childContent.IsEmptyOrWhiteSpace)
            {
                await base.ProcessAsync(context, output);
                return;
            }

            // we need some content after all, fill some gibberish
            output.Content.SetHtmlContent("Lorem ipsum dolor sit amet");
        }
    }
}
