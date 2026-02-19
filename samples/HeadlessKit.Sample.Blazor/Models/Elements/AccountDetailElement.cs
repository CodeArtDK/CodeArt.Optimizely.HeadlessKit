using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;

namespace HeadlessKit.Sample.Blazor.Models.Elements
{
    [ContentType("AccountDetailElement", BaseTypes.Element)]
    public class AccountDetailElement : GraphBlock
    {
        [CultureSpecific]
        [CMSProperty(Format = "shortString")]
        public string? Label { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? IconClass { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? DisplayFormat { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? DataKey { get; set; }
    }
}
