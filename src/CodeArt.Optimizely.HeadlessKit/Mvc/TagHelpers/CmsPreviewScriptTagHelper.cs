using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.TagHelpers
{
    /// <summary>
    /// Injects the Optimizely CMS communication script and contentSaved listener
    /// when in preview or edit mode.
    /// Usage: place <![CDATA[<cms-preview-scripts />]]> in your layout, typically before </body>.
    /// Requires <see cref="ContentGraphOptions.CmsAppUrl"/> to be configured.
    /// </summary>
    [HtmlTargetElement("cms-preview-scripts", TagStructure = TagStructure.WithoutEndTag)]
    public class CmsPreviewScriptTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ContentGraphOptions _options;

        public CmsPreviewScriptTagHelper(IHttpContextAccessor httpContextAccessor, IOptions<ContentGraphOptions> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

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
