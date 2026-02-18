using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    [HtmlTargetElement("graph-image")]
    public class GraphImageTagHelper : TagHelper
    {
        [HtmlAttributeName("content")]
        public GraphContentReference? Content { get; set; }

        [HtmlAttributeName("width")]
        public int? Width { get; set; }

        [HtmlAttributeName("height")]
        public int? Height { get; set; }

        [HtmlAttributeName("alt")]
        public string? Alt { get; set; }

        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = Content?.Url?.Default;
            if (string.IsNullOrWhiteSpace(url))
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.SetAttribute("src", url);

            if (!string.IsNullOrWhiteSpace(Alt))
                output.Attributes.SetAttribute("alt", Alt);
            else
                output.Attributes.SetAttribute("alt", "");

            if (Width.HasValue)
                output.Attributes.SetAttribute("width", Width.Value);
            if (Height.HasValue)
                output.Attributes.SetAttribute("height", Height.Value);
            if (!string.IsNullOrWhiteSpace(CssClass))
                output.Attributes.SetAttribute("class", CssClass);
        }
    }
}
