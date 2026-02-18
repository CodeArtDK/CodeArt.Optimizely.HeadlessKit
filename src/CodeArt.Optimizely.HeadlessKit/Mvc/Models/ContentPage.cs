using CodeArt.Optimizely.HeadlessKit.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.Mvc.Models
{
    /// <summary>
    /// Base Razor PageModel for content-routed pages. The <see cref="CurrentContent"/> property
    /// is automatically bound from the content resolved by <see cref="Infrastructure.ContentRouteTransformer"/>.
    /// </summary>
    /// <typeparam name="TContentModel">The content type this page renders.</typeparam>
    public abstract class ContentPage<TContentModel> : PageModel where TContentModel : class, IGraphContent
    {
        /// <summary>
        /// The content item resolved for the current request. Automatically populated by model binding.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public TContentModel? CurrentContent { get; set; }


    }
}
