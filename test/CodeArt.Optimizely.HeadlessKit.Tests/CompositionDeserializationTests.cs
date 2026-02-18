using CodeArt.Optimizely.HeadlessKit.Core.Models.Composition;
using FluentAssertions;
using System.Text.Json;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class CompositionDeserializationTests
{
    [Fact]
    public void Deserialize_StructureNode_ReturnsCorrectType()
    {
        var json = """
        {
            "__typename": "CompositionStructureNode",
            "key": "section-1",
            "nodeType": "section",
            "displayName": "Main Section",
            "displayTemplateKey": "DefaultSection",
            "displaySettings": [
                { "key": "width", "value": "full" }
            ],
            "nodes": []
        }
        """;

        var node = JsonSerializer.Deserialize<ICompositionNode>(json);

        node.Should().BeOfType<CompositionStructureNode>();
        node!.Key.Should().Be("section-1");
        node.NodeType.Should().Be("section");
        node.DisplayName.Should().Be("Main Section");
        node.DisplayTemplateKey.Should().Be("DefaultSection");
        node.DisplaySettings.Should().HaveCount(1);
        node.DisplaySettings![0].Key.Should().Be("width");
        node.DisplaySettings[0].Value.Should().Be("full");
    }

    [Fact]
    public void Deserialize_ComponentNode_ReturnsCorrectType()
    {
        var json = """
        {
            "__typename": "CompositionComponentNode",
            "key": "component-1",
            "displayName": "Hero Component",
            "displayTemplateKey": "HeroTemplate",
            "displaySettings": []
        }
        """;

        var node = JsonSerializer.Deserialize<ICompositionNode>(json);

        node.Should().BeOfType<CompositionComponentNode>();
        node!.Key.Should().Be("component-1");
    }

    [Fact]
    public void Deserialize_NestedStructure_PreservesHierarchy()
    {
        var json = """
        {
            "__typename": "CompositionStructureNode",
            "key": "root",
            "nodeType": "experience",
            "nodes": [
                {
                    "__typename": "CompositionStructureNode",
                    "key": "section-1",
                    "nodeType": "section",
                    "nodes": [
                        {
                            "__typename": "CompositionComponentNode",
                            "key": "elem-1",
                            "displayName": "Element"
                        }
                    ]
                }
            ]
        }
        """;

        var node = JsonSerializer.Deserialize<ICompositionNode>(json);

        var root = node.Should().BeOfType<CompositionStructureNode>().Subject;
        root.Nodes.Should().HaveCount(1);
        var section = root.Nodes![0].Should().BeOfType<CompositionStructureNode>().Subject;
        section.Nodes.Should().HaveCount(1);
        section.Nodes![0].Should().BeOfType<CompositionComponentNode>();
    }

    [Fact]
    public void Deserialize_ContentComposition_Works()
    {
        var json = """
        {
            "nodes": [
                {
                    "__typename": "CompositionStructureNode",
                    "key": "root",
                    "nodeType": "experience",
                    "nodes": []
                }
            ]
        }
        """;

        var composition = JsonSerializer.Deserialize<ContentComposition>(json);

        composition.Should().NotBeNull();
        composition!.Nodes.Should().HaveCount(1);
        composition.Nodes![0].Should().BeOfType<CompositionStructureNode>();
    }

    [Fact]
    public void Deserialize_KnownTypenames_AllResolve()
    {
        var structureJson = """{ "__typename": "CompositionStructureNode", "key": "s1", "nodes": [] }""";
        var componentJson = """{ "__typename": "CompositionComponentNode", "key": "c1" }""";

        var structure = JsonSerializer.Deserialize<ICompositionNode>(structureJson);
        var component = JsonSerializer.Deserialize<ICompositionNode>(componentJson);

        structure.Should().BeOfType<CompositionStructureNode>();
        component.Should().BeOfType<CompositionComponentNode>();
    }

    [Fact]
    public void Deserialize_DisplaySettings_PopulatesCorrectly()
    {
        var json = """
        {
            "__typename": "CompositionStructureNode",
            "key": "s1",
            "displaySettings": [
                { "key": "width", "value": "full" },
                { "key": "background", "value": "dark" }
            ],
            "nodes": []
        }
        """;

        var node = JsonSerializer.Deserialize<ICompositionNode>(json) as CompositionStructureNode;

        node!.DisplaySettings.Should().HaveCount(2);
        node.DisplaySettings![0].Key.Should().Be("width");
        node.DisplaySettings[1].Value.Should().Be("dark");
    }
}
