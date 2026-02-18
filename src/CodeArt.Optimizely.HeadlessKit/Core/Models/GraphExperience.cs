using System.Text.Json.Serialization;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public class GraphExperience : GraphPageContent
    {
        public ContentComposition? Composition { get; set; }
    }
}
