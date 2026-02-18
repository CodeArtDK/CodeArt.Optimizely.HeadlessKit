using CodeArt.Optimizely.HeadlessKit.Mvc.Attributes;
using CodeArt.Optimizely.HeadlessKit.Mvc.Controllers;
using HeadlessKit.Sample.Mvc.Models.Experiences;

namespace HeadlessKit.Sample.Mvc.Controllers
{
    [TemplateDescriptor(typeof(ArticlePage))]
    public class ArticlePageController : ContentControllerBase<ArticlePage>
    {
    }
}
