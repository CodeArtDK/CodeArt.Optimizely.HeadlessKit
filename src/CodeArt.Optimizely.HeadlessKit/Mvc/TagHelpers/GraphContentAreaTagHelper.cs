using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Renders a <c>&lt;graph-content-area&gt;</c> element by invoking the
    /// <see cref="Components.CompositionRendererViewComponent"/> to render a composition tree.
    /// Output is wrapped in a <c>&lt;div&gt;</c> element. Suppresses output if the composition has no nodes.
    /// </summary>
    /// <remarks>
    /// The wrapper <c>&lt;div&gt;</c> receives the CSS class specified by <see cref="CssClass"/>,
    /// defaulting to <c>"opti-content-area"</c> if not provided.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <graph-content-area composition="@Model.CurrentContent?.Composition" css-class="main-area" />
    /// ]]></code>
    /// </example>
    [HtmlTargetElement("graph-content-area")]
    public class GraphContentAreaTagHelper : TagHelper
    {
        private readonly IViewComponentHelper _viewComponentHelper;

        /// <summary>
        /// The composition tree to render.
        /// </summary>
        [HtmlAttributeName("composition")]
        public ContentComposition? Composition { get; set; }

        /// <summary>
        /// CSS class for the wrapper <c>&lt;div&gt;</c>. Defaults to <c>"opti-content-area"</c> if not specified.
        /// </summary>
        [HtmlAttributeName("css-class")]
        public string? CssClass { get; set; }

        /// <summary>
        /// The current <see cref="ViewContext"/>. Automatically populated by the framework.
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphContentAreaTagHelper"/> class.
        /// </summary>
        /// <param name="viewComponentHelper">The view component helper used to invoke the composition renderer.</param>
        public GraphContentAreaTagHelper(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        /// <inheritdoc />
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
