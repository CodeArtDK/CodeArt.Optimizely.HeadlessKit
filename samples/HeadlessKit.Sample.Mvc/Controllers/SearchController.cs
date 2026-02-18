using CodeArt.Optimizely.HeadlessKit.ContentClient;
using HeadlessKit.Sample.Mvc.Models.Experiences;
using Microsoft.AspNetCore.Mvc;

namespace HeadlessKit.Sample.Mvc.Controllers
{
    public class SearchController : Controller
    {
        private readonly ContentGraphClient _graphClient;

        public SearchController(ContentGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<IActionResult> Index([FromQuery(Name = "q")] string? query)
        {
            ViewData["Query"] = query;

            if (string.IsNullOrWhiteSpace(query))
            {
                ViewData["Results"] = new List<ArticlePage>();
                ViewData["Total"] = 0;
                return View();
            }

            try
            {
                var locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var result = await GraphQuery.SearchPages<ArticlePage>(_graphClient)
                    .Locale(locale)
                    .Fuzzy(query)
                    .Take(20)
                    .ExecuteAsync();

                ViewData["Results"] = result.Items;
                ViewData["Total"] = result.Total;
            }
            catch
            {
                // Search may fail if Graph is unavailable
                ViewData["Results"] = new List<ArticlePage>();
                ViewData["Total"] = 0;
            }

            return View();
        }
    }
}
