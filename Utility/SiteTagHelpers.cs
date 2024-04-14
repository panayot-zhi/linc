using System.Text.Encodings.Web;
using linc.Contracts;
using linc.Models.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;

namespace linc.Utility
{
    [HtmlTargetElement("*", Attributes = DatabaseLocalizationKeyName)]
    public class DatabaseLocalizationTagHelper : TagHelper
    {
        private const string DatabaseLocalizationKeyName = "contenteditable-for";
        private const string AllowedName = "contenteditable-allowed-for";

        [HtmlAttributeName(DatabaseLocalizationKeyName)]
        public string DatabaseLocalizationKey { get; set; }

        [HtmlAttributeName(AllowedName)]
        public SiteRole? Allowed { get; set; }

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

            if (!Allowed.HasValue)
            {
                Allowed = SiteRole.Editor;
            }

            if (ViewContext.HttpContext.User.IsAtLeast(Allowed.Value) && IsEditing())
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

        private bool IsEditing()
        {
            return ViewContext.TempData.ContainsKey(SiteConstant.TempDataEditableKey);
        }
    }

    [HtmlTargetElement("th", Attributes = TableColumnForAttributeName)]
    public class SortableTableColumnTagHelper : TagHelper
    {
        private const string TableColumnForAttributeName = "sortable-for";
        private const string OrderQueryStringParameterName = "order";
        private const string DescendingOrderQueryStringParameterValue = "desc";
        private const string SortQueryStringParameterName = "sort";

        [HtmlAttributeName(TableColumnForAttributeName)]
        public string PropertyName { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private SiteSortOrder ResolveSortOrder(string name)
        {
            var query = ViewContext.HttpContext.Request.Query;
            var sort = query[SortQueryStringParameterName].FirstOrDefault();
            if (sort == null || sort != name)
            {
                return SiteSortOrder.Unspecified;
            }

            var order = query[OrderQueryStringParameterName].FirstOrDefault();

            return order == DescendingOrderQueryStringParameterValue ?
                SiteSortOrder.Desc : SiteSortOrder.Asc; // the default order is ascending
        }

        private string ResolveUrl(string name, SiteSortOrder sortOrder)
        {
            var request = ViewContext.HttpContext.Request;
            var currentQueryString = request.Query;
            var dictionary = new Dictionary<string, string>();

            switch (sortOrder)
            {
                case SiteSortOrder.Unspecified:
                    // do not add keys when no sort
                    break;

                default:
                    dictionary.Add(
                        key: SortQueryStringParameterName,
                        value: name
                    );

                    dictionary.Add(
                        key: OrderQueryStringParameterName,
                        value: sortOrder.ToString().ToLowerInvariant()
                    );
                    break;
            }

            // gather existing query key:values
            foreach (var item in currentQueryString)
            {
                // add these only once
                if (item.Key is SortQueryStringParameterName or OrderQueryStringParameterName)
                {
                    continue;
                }

                dictionary.TryAdd(item.Key, item.Value);
            }

            // construct query string
            return QueryHelpers.AddQueryString($"{request.PathBase}{request.Path}", dictionary);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var aBuilder = new TagBuilder("a");
            var sortOrder = ResolveSortOrder(PropertyName);
            output.AddClass("sortable", HtmlEncoder.Default);

            switch (sortOrder)
            {
                case SiteSortOrder.Unspecified:
                    aBuilder.Attributes.Add("href", ResolveUrl(PropertyName, SiteSortOrder.Asc));
                    break;
                case SiteSortOrder.Asc:
                    aBuilder.Attributes.Add("href", ResolveUrl(PropertyName, SiteSortOrder.Desc));
                    output.AddClass("desc", HtmlEncoder.Default);
                    break;
                case SiteSortOrder.Desc:
                    aBuilder.Attributes.Add("href", ResolveUrl(PropertyName, SiteSortOrder.Unspecified));
                    output.AddClass("asc", HtmlEncoder.Default);
                    break;
                default:
                    aBuilder.Attributes.Add("href", "#");
                    break;
            }

            output.PreContent.SetHtmlContent(aBuilder.RenderStartTag());
            output.PostContent.SetHtmlContent(aBuilder.RenderEndTag());
        }
    }

}
