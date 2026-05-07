using System.Runtime.InteropServices;
using SnipasteOcrHelper.Clipboard;
using SnipasteOcrHelper.Ocr;

namespace SnipasteOcrHelper.Tests;

public sealed class AdapterTests
{
    [Fact]
    public async Task RecognizeAsync_ReturnsFailure_WhenTessdataDirectoryIsMissing()
    {
        var provider = new TesseractOcrProvider(
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")),
            "eng+chi_sim");

        var result = await provider.RecognizeAsync("missing.png");

        Assert.False(result.IsSuccess);
        Assert.NotEqual(string.Empty, result.Error);
    }

    [Fact]
    public async Task WriteTextAsync_UsesConfiguredTextWriter()
    {
        string? captured = null;
        var writer = new ClipboardWriter(text =>
        {
            captured = text;
            return Task.CompletedTask;
        });

        await writer.WriteTextAsync("recognized text");

        Assert.Equal("recognized text", captured);
    }

    [Fact]
    public async Task WriteTextAsync_RetriesWhenClipboardIsBusy()
    {
        var attempts = 0;
        var writer = new ClipboardWriter(_ =>
        {
            attempts++;
            if (attempts < 10)
            {
                throw new COMException("OpenClipboard failed", unchecked((int)0x800401D0));
            }

            return Task.CompletedTask;
        });

        await writer.WriteTextAsync("recognized text");

        Assert.Equal(10, attempts);
    }
}
