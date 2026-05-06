using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Queue;

namespace SnipasteOcrHelper.Tests;

public sealed class OcrQueueTests
{
    [Fact]
    public async Task EnqueueAsync_ProcessesOnePathOnce_WhenDuplicated()
    {
        var ocr = new FakeOcrProvider(OcrResult.Success("hello"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);
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
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);

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
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);

        await queue.EnqueueAsync("first.png");
        await queue.EnqueueAsync("second.png");
        await queue.DrainAsync();

        Assert.Equal("next text", clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.Error);
        Assert.Contains(statuses, s => s.Status == AppStatus.LastSuccess);
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
}
