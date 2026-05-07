using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.History;
using SnipasteOcrHelper.Queue;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class OcrQueueTests
{
    [Fact]
    public async Task EnqueueAsync_ProcessesOnePathOnce_WhenDuplicated()
    {
        var ocr = new FakeOcrProvider(OcrResult.Success("hello"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(() => ocr, clipboard, statuses.Add);
        var path = Path.Combine(Path.GetTempPath(), "capture.png");

        await queue.EnqueueAsync(path);
        await queue.EnqueueAsync(path);
        await queue.DrainAsync();

        Assert.Single(ocr.Processed);
        Assert.Equal("hello", clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.LastSuccess);
    }

    [Fact]
    public async Task EnqueueAsync_DoesNotWriteClipboard_WhenOcrTextIsEmpty()
    {
        var ocr = new FakeOcrProvider(OcrResult.Success("   "));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(() => ocr, clipboard, statuses.Add);

        await queue.EnqueueAsync("empty.png");
        await queue.DrainAsync();

        Assert.Null(clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.NoText);
    }

    [Fact]
    public async Task EnqueueAsync_ContinuesAfterOcrFailure()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Failure("bad image"),
            OcrResult.Success("next text"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(() => ocr, clipboard, statuses.Add);

        await queue.EnqueueAsync("first.png");
        await queue.EnqueueAsync("second.png");
        await queue.DrainAsync();

        Assert.Equal("next text", clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.Error);
        Assert.Contains(statuses, s => s.Status == AppStatus.LastSuccess);
    }

    [Fact]
    public async Task DrainAsync_RecordsSuccessNoTextAndFailureInHistory()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Success("recognized text"),
            OcrResult.Success("   "),
            OcrResult.Failure("bad image"));
        var history = new OcrHistoryStore();
        var queue = new OcrQueue(() => ocr, new FakeClipboardWriter(), _ => { }, history);

        await queue.EnqueueAsync("success.png");
        await queue.EnqueueAsync("empty.png");
        await queue.EnqueueAsync("failed.png");
        await queue.DrainAsync();

        var entries = history.Snapshot();
        Assert.Contains(entries, entry => entry.FileName == "success.png" && entry.Status == OcrHistoryStatus.Success && entry.Detail == "recognized text");
        Assert.Contains(entries, entry => entry.FileName == "empty.png" && entry.Status == OcrHistoryStatus.NoText);
        Assert.Contains(entries, entry => entry.FileName == "failed.png" && entry.Status == OcrHistoryStatus.Failed && entry.Detail == "bad image");
    }

    [Fact]
    public async Task DrainAsync_DisposesDisposableOcrProviderAfterRecognition()
    {
        var ocr = new DisposableOcrProvider(OcrResult.Success("recognized text"));
        var queue = new OcrQueue(() => ocr, new FakeClipboardWriter(), _ => { });

        await queue.EnqueueAsync("success.png");
        await queue.DrainAsync();

        Assert.True(ocr.Disposed);
    }

    [Fact]
    public async Task DrainAsync_DeletesOnlySuccessfulImages_WhenDeleteModeIsOnSuccess()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Success("recognized text"),
            OcrResult.Success("   "),
            OcrResult.Failure("bad image"));
        var deleter = new FakeImageFileDeleter();
        var queue = new OcrQueue(() => ocr, new FakeClipboardWriter(), _ => { }, imageDeleteMode: OcrImageDeleteMode.OnSuccess, imageFileDeleter: deleter);

        await queue.EnqueueAsync("success.png");
        await queue.EnqueueAsync("empty.png");
        await queue.EnqueueAsync("failed.png");
        await queue.DrainAsync();

        Assert.Equal([Path.GetFullPath("success.png")], deleter.Deleted);
    }

    [Fact]
    public async Task DrainAsync_DeletesEveryProcessedImage_WhenDeleteModeIsAlways()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Success("recognized text"),
            OcrResult.Success("   "),
            OcrResult.Failure("bad image"));
        var deleter = new FakeImageFileDeleter();
        var queue = new OcrQueue(() => ocr, new FakeClipboardWriter(), _ => { }, imageDeleteMode: OcrImageDeleteMode.Always, imageFileDeleter: deleter);

        await queue.EnqueueAsync("success.png");
        await queue.EnqueueAsync("empty.png");
        await queue.EnqueueAsync("failed.png");
        await queue.DrainAsync();

        Assert.Equal([
            Path.GetFullPath("success.png"),
            Path.GetFullPath("empty.png"),
            Path.GetFullPath("failed.png")], deleter.Deleted);
    }

    [Fact]
    public async Task DrainAsync_ContinuesAfterImageDeleteFailure()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Success("first text"),
            OcrResult.Success("second text"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var deleter = new FakeImageFileDeleter { ThrowOnDelete = true };
        var queue = new OcrQueue(() => ocr, clipboard, statuses.Add, imageDeleteMode: OcrImageDeleteMode.Always, imageFileDeleter: deleter);

        await queue.EnqueueAsync("first.png");
        await queue.EnqueueAsync("second.png");
        await queue.DrainAsync();

        Assert.Equal("second text", clipboard.LastText);
        Assert.Equal(2, deleter.Deleted.Count);
        Assert.Contains(statuses, s => s.Status == AppStatus.Error && s.Message.Contains("delete failed"));
    }

    private sealed class FakeOcrProvider : IImageOcrProvider
    {
        private readonly OcrResult result;
        public List<string> Processed { get; } = [];

        public FakeOcrProvider(OcrResult result)
        {
            this.result = result;
        }

        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            Processed.Add(imagePath);
            return Task.FromResult(result);
        }
    }

    private sealed class DisposableOcrProvider : IImageOcrProvider, IDisposable
    {
        private readonly OcrResult result;

        public DisposableOcrProvider(OcrResult result)
        {
            this.result = result;
        }

        public bool Disposed { get; private set; }

        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(result);
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    private sealed class SequenceOcrProvider : IImageOcrProvider
    {
        private readonly Queue<OcrResult> results;

        public SequenceOcrProvider(params OcrResult[] results)
        {
            this.results = new Queue<OcrResult>(results);
        }

        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(results.Dequeue());
        }
    }

    private sealed class FakeClipboardWriter : IClipboardWriter
    {
        public string? LastText { get; private set; }

        public Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
        {
            LastText = text;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeImageFileDeleter : IImageFileDeleter
    {
        public bool ThrowOnDelete { get; init; }
        public List<string> Deleted { get; } = [];

        public Task MoveToRecycleBinAsync(string path, CancellationToken cancellationToken = default)
        {
            Deleted.Add(path);
            if (ThrowOnDelete)
            {
                throw new IOException("delete failed");
            }

            return Task.CompletedTask;
        }
    }
}
