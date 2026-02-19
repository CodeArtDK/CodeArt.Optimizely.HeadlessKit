using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

namespace HeadlessKit.Sample.Blazor.Models.Experiences
{
    [ContentType("PortalPage", BaseTypes.Experience, MayContainTypes = new[] {
        "WelcomeBannerElement", "UsageStatElement", "InfoCardElement",
        "AnnouncementElement", "AccountDetailElement", "BillingHistoryElement"
    })]
    public class PortalPage : GraphExperience
    {
        [CultureSpecific]
        [MaxLength(200)]
        [CMSProperty(Format = "shortString")]
        public string? PageTitle { get; set; }

        [CultureSpecific]
        [MaxLength(500)]
        [CMSProperty(Format = "shortString")]
        public string? PageDescription { get; set; }
    }
}
