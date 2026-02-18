using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="HttpContext"/> to detect CMS preview and edit modes.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Returns <c>true</c> if the current request is in CMS on-page edit mode.
        /// </summary>
        /// <param name="context">The HTTP context to check.</param>
        /// <returns><c>true</c> if the request is in edit mode; otherwise, <c>false</c>.</returns>
        public static bool IsOnPageEdit(this HttpContext context)
        {
            return context.Items.ContainsKey("PreviewMode") && context.Items["PreviewMode"] == "edit";
        }

        /// <summary>
        /// Returns <c>true</c> if the current request is in CMS preview mode.
        /// </summary>
        /// <param name="context">The HTTP context to check.</param>
        /// <returns><c>true</c> if the request is in preview mode; otherwise, <c>false</c>.</returns>
        public static bool IsPreview(this HttpContext context)
        {
            return context.Items.ContainsKey("PreviewMode") && context.Items["PreviewMode"] == "preview";
        }
    }
}
