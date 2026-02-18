using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Models;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync;
using FluentAssertions;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class ContentTypeComparerTests
{
    private static SaaSContentType CreateContentType(
        string key = "TestType",
        string? displayName = "Test Type",
        int sortOrder = 0,
        Dictionary<string, SaaSPropertyDefinition>? properties = null)
    {
        return new SaaSContentType
        {
            Key = key,
            DisplayName = displayName,
            BaseType = BaseTypes.Page,
            SortOrder = sortOrder,
            Features = new[] { Features.Versioning, Features.Localization },
            Usages = new[] { Usages.Property, Usages.Instance },
            MayContainTypes = Array.Empty<string>(),
            MediaFileExtensions = Array.Empty<string>(),
            CompositionBehaviors = Array.Empty<string>(),
            Contracts = Array.Empty<string>(),
            Properties = properties ?? new Dictionary<string, SaaSPropertyDefinition>()
        };
    }

    [Fact]
    public void Identical_Types_HasNoChanges()
    {
        var local = CreateContentType();
        var remote = CreateContentType();

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void Different_DisplayName_HasChanges()
    {
        var local = CreateContentType(displayName: "Updated Name");
        var remote = CreateContentType(displayName: "Original Name");

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void Different_SortOrder_HasChanges()
    {
        var local = CreateContentType(sortOrder: 10);
        var remote = CreateContentType(sortOrder: 5);

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void New_Property_HasChanges()
    {
        var localProps = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Title" }
        };
        var local = CreateContentType(properties: localProps);
        var remote = CreateContentType();

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void Changed_Property_DisplayName_HasChanges()
    {
        var localProps = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Updated Title" }
        };
        var remoteProps = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Original Title" }
        };

        var local = CreateContentType(properties: localProps);
        var remote = CreateContentType(properties: remoteProps);

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void Identical_Properties_NoChanges()
    {
        var props = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition
            {
                Type = "string",
                DisplayName = "Title",
                Group = "Content",
                SortOrder = 1,
                Localized = true
            }
        };

        var local = CreateContentType(properties: new Dictionary<string, SaaSPropertyDefinition>(props));
        var remote = CreateContentType(properties: new Dictionary<string, SaaSPropertyDefinition>(props));

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void BuildPatchPayload_PreservesRemoteOnlyProperties()
    {
        var localProps = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Title" }
        };
        var remoteProps = new Dictionary<string, SaaSPropertyDefinition>
        {
            ["Title"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Old Title" },
            ["ServerOnly"] = new SaaSPropertyDefinition { Type = "string", DisplayName = "Server Only" }
        };

        var local = CreateContentType(properties: localProps);
        var remote = CreateContentType(properties: remoteProps);

        var comparer = new ContentTypeComparer(local, remote);
        var patch = comparer.BuildPatchPayload();

        // Patch should contain both local and remote-only properties
        patch.Properties.Should().ContainKey("Title");
        patch.Properties.Should().ContainKey("ServerOnly");
    }

    [Fact]
    public void BuildPatchPayload_NoChanges_ReturnsMinimal()
    {
        var local = CreateContentType();
        var remote = CreateContentType();

        var comparer = new ContentTypeComparer(local, remote);
        var patch = comparer.BuildPatchPayload();

        patch.Key.Should().Be("TestType");
        patch.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Different_CompositionBehaviors_HasChanges()
    {
        var local = CreateContentType();
        local.CompositionBehaviors = new[] { "elementEnabled" };

        var remote = CreateContentType();
        remote.CompositionBehaviors = Array.Empty<string>();

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void ServerManagedFields_AreIgnored()
    {
        var local = CreateContentType();
        var remote = CreateContentType();
        remote.Created = DateTimeOffset.UtcNow;
        remote.LastModified = DateTimeOffset.UtcNow;
        remote.LastModifiedBy = "system";
        remote.Source = "api";

        var comparer = new ContentTypeComparer(local, remote);

        comparer.HasChanges.Should().BeFalse();
    }
}
