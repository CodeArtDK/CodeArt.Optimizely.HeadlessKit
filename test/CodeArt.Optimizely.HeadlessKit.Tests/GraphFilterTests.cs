using CodeArt.Optimizely.HeadlessKit.ContentClient;
using FluentAssertions;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class GraphFilterTests
{
    private readonly GraphFilterBuilder _builder = new();

    [Fact]
    public void Eq_String_FormatsCorrectly()
    {
        var filter = _builder.Field("title").Eq("hello");

        filter.ToString().Should().Be("title: { eq: \"hello\" }");
    }

    [Fact]
    public void Eq_Int_FormatsCorrectly()
    {
        var filter = _builder.Field("count").Eq(42);

        filter.ToString().Should().Be("count: { eq: 42 }");
    }

    [Fact]
    public void Eq_Bool_FormatsCorrectly()
    {
        var filter = _builder.Field("active").Eq(true);

        filter.ToString().Should().Be("active: { eq: true }");
    }

    [Fact]
    public void In_Strings_FormatsCorrectly()
    {
        var filter = _builder.Field("status").In("draft", "published");

        filter.ToString().Should().Be("status: { in: [\"draft\", \"published\"] }");
    }

    [Fact]
    public void In_Ints_FormatsCorrectly()
    {
        var filter = _builder.Field("id").In(1, 2, 3);

        filter.ToString().Should().Be("id: { in: [1, 2, 3] }");
    }

    [Fact]
    public void Like_FormatsCorrectly()
    {
        var filter = _builder.Field("name").Like("*test*");

        filter.ToString().Should().Be("name: { like: \"*test*\" }");
    }

    [Fact]
    public void StartsWith_FormatsCorrectly()
    {
        var filter = _builder.Field("path").StartsWith("/en/");

        filter.ToString().Should().Be("path: { startsWith: \"/en/\" }");
    }

    [Fact]
    public void Exists_FormatsCorrectly()
    {
        var filter = _builder.Field("image").Exists(true);

        filter.ToString().Should().Be("image: { exist: true }");
    }

    [Fact]
    public void NestedField_FormatsWithNestedBraces()
    {
        var filter = _builder.Field("_metadata.url.default").Eq("/test");

        filter.ToString().Should().Be("_metadata: { url: { default: { eq: \"/test\" } } }");
    }

    [Fact]
    public void And_CombinesFilters()
    {
        var filter = _builder.And(
            _builder.Field("a").Eq("1"),
            _builder.Field("b").Eq("2")
        );

        filter.ToString().Should().Contain("_and:");
    }

    [Fact]
    public void Or_CombinesFilters()
    {
        var filter = _builder.Or(
            _builder.Field("a").Eq("1"),
            _builder.Field("b").Eq("2")
        );

        filter.ToString().Should().Contain("_or:");
    }

    [Fact]
    public void Not_WrapsFilter()
    {
        var filter = GraphFilter.Not(_builder.Field("active").Eq(true));

        filter.ToString().Should().Contain("_not:");
    }

    [Fact]
    public void Eq_String_EscapesQuotes()
    {
        var filter = _builder.Field("title").Eq("say \"hello\"");

        filter.ToString().Should().Contain("\\\"hello\\\"");
    }

    [Fact]
    public void Metadata_Key_FormatsCorrectly()
    {
        var filter = _builder.Metadata.Key.Eq("test-key");

        filter.ToString().Should().Contain("_metadata:");
        filter.ToString().Should().Contain("key:");
        filter.ToString().Should().Contain("eq: \"test-key\"");
    }

    [Fact]
    public void Metadata_Url_Default_FormatsCorrectly()
    {
        var filter = _builder.Metadata.Url.Default.Eq("/test");

        filter.ToString().Should().Contain("_metadata:");
        filter.ToString().Should().Contain("url:");
        filter.ToString().Should().Contain("default:");
    }

    [Fact]
    public void Fulltext_Match_FormatsCorrectly()
    {
        var filter = _builder.Fulltext.Match("search term");

        filter.ToString().Should().Contain("_fulltext:");
        filter.ToString().Should().Contain("match:");
        filter.ToString().Should().Contain("\"search term\"");
    }

    [Fact]
    public void Fulltext_Fuzzy_FormatsCorrectly()
    {
        var filter = _builder.Fulltext.Fuzzy("search term");

        filter.ToString().Should().Contain("fuzzy: true");
    }

    [Fact]
    public void Gt_Int_FormatsCorrectly()
    {
        var filter = _builder.Field("count").Gt(5);

        filter.ToString().Should().Be("count: { gt: 5 }");
    }

    [Fact]
    public void Lte_Int_FormatsCorrectly()
    {
        var filter = _builder.Field("count").Lte(100);

        filter.ToString().Should().Be("count: { lte: 100 }");
    }
}
