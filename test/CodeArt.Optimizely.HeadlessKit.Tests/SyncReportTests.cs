using CodeArt.Optimizely.HeadlessKit.TypeBuilder.Sync;
using FluentAssertions;

namespace CodeArt.Optimizely.HeadlessKit.Tests;

public class SyncReportTests
{
    [Fact]
    public void ToString_ReturnsFormattedSummary()
    {
        var report = new SyncReport();
        report.Created.Add("TypeA");
        report.Updated.Add("TypeB");
        report.Updated.Add("TypeC");
        report.Unchanged.Add("TypeD");
        report.Failed.Add(new SyncFailure("TypeE", "error"));

        report.ToString().Should().Be("Created: 1, Updated: 2, Unchanged: 1, Failed: 1");
    }

    [Fact]
    public void Empty_Report_ShowsZeros()
    {
        var report = new SyncReport();

        report.ToString().Should().Be("Created: 0, Updated: 0, Unchanged: 0, Failed: 0");
    }

    [Fact]
    public void SyncFailure_StoresKeyAndMessage()
    {
        var failure = new SyncFailure("MyType", "Network error");

        failure.Key.Should().Be("MyType");
        failure.ErrorMessage.Should().Be("Network error");
    }
}
