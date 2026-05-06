using System.IO;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Queue;

public sealed class OcrQueue
{
    private readonly Func<IImageOcrProvider> ocrProviderFactory;
    private readonly IClipboardWriter clipboardWriter;
    private readonly Action<AppStatusUpdate> publishStatus;
    private readonly Queue<string> pending = new();
    private readonly HashSet<string> knownPaths = new(StringComparer.OrdinalIgnoreCase);
    private bool processing;

    public OcrQueue(
        Func<IImageOcrProvider> ocrProviderFactory,
        IClipboardWriter clipboardWriter,
        Action<AppStatusUpdate> publishStatus)
    {
        this.ocrProviderFactory = ocrProviderFactory;
        this.clipboardWriter = clipboardWriter;
        this.publishStatus = publishStatus;
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
                    var result = await ocrProviderFactory().RecognizeAsync(path, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        publishStatus(new AppStatusUpdate(AppStatus.Error, result.Error));
                    }
                    else if (string.IsNullOrWhiteSpace(result.Text))
                    {
                        publishStatus(new AppStatusUpdate(AppStatus.NoText, Path.GetFileName(path)));
                    }
                    else
                    {
                        await clipboardWriter.WriteTextAsync(result.Text, cancellationToken);
                        publishStatus(new AppStatusUpdate(AppStatus.LastSuccess, Path.GetFileName(path)));
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    publishStatus(new AppStatusUpdate(AppStatus.Error, ex.Message));
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
}
