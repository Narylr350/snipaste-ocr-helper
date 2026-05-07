using System.IO;
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.History;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Queue;

public sealed class OcrQueue
{
    private readonly Func<IImageOcrProvider> ocrProviderFactory;
    private readonly IClipboardWriter clipboardWriter;
    private readonly Action<AppStatusUpdate> publishStatus;
    private readonly OcrHistoryStore? history;
    private readonly Func<OcrImageDeleteMode> getImageDeleteMode;
    private readonly IImageFileDeleter imageFileDeleter;
    private readonly Queue<string> pending = new();
    private readonly HashSet<string> knownPaths = new(StringComparer.OrdinalIgnoreCase);
    private bool processing;

    public OcrQueue(
        Func<IImageOcrProvider> ocrProviderFactory,
        IClipboardWriter clipboardWriter,
        Action<AppStatusUpdate> publishStatus,
        OcrHistoryStore? history = null,
        OcrImageDeleteMode imageDeleteMode = OcrImageDeleteMode.Never,
        IImageFileDeleter? imageFileDeleter = null)
        : this(ocrProviderFactory, clipboardWriter, publishStatus, history, () => imageDeleteMode, imageFileDeleter)
    {
    }

    public OcrQueue(
        Func<IImageOcrProvider> ocrProviderFactory,
        IClipboardWriter clipboardWriter,
        Action<AppStatusUpdate> publishStatus,
        OcrHistoryStore? history,
        Func<OcrImageDeleteMode> getImageDeleteMode,
        IImageFileDeleter? imageFileDeleter = null)
    {
        this.ocrProviderFactory = ocrProviderFactory;
        this.clipboardWriter = clipboardWriter;
        this.publishStatus = publishStatus;
        this.history = history;
        this.getImageDeleteMode = getImageDeleteMode;
        this.imageFileDeleter = imageFileDeleter ?? new ImageFileDeleter();
    }

    public Task EnqueueAsync(string path, CancellationToken cancellationToken = default)
    {
        var normalized = Path.GetFullPath(path);
        if (knownPaths.Add(normalized))
        {
            pending.Enqueue(normalized);
        }

        return Task.CompletedTask;
    }

    public async Task DrainAsync(CancellationToken cancellationToken = default)
    {
        if (processing)
        {
            return;
        }

        processing = true;
        try
        {
            while (pending.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var path = pending.Dequeue();
                publishStatus(new AppStatusUpdate(AppStatus.Processing, Path.GetFileName(path)));

                try
                {
                    var fileName = Path.GetFileName(path);
                    var result = await ocrProviderFactory().RecognizeAsync(path, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        var error = result.Error ?? string.Empty;
                        history?.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, fileName, OcrHistoryStatus.Failed, error));
                        publishStatus(new AppStatusUpdate(AppStatus.Error, error));
                        await DeleteImageIfNeededAsync(path, false, cancellationToken);
                    }
                    else if (string.IsNullOrWhiteSpace(result.Text))
                    {
                        history?.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, fileName, OcrHistoryStatus.NoText, string.Empty));
                        publishStatus(new AppStatusUpdate(AppStatus.NoText, fileName));
                        await DeleteImageIfNeededAsync(path, false, cancellationToken);
                    }
                    else
                    {
                        await clipboardWriter.WriteTextAsync(result.Text, cancellationToken);
                        history?.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, fileName, OcrHistoryStatus.Success, result.Text));
                        publishStatus(new AppStatusUpdate(AppStatus.LastSuccess, fileName));
                        await DeleteImageIfNeededAsync(path, true, cancellationToken);
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    history?.Add(new OcrHistoryEntry(DateTimeOffset.UtcNow, Path.GetFileName(path), OcrHistoryStatus.Failed, ex.Message));
                    publishStatus(new AppStatusUpdate(AppStatus.Error, ex.Message));
                    await DeleteImageIfNeededAsync(path, false, cancellationToken);
                }
                finally
                {
                    knownPaths.Remove(path);
                }
            }
        }
        finally
        {
            processing = false;
        }
    }

    private async Task DeleteImageIfNeededAsync(string path, bool success, CancellationToken cancellationToken)
    {
        var imageDeleteMode = getImageDeleteMode();
        if (imageDeleteMode == OcrImageDeleteMode.Never || (imageDeleteMode == OcrImageDeleteMode.OnSuccess && !success))
        {
            return;
        }

        try
        {
            await imageFileDeleter.MoveToRecycleBinAsync(path, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            publishStatus(new AppStatusUpdate(AppStatus.Error, ex.Message));
        }
    }
}
