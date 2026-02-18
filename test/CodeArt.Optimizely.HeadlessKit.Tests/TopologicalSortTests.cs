using CodeArt.Optimizely.HeadlessKit.TypeBuilder;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using FluentAssertions;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class TopologicalSortTests
{
    [Fact]
    public void TopologicalSort_IndependentTypes_PreservesOrder()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = "A", BaseType = BaseTypes.Page },
            new SaaSContentType { Key = "B", BaseType = BaseTypes.Page },
            new SaaSContentType { Key = "C", BaseType = BaseTypes.Page }
        };

        var result = CMSTypeSyncService.TopologicalSort(types);

        result.Should().HaveCount(3);
        result.Select(t => t.Key).Should().ContainInOrder("A", "B", "C");
    }

    [Fact]
    public void TopologicalSort_DependentType_PlacedAfterDependency()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = "Container", BaseType = BaseTypes.Page, MayContainTypes = new[] { "Child" } },
            new SaaSContentType { Key = "Child", BaseType = BaseTypes.Block }
        };

        var result = CMSTypeSyncService.TopologicalSort(types);

        var containerIndex = result.ToList().FindIndex(t => t.Key == "Container");
        var childIndex = result.ToList().FindIndex(t => t.Key == "Child");

        childIndex.Should().BeLessThan(containerIndex, "Child should be ordered before Container");
    }

    [Fact]
    public void TopologicalSort_TransitiveDependency_OrderedCorrectly()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = "A", BaseType = BaseTypes.Page, MayContainTypes = new[] { "B" } },
            new SaaSContentType { Key = "B", BaseType = BaseTypes.Block, MayContainTypes = new[] { "C" } },
            new SaaSContentType { Key = "C", BaseType = BaseTypes.Element }
        };

        var result = CMSTypeSyncService.TopologicalSort(types);
        var list = result.ToList();

        list.FindIndex(t => t.Key == "C").Should().BeLessThan(list.FindIndex(t => t.Key == "B"));
        list.FindIndex(t => t.Key == "B").Should().BeLessThan(list.FindIndex(t => t.Key == "A"));
    }

    [Fact]
    public void TopologicalSort_CyclicDependency_DoesNotThrow()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = "A", BaseType = BaseTypes.Page, MayContainTypes = new[] { "B" } },
            new SaaSContentType { Key = "B", BaseType = BaseTypes.Page, MayContainTypes = new[] { "A" } }
        };

        var act = () => CMSTypeSyncService.TopologicalSort(types);

        act.Should().NotThrow();
        var result = act();
        result.Should().HaveCount(2);
    }

    [Fact]
    public void TopologicalSort_NullKey_IsSkipped()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = null, BaseType = BaseTypes.Page },
            new SaaSContentType { Key = "Valid", BaseType = BaseTypes.Page }
        };

        var result = CMSTypeSyncService.TopologicalSort(types);

        result.Should().HaveCount(1);
        result[0].Key.Should().Be("Valid");
    }

    [Fact]
    public void TopologicalSort_MissingDependency_HandledGracefully()
    {
        var types = new List<SaaSContentType>
        {
            new SaaSContentType { Key = "A", BaseType = BaseTypes.Page, MayContainTypes = new[] { "NonExistent" } }
        };

        var act = () => CMSTypeSyncService.TopologicalSort(types);

        act.Should().NotThrow();
        var result = act();
        result.Should().HaveCount(1);
    }
}
