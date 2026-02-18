using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    /// <summary>
    /// Marks a POCO class as an Optimizely CMS SaaS content type for automatic sync.
    /// </summary>
    /// <remarks>
    /// Types are created and updated on startup but never deleted, to prevent accidental content loss.
    /// </remarks>
    /// <example>
    /// <code>
    /// [ContentType("heroElement", BaseTypes.Element, DisplayName = "Hero Element")]
    /// public class HeroElement : GraphContent
    /// {
    ///     public string? Heading { get; set; }
    /// }
    /// </code>
    /// </example>
    public class ContentTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets the unique key identifying this content type in the CMS.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the display name shown in the CMS editor UI.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description shown in the CMS editor UI.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the base content type (e.g. Page, Block, Element).
        /// </summary>
        public BaseTypes BaseType { get; set; }

        /// <summary>
        /// Gets or sets the sort order for this type in the CMS editor.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the feature flags enabled for this content type. Defaults to Versioning, Localization, and PublishPeriod.
        /// </summary>
        public Features[] Features { get; set; } = new Features[] { Models.Features.Versioning, Models.Features.Localization, Models.Features.PublishPeriod };

        /// <summary>
        /// Gets or sets how this content type can be used in the CMS. Defaults to both Property and Instance.
        /// </summary>
        public Usages[] Usages { get; set; } = new Usages[] { Models.Usages.Property, Models.Usages.Instance };

        /// <summary>
        /// Gets or sets the content type keys that this type may contain as children.
        /// </summary>
        public string[]? MayContainTypes { get; set; }

        /// <summary>
        /// Gets or sets the allowed file extensions for media content types.
        /// </summary>
        public string[]? MediaFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets the composition behavior keys assigned to this content type.
        /// </summary>
        public string[]? CompositionBehaviors { get; set; }

        /// <summary>
        /// Gets or sets the contract keys that this content type implements.
        /// </summary>
        public string[]? Contracts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this content type is a contract (interface-like type).
        /// </summary>
        public bool IsContract { get; set; }

        /// <summary>
        /// Gets or sets the source identifier for this content type definition.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeAttribute"/> class.
        /// </summary>
        /// <param name="key">The unique key identifying this content type in the CMS.</param>
        /// <param name="baseType">The base content type (e.g. Page, Block, Element).</param>
        public ContentTypeAttribute(string key, BaseTypes baseType)
        {
            Key = key;
            BaseType = baseType;

        }
    }
}
