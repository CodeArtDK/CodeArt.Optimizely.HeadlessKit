using CodeArt.Optimizely.HeadlessKit.ContentClient;
using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using HeadlessKit.Sample.Mvc.Models.Elements;
using HeadlessKit.Sample.Mvc.Models.Experiences;
using Microsoft.AspNetCore.Mvc;

namespace HeadlessKit.Sample.Mvc.ViewComponents
{
    [TemplateDescriptor(typeof(ArticleListElement))]
    public class ArticleListViewComponent : ViewComponent
    {
        private readonly ContentGraphClient _graphClient;

        public ArticleListViewComponent(ContentGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<IViewComponentResult> InvokeAsync(ArticleListElement model)
        {
            var count = model.Count > 0 ? model.Count : 6;

            List<ArticlePage> articles;
            try
            {
                articles = await GraphQuery.For<ArticlePage>(_graphClient)
                    .Where(f => f.Metadata.Status.Eq("Published"))
                    .OrderBy(ap => ap.MetaData.Published, OrderDirection.DESC)
                    .Take(count)
                    .ToListAsync();
            }
            catch
            {
                articles = new List<ArticlePage>();
            }

            ViewBag.Heading = model.Heading;
            return View(articles);
        }
    }
}
