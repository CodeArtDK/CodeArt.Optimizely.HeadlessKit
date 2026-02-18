using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("FormElement", BaseTypes.Element)]
    public class FormElement : GraphBlock
    {
        [CMSProperty(Format = "shortString")]
        public string FormId { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Title { get; set; }

        [CultureSpecific]
        public string Description { get; set; }
    }
}
