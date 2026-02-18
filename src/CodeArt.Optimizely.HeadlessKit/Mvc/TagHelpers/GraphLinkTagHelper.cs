using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    [HtmlTargetElement("graph-link")]
    public class GraphLinkTagHelper : TagHelper
    {
        [HtmlAttributeName("content")]
        public GraphContentUrl? Content { get; set; }

        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        [HtmlAttributeName("target")]
        public string? Target { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var url = Content?.Default;
            if (string.IsNullOrWhiteSpace(url))
            {
                output.TagName = null;
                var childContent = await output.GetChildContentAsync();
                output.Content.SetHtmlContent(childContent);
                return;
            }

            output.TagName = "a";
            output.Attributes.SetAttribute("href", url);

            if (!string.IsNullOrWhiteSpace(CssClass))
                output.Attributes.SetAttribute("class", CssClass);
            if (!string.IsNullOrWhiteSpace(Target))
                output.Attributes.SetAttribute("target", Target);
        }
    }
}
