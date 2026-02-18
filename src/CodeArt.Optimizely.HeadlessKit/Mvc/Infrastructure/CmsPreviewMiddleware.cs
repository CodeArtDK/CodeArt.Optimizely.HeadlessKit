using CodeArt.Optimizely.HeadlessKit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    /// <summary>
    /// Allows the Optimizely CMS editor to embed the site in an iframe for live preview.
    /// Sets Content-Security-Policy frame-ancestors and removes X-Frame-Options on preview requests.
    /// </summary>
    public class CmsPreviewMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ContentGraphOptions _options;

        public CmsPreviewMiddleware(RequestDelegate next, IOptions<ContentGraphOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

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
