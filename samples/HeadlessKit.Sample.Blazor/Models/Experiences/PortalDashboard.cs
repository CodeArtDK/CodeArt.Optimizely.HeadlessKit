using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System.ComponentModel.DataAnnotations;

namespace HeadlessKit.Sample.Blazor.Models.Experiences
{
    [ContentType("PortalDashboard", BaseTypes.Experience, MayContainTypes = new[] {
        "WelcomeBannerElement", "UsageStatElement", "InfoCardElement",
        "AnnouncementElement", "AccountDetailElement", "BillingHistoryElement"
    })]
    public class PortalDashboard : GraphExperience
    {
        [CultureSpecific]
        [MaxLength(100)]
        [CMSProperty(Format = "shortString")]
        public string? PortalName { get; set; }

        [CMSProperty(Format = "shortString")]
        public string? SupportEmail { get; set; }
    }
}
