using CodeArt.Optimizely.HeadlessKit.Core.Models;
using CodeArt.Optimizely.HeadlessKit.ContentClient;
using FluentAssertions;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class TestPageContent : GraphPageContent
{
    public string? Title { get; set; }
}

public class GraphQueryBuilderTests
{
    [Fact]
    public void Build_SimpleQuery_ContainsTypeName()
    {
        var query = new GraphQueryBuilder<TestPageContent>().Build();

        query.Should().Contain("TestPageContent(");
    }

    [Fact]
    public void Build_WithTake_IncludesLimit()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .Take(10)
            .Build();

        query.Should().Contain("limit: 10");
    }

    [Fact]
    public void Build_WithSkip_IncludesSkip()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .Skip(5)
            .Build();

        query.Should().Contain("skip: 5");
    }

    [Fact]
    public void Build_WithOrderBy_IncludesOrderClause()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .OrderBy("Title", OrderDirection.DESC)
            .Build();

        query.Should().Contain("orderBy: { Title: DESC }");
    }

    [Fact]
    public void Build_WithNestedOrderBy_IncludesNestedOrderClause()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .OrderBy("_metadata.published", OrderDirection.ASC)
            .Build();

        query.Should().Contain("orderBy:");
        query.Should().Contain("_metadata:");
        query.Should().Contain("published: ASC");
    }

    [Fact]
    public void Build_WithLocale_IncludesLocale()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .Locale("en", "da")
            .Build();

        query.Should().Contain("locale: [en, da]");
    }

    [Fact]
    public void Build_WithSingleFilter_IncludesWhereClause()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .Where(f => f.Metadata.Key.Eq("test-key"))
            .Build();

        query.Should().Contain("where: {");
        query.Should().Contain("_metadata:");
        query.Should().Contain("key:");
        query.Should().Contain("eq: \"test-key\"");
    }

    [Fact]
    public void Build_WithMultipleFilters_UsesAnd()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .Where(f => f.Metadata.Key.Eq("a"))
            .Where(f => f.Metadata.Locale.Eq("en"))
            .Build();

        query.Should().Contain("_and:");
    }

    [Fact]
    public void Build_ForUrl_CreatesUrlFilter()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .ForUrl("/about")
            .Build();

        query.Should().Contain("_metadata:");
        query.Should().Contain("url:");
        query.Should().Contain("default:");
        query.Should().Contain("eq: \"/about\"");
    }

    [Fact]
    public void Build_IncludesItemsAndTotalAndCursor()
    {
        var query = new GraphQueryBuilder<TestPageContent>().Build();

        query.Should().Contain("items {");
        query.Should().Contain("total");
        query.Should().Contain("cursor");
    }

    [Fact]
    public void Build_WithCursor_IncludesCursorArg()
    {
        var query = new GraphQueryBuilder<TestPageContent>()
            .After("abc123")
            .Build();

        query.Should().Contain("cursor: \"abc123\"");
    }

    [Fact]
    public void Build_IncludesFieldSelection()
    {
        var query = new GraphQueryBuilder<TestPageContent>().Build();

        query.Should().Contain("Title");
        query.Should().Contain("_metadata");
    }
}
