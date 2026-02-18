using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    public interface IGraphContent
    {
        [JsonPropertyName("_metadata")]
        GraphContentMetadata? MetaData { get; set; }
    }

    public interface IGraphPageContent : IGraphContent { }

    public interface IGraphComponentContent : IGraphContent { }

    public interface IGraphMediaContent : IGraphContent { }
}
