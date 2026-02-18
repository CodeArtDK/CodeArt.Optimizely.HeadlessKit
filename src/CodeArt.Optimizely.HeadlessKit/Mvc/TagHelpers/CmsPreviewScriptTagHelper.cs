using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Injects the Optimizely CMS communication script and a <c>contentSaved</c> event listener
    /// when the current request is in preview or edit mode.
    /// </summary>
    /// <remarks>
    /// Only renders output when the request is in preview mode and
    /// <see cref="ContentGraphOptions.CmsAppUrl"/> is configured. When active, it injects the
    /// CMS <c>communicationinjector.js</c> script and a listener that reloads the page on content save.
    /// Place this tag in your layout, typically before the closing <c>&lt;/body&gt;</c> tag.
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// <cms-preview-scripts />
    /// ]]></code>
    /// </example>
    [HtmlTargetElement("cms-preview-scripts", TagStructure = TagStructure.WithoutEndTag)]
    public class CmsPreviewScriptTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ContentGraphOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmsPreviewScriptTagHelper"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
        /// <param name="options">The content graph options containing the CMS app URL.</param>
        public CmsPreviewScriptTagHelper(IHttpContextAccessor httpContextAccessor, IOptions<ContentGraphOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null
                || httpContext.Items["PreviewMode"] == null
                || string.IsNullOrEmpty(_options.CmsAppUrl))
            {
                output.SuppressOutput();
                return;
            }

            var cmsAppUrl = _options.CmsAppUrl.TrimEnd('/');

            output.TagName = null;
            output.Content.AppendHtml(
                $"<script src=\"{cmsAppUrl}/util/javascript/communicationinjector.js\"></script>\n");
            output.Content.AppendHtml(
@"<script>
window.addEventListener('optimizely:cms:contentSaved', function (event) {
    if (event.detail && event.detail.previewUrl) {
        window.location.href = event.detail.previewUrl;
    } else {
        window.location.reload();
    }
});
</script>
");
        }
    }
}
