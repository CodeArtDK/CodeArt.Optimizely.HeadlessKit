using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.HeadlessKit.TypeBuilder.Annotation
{
    public class ContentTypeAttribute : Attribute
    {
        public string Key { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public BaseTypes BaseType { get; set; }

        public int SortOrder { get; set; }

        public Features[] Features { get; set; } = new Features[] { Models.Features.Versioning, Models.Features.Localization, Models.Features.PublishPeriod };

        public Usages[] Usages { get; set; } = new Usages[] { Models.Usages.Property, Models.Usages.Instance };

        public string[]? MayContainTypes { get; set; }

        public string[]? MediaFileExtensions { get; set; }

        public string[]? CompositionBehaviors { get; set; }

        public string[]? Contracts { get; set; }

        public bool IsContract { get; set; }

        public string? Source { get; set; }

        public ContentTypeAttribute(string key, BaseTypes baseType)
        {
            Key = key;
            BaseType = baseType;

        }
    }
}
