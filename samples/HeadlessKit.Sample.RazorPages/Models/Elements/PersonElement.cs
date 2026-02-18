using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("PersonElement", BaseTypes.Element)]
    public class PersonElement : GraphBlock
    {
        public GraphContentReference Photo { get; set; }

        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string Name { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Role { get; set; }

        public string Bio { get; set; }

        [CMSProperty(Format = "shortString")]
        public string TwitterHandle { get; set; }

        [CMSProperty(Format = "shortString")]
        public string LinkedInUrl { get; set; }
    }
}
