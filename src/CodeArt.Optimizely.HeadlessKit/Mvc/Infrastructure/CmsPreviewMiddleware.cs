using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    /// <summary>
    /// Middleware that allows the Optimizely CMS editor to embed the site in an iframe for live preview.
    /// </summary>
    /// <remarks>
    /// On <c>/preview</c> requests, this middleware removes the <c>X-Frame-Options</c> header and adds
    /// a <c>Content-Security-Policy</c> <c>frame-ancestors</c> header that permits the CMS application
    /// (configured via <see cref="ContentGraphOptions.CmsAppUrl"/>) to embed the site in an iframe.
    /// This is only active when <see cref="ContentGraphOptions.CmsAppUrl"/> is configured.
    /// </remarks>
    public class CmsPreviewMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ContentGraphOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmsPreviewMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The content graph options containing the CMS app URL.</param>
        public CmsPreviewMiddleware(RequestDelegate next, IOptions<ContentGraphOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        /// <summary>
        /// Processes the request. On <c>/preview</c> requests, adjusts security headers to allow iframe embedding.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task Invoke(HttpContext context)
        {
            if (!string.IsNullOrEmpty(_options.CmsAppUrl)
                && context.Request.Path.Value == "/preview")
            {
                var cmsAppUrl = _options.CmsAppUrl.TrimEnd('/');

                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Remove("X-Frame-Options");
                    context.Response.Headers["Content-Security-Policy"] =
                        $"frame-ancestors 'self' {cmsAppUrl}";
                    return Task.CompletedTask;
                });
            }

            await _next(context);
        }
    }
}
