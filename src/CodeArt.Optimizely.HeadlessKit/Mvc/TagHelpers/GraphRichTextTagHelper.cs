using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    [HtmlTargetElement("graph-rich-text")]
    public class GraphRichTextTagHelper : TagHelper
    {
        [HtmlAttributeName("content")]
        public GraphContentRichText? Content { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            if (Content != null && !string.IsNullOrWhiteSpace(Content.Html))
            {
                output.Content.SetHtmlContent(Content.Html);
            }
        }
    }
}
