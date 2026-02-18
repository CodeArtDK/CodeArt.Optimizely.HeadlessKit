using CodeArt.Optimizely.HeadlessKit.ContentClient;
using HeadlessKit.Sample.RazorPages.Models.Experiences;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeadlessKit.Sample.RazorPages.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ContentGraphClient _graphClient;

        public SearchModel(ContentGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        [BindProperty(SupportsGet = true, Name = "q")]
        public string? Query { get; set; }

        public List<ArticlePage> Results { get; set; } = new();
        public int Total { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
                return;

            try
            {
                var locale = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var result = await GraphQuery.SearchPages<ArticlePage>(_graphClient)
                    .Locale(locale)
                    .Fuzzy(Query)
                    .Take(20)
                    .ExecuteAsync();

                Results = result.Items;
                Total = result.Total;
            }
            catch
            {
                // Search may fail if Graph is unavailable
                Results = new List<ArticlePage>();
                Total = 0;
            }
        }
    }
}
