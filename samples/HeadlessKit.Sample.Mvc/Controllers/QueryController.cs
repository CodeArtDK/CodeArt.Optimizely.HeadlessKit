using CodeArt.Optimizely.HeadlessKit.ContentClient;
using HeadlessKit.Sample.Mvc.Models.Experiences;
using Microsoft.AspNetCore.Mvc;

namespace HeadlessKit.Sample.Mvc.Controllers
{
    public class QueryController : Controller
    {
        private readonly ContentGraphClient? _graphClient;
        private readonly IContentTypeRegistry? _registry;

        public QueryController(ContentGraphClient? graphClient = null, IContentTypeRegistry? registry = null)
        {
            _graphClient = graphClient;
            _registry = registry;
        }

        public IActionResult Index()
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

            ViewData["GeneratedQuery"] = $"""
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

            return View();
        }
    }
}
