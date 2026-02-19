using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using CodeArt.Optimizely.HeadlessKit.Mvc.Components;
using Microsoft.AspNetCore.Mvc;

namespace HeadlessKit.Sample.Mvc.ViewComponents
{
    public class NavigationViewComponent : ContentViewComponentBase
    {
        public NavigationViewComponent(IContentRepository repo) : base(repo) { }

        public async Task<IViewComponentResult> InvokeAsync(string? startPageKey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(startPageKey))
                {
                    var startPage = await ContentRepository.GetContentByPath<GraphPageContent>("/");
                    startPageKey = startPage?.MetaData?.Key;
                }

                if (string.IsNullOrEmpty(startPageKey))
                    return View(new List<GraphPageContent>());

                var children = await ContentRepository.GetChildren<GraphPageContent>(startPageKey);
                return View(children ?? new List<GraphPageContent>());
            }
            catch
            {
                return View(new List<GraphPageContent>());
            }
        }
    }
}
