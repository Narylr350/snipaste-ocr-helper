using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Clipboard;

public sealed class ClipboardWriter : IClipboardWriter
{
    private readonly Func<string, Task> writeText;

    public ClipboardWriter()
        : this(text => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => System.Windows.Clipboard.SetText(text)).Task)
    {
    }

    public ClipboardWriter(Func<string, Task> writeText)
    {
        this.writeText = writeText;
    }

    public Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return writeText(text);
    }
}
