using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Core.Models
{
    /// <summary>
    /// Base interface for all content types returned from Optimizely Graph.
    /// All content model classes should implement this interface.
    /// </summary>
    public interface IGraphContent
    {
        /// <summary>
        /// Gets or sets the content metadata from Optimizely Graph.
        /// </summary>
        [JsonPropertyName("_metadata")]
        GraphContentMetadata? MetaData { get; set; }
    }

    /// <summary>
    /// Marker interface for page content types (routable content with URLs).
    /// </summary>
    public interface IGraphPageContent : IGraphContent { }

    /// <summary>
    /// Marker interface for component/element content types (embeddable blocks).
    /// </summary>
    public interface IGraphComponentContent : IGraphContent { }

    /// <summary>
    /// Marker interface for media content types (images, videos, files).
    /// </summary>
    public interface IGraphMediaContent : IGraphContent { }
}
