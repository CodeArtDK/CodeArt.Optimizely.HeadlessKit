using CodeArt.Optimizely.HeadlessKit.ContentClient;
using CodeArt.Optimizely.HeadlessKit.Mvc.Components;
using HeadlessKit.Sample.Mvc.Models.Experiences;
using Microsoft.AspNetCore.Mvc;

namespace HeadlessKit.Sample.Mvc.ViewComponents
{
    public class SiteSettingsViewComponent : ContentViewComponentBase
    {
        public SiteSettingsViewComponent(IContentRepository repo) : base(repo) { }

        public async Task<IViewComponentResult> InvokeAsync(string? startPageKey = null, string section = "header")
        {
            LandingPage? startPage = null;
            try
            {
                startPage = string.IsNullOrEmpty(startPageKey)
                    ? await ContentRepository.GetContentByPath<LandingPage>("/")
                    : await ContentRepository.GetContent<LandingPage>(startPageKey);
            }
            catch
            {
                // Fall through with null — views use ?? fallback defaults
            }

            return section.ToLowerInvariant() switch
            {
                "footer" => View("Footer", startPage),
                _ => View("Header", startPage),
            };
        }
    }
}
