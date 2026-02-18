using CodeArt.Optimizely.HeadlessKit.ContentClient;
using HeadlessKit.Sample.RazorPages.Models.Experiences;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeadlessKit.Sample.RazorPages.Pages
{
    public class QueryModel : PageModel
    {
        private readonly ContentGraphClient? _graphClient;
        private readonly IContentTypeRegistry? _registry;

        public string GeneratedQuery { get; set; } = string.Empty;
        public string? QueryResult { get; set; }
        public string? ErrorMessage { get; set; }

        public QueryModel(ContentGraphClient? graphClient = null, IContentTypeRegistry? registry = null)
        {
            _graphClient = graphClient;
            _registry = registry;
        }

        public void OnGet()
        {
            // Demo 1: Query by URL
            var urlQuery = GraphQuery.For<StandardPage>()
                .ForUrl("/en/")
                .Locale("en")
                .Take(1)
                .Build();

            // Demo 2: Query with filters
            var filterQuery = GraphQuery.For<ArticlePage>()
                .Where(f => f.Metadata.Status.Eq("Published"))
                .OrderBy("_metadata.published", OrderDirection.DESC)
                .Locale("en")
                .Take(5)
                .Build();

            // Demo 3: Query with composition (requires registry)
            string compositionQuery;
            if (_registry != null)
            {
                compositionQuery = GraphQuery.For<StandardPage>(_graphClient!, _registry)
                    .ForUrl("/en/")
                    .Locale("en")
                    .WithComposition(depth: 3)
                    .Take(1)
                    .Build();
            }
            else
            {
                compositionQuery = "// IContentTypeRegistry not available — register AddCodeArtOptimizelyGraphContentClient() to enable composition queries";
            }

            // Demo 4: Query by key
            var keyQuery = GraphQuery.For<ArticlePage>()
                .ForKey("my-article-key")
                .Locale("en")
                .Build();

            // Demo 5: Complex filter
            var complexQuery = GraphQuery.For<ArticlePage>()
                .Where(f => f.And(
                    f.Metadata.Status.Eq("Published"),
                    f.Or(
                        f.Field("Title").Contains("cloud"),
                        f.Field("Title").Contains("digital")
                    )
                ))
                .OrderBy("_metadata.published", OrderDirection.DESC)
                .Take(10)
                .Build();

            GeneratedQuery = $"""
            === Query by URL ===
            {urlQuery}

            === Query with Filters ===
            {filterQuery}

            === Query with Composition Tree ===
            {compositionQuery}

            === Query by Key ===
            {keyQuery}

            === Complex Filter Query ===
            {complexQuery}
            """;
        }
    }
}
