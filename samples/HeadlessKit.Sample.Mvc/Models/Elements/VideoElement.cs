using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Mvc.Models.Elements
{
    [ContentType("VideoElement", BaseTypes.Element)]
    public class VideoElement : GraphBlock
    {
        public string VideoUrl { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CultureSpecific]
        public string Description { get; set; }
    }
}
