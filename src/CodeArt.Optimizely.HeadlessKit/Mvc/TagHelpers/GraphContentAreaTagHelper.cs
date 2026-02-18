using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    [HtmlTargetElement("graph-content-area")]
    public class GraphContentAreaTagHelper : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;

        [HtmlAttributeName("composition")]
        public ContentComposition? Composition { get; set; }

        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public GraphContentAreaTagHelper(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Composition?.Nodes == null || Composition.Nodes.Count == 0)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (!string.IsNullOrWhiteSpace(CssClass))
                output.Attributes.SetAttribute("class", CssClass);
            else
                output.Attributes.SetAttribute("class", "opti-content-area");

            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            var content = await _viewComponentHelper.InvokeAsync("CompositionRenderer", new { composition = Composition });
            output.Content.SetHtmlContent(content);
        }
    }
}
