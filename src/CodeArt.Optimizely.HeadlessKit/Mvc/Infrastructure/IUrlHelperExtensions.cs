using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure
{
    public static class IUrlHelperExtensions
    {
        public static string ContentUrl(this IUrlHelper helper, GraphContentReference? graph)
        {
            return graph?.Url?.Default;
        }

        public static string ContentUrl(this IUrlHelper helper, GraphContentUrl? url)
        {
            return url?.Default;
        }
    }
}
