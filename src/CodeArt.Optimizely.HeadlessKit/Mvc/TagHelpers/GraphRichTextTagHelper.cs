using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Renders a <c>&lt;graph-rich-text&gt;</c> element as raw HTML from a <see cref="GraphContentRichText"/>.
    /// The tag element is replaced entirely with the HTML content.
    /// </summary>
    /// <remarks>
    /// The tag helper removes the wrapper element and outputs the raw HTML from the rich text content.
    /// If the content is null or empty, nothing is rendered.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <graph-rich-text content="@Model.Body" />
    /// ]]></code>
    /// </example>
    [HtmlTargetElement("graph-rich-text")]
    public class GraphRichTextTagHelper : TagHelper
    {
        /// <summary>
        /// The rich text content containing the HTML to render.
        /// </summary>
        [HtmlAttributeName("content")]
        public GraphContentRichText? Content { get; set; }

        /// <inheritdoc />
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
