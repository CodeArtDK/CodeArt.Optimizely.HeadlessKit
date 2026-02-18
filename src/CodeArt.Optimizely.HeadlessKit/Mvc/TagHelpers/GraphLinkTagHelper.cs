using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Renders a <c>&lt;graph-link&gt;</c> element as an HTML <c>&lt;a&gt;</c> tag using the URL
    /// from a <see cref="GraphContentUrl"/>. If the URL is null or empty, renders only the inner
    /// content without a wrapping element.
    /// </summary>
    /// <remarks>
    /// When the <see cref="Content"/> URL is available, the tag helper renders an anchor element
    /// with the child content preserved. When the URL is missing, the wrapper tag is removed and
    /// only the child content is emitted.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <graph-link content="@Model.Link" css-class="btn" target="_blank">Click here</graph-link>
    /// ]]></code>
    /// </example>
    [HtmlTargetElement("graph-link")]
    public class GraphLinkTagHelper : TagHelper
    {
        /// <summary>
        /// The content URL to use as the <c>href</c> for the rendered link.
        /// </summary>
        [HtmlAttributeName("content")]
        public GraphContentUrl? Content { get; set; }

        /// <summary>
        /// CSS class to apply to the rendered <c>&lt;a&gt;</c> element.
        /// </summary>
        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        /// <summary>
        /// Target attribute for the rendered link (e.g., <c>_blank</c>, <c>_self</c>).
        /// </summary>
        [HtmlAttributeName("target")]
        public string? Target { get; set; }

        /// <inheritdoc />
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
