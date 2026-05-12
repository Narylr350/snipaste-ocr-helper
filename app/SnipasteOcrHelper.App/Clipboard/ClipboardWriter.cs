using System.Runtime.InteropServices;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Clipboard;

public sealed class ClipboardWriter : IClipboardWriter
{
    private readonly Func<string, Task> writeText;
    private readonly Func<TimeSpan, CancellationToken, Task> delay;

    public ClipboardWriter()
        : this(text => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => System.Windows.Forms.Clipboard.SetDataObject(text, true, 20, 300)).Task)
    {
    }

    public ClipboardWriter(Func<string, Task> writeText)
        : this(writeText, Task.Delay)
    {
    }

    public ClipboardWriter(Func<string, Task> writeText, Func<TimeSpan, CancellationToken, Task> delay)
    {
        this.writeText = writeText;
        this.delay = delay;
    }

    public async Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await writeText(text);
                return;
            }
            catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x800401D0))
            {
                await delay(TimeSpan.FromMilliseconds(300), cancellationToken);
            }
        }
    }
}
