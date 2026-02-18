using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.RazorPages.Models.Elements
{
    [ContentType("ContactInfoElement", BaseTypes.Element)]
    public class ContactInfoElement : GraphBlock
    {
        public string Address { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Phone { get; set; }

        [CMSProperty(Format = "shortString")]
        public string Email { get; set; }

        public string MapEmbedUrl { get; set; }
    }
}
