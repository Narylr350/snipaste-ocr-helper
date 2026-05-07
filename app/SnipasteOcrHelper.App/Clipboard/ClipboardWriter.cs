using System.Runtime.InteropServices;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Clipboard;

public sealed class ClipboardWriter : IClipboardWriter
{
    private readonly Func<string, Task> writeText;

    public ClipboardWriter()
        : this(text => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => System.Windows.Forms.Clipboard.SetDataObject(text, true, 20, 300)).Task)
    {
    }

    public ClipboardWriter(Func<string, Task> writeText)
    {
        this.writeText = writeText;
    }

    public async Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await writeText(text);
                return;
            }
            catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x800401D0) && attempt < 19)
            {
                await Task.Delay(300, cancellationToken);
            }
        }
    }
}
