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
    public abstract class ContentPage<TContentModel> : PageModel where TContentModel : class, IGraphContent
    {
        [BindProperty(SupportsGet = true)]
        public TContentModel? CurrentContent { get; set; }


    }
}
