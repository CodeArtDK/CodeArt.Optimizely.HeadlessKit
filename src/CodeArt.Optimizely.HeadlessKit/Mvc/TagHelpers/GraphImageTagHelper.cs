using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Renders a <c>&lt;graph-image&gt;</c> element as an HTML <c>&lt;img&gt;</c> tag using the URL
    /// from a <see cref="GraphContentReference"/>. Suppresses output if the image URL is null or empty.
    /// </summary>
    /// <remarks>
    /// The tag helper resolves the image URL from <see cref="Content"/>.<c>Url.Default</c>.
    /// If no URL is available, no HTML is rendered. An empty <c>alt</c> attribute is always emitted for accessibility.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <graph-image content="@Model.Image" width="800" height="600" alt="Hero" css-class="hero-img" />
    /// ]]></code>
    /// </example>
    [HtmlTargetElement("graph-image")]
    public class GraphImageTagHelper : TagHelper
    {
        /// <summary>
        /// The image content reference containing the URL to render.
        /// </summary>
        [HtmlAttributeName("content")]
        public GraphContentReference? Content { get; set; }

        /// <summary>
        /// Optional width attribute for the rendered <c>&lt;img&gt;</c> element.
        /// </summary>
        [HtmlAttributeName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Optional height attribute for the rendered <c>&lt;img&gt;</c> element.
        /// </summary>
        [HtmlAttributeName("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Alt text for the image. If not specified, an empty alt attribute is rendered.
        /// </summary>
        [HtmlAttributeName("alt")]
        public string? Alt { get; set; }

        /// <summary>
        /// CSS class to apply to the rendered <c>&lt;img&gt;</c> element.
        /// </summary>
        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        /// <inheritdoc />
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
