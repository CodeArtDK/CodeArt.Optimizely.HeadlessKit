using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Helpers
{
    public static class HttpContextExtensions
    {
        public static bool IsOnPageEdit(this HttpContext context)
        {
            return context.Items.ContainsKey("PreviewMode") && context.Items["PreviewMode"] == "edit";
        }

        public static bool IsPreview(this HttpContext context)
        {
            return context.Items.ContainsKey("PreviewMode") && context.Items["PreviewMode"] == "preview";
        }
    }
}
