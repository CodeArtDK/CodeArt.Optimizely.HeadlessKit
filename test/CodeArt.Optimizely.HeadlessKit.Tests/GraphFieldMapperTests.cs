using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using FluentAssertions;
using System.Text.Json.Serialization;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class GraphFieldMapperTests
{
    private class SimpleModel : GraphPageContent
    {
        public string? Title { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    private class RichTextModel : GraphPageContent
    {
        public GraphContentRichText? Body { get; set; }
    }

    private class ReferenceModel : GraphPageContent
    {
        public GraphContentReference? Image { get; set; }
    }

    private class UrlModel : GraphPageContent
    {
        public GraphContentUrl? Link { get; set; }
    }

    private class JsonPropertyNameModel : GraphPageContent
    {
        [JsonPropertyName("custom_name")]
        public string? MyProperty { get; set; }
    }

    private class JsonIgnoreModel : GraphPageContent
    {
        public string? Visible { get; set; }

        [JsonIgnore]
        public string? Hidden { get; set; }
    }

    private class ListModel : GraphPageContent
    {
        public List<string>? Tags { get; set; }
    }

    private class NestedModel : GraphPageContent
    {
        public InnerClass? Inner { get; set; }
    }

    public class InnerClass
    {
        public string? Name { get; set; }
    }

    private class CompositionModel : GraphPageContent
    {
        public string? Title { get; set; }
        public ContentComposition? Composition { get; set; }
    }

    private class CircularA : GraphPageContent
    {
        public CircularB? Ref { get; set; }
    }

    private class CircularB
    {
        public CircularA? Back { get; set; }
    }

    [Fact]
    public void BuildFieldSelection_ScalarProperties_IncludesFieldNames()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(SimpleModel));

        result.Should().Contain("Title");
        result.Should().Contain("Count");
        result.Should().Contain("IsActive");
    }

    [Fact]
    public void BuildFieldSelection_IncludesMetadata()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(SimpleModel));

        result.Should().Contain("_metadata");
        result.Should().Contain("key");
        result.Should().Contain("displayName");
    }

    [Fact]
    public void BuildFieldSelection_RichText_IncludesHtmlSubfield()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(RichTextModel));

        result.Should().Contain("Body { html }");
    }

    [Fact]
    public void BuildFieldSelection_ContentReference_IncludesSubfields()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(ReferenceModel));

        result.Should().Contain("Image { key url { default } }");
    }

    [Fact]
    public void BuildFieldSelection_Url_IncludesSubfields()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(UrlModel));

        result.Should().Contain("Link { default hierarchical type base }");
    }

    [Fact]
    public void BuildFieldSelection_JsonPropertyName_UsesCustomName()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(JsonPropertyNameModel));

        result.Should().Contain("custom_name");
        result.Should().NotContain("MyProperty");
    }

    [Fact]
    public void BuildFieldSelection_JsonIgnore_ExcludesProperty()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(JsonIgnoreModel));

        result.Should().Contain("Visible");
        result.Should().NotContain("Hidden");
    }

    [Fact]
    public void BuildFieldSelection_ListOfStrings_IncludesFieldName()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(ListModel));

        result.Should().Contain("Tags");
    }

    [Fact]
    public void BuildFieldSelection_NestedObject_IncludesSubfields()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(NestedModel));

        result.Should().Contain("Inner {");
        result.Should().Contain("Name");
    }

    [Fact]
    public void BuildFieldSelection_Composition_IsSkipped()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(CompositionModel));

        result.Should().Contain("Title");
        result.Should().NotContain("Composition");
        result.Should().NotContain("composition");
    }

    [Fact]
    public void BuildFieldSelection_CircularReference_DoesNotStackOverflow()
    {
        var result = GraphFieldMapper.BuildFieldSelection(typeof(CircularA));

        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BuildCompositionSelection_GeneratesCorrectStructure()
    {
        var componentTypes = new[] { typeof(SimpleModel) };
        var result = GraphFieldMapper.BuildCompositionSelection(componentTypes, 2);

        result.Should().Contain("composition {");
        result.Should().Contain("nodes {");
        result.Should().Contain("... on CompositionStructureNode {");
        result.Should().Contain("... on CompositionComponentNode {");
        result.Should().Contain("component {");
        result.Should().Contain($"... on {nameof(SimpleModel)}");
    }
}
