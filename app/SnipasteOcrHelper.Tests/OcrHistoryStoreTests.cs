using SnipasteOcrHelper.History;

namespace SnipasteOcrHelper.Tests;

public sealed class OcrHistoryStoreTests
{
    [Fact]
    public void Snapshot_ReturnsNewestEntriesFirst()
    {
        var store = new OcrHistoryStore(10);
        store.Add(new OcrHistoryEntry(new DateTimeOffset(2026, 5, 6, 10, 0, 0, TimeSpan.Zero), "first.png", OcrHistoryStatus.Success, "first text"));
        store.Add(new OcrHistoryEntry(new DateTimeOffset(2026, 5, 6, 10, 1, 0, TimeSpan.Zero), "second.png", OcrHistoryStatus.NoText, ""));

        var entries = store.Snapshot();

        Assert.Equal("second.png", entries[0].FileName);
        Assert.Equal("first.png", entries[1].FileName);
    }

    [Fact]
    public void Add_DropsOldestEntries_WhenCapacityIsExceeded()
    {
        var store = new OcrHistoryStore(2);
        store.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, "first.png", OcrHistoryStatus.Success, "first"));
        store.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, "second.png", OcrHistoryStatus.Success, "second"));
        store.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, "third.png", OcrHistoryStatus.Failed, "bad"));

        var entries = store.Snapshot();

        Assert.Equal(2, entries.Count);
        Assert.DoesNotContain(entries, entry => entry.FileName == "first.png");
        Assert.Contains(entries, entry => entry.FileName == "third.png" && entry.Detail == "bad");
    }
}
