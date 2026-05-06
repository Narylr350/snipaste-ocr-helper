using SnipasteOcrHelper.Watching;

namespace SnipasteOcrHelper.Tests;

public sealed class FileStabilityProbeTests
{
    [Fact]
    public async Task WaitUntilStableAsync_ReturnsTrue_ForReadableUnchangedFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        await File.WriteAllTextAsync(path, "image bytes");
        var probe = new FileStabilityProbe(TimeSpan.FromMilliseconds(20), 2);

        var stable = await probe.WaitUntilStableAsync(path);

        Assert.True(stable);
        File.Delete(path);
    }

    [Fact]
    public async Task WaitUntilStableAsync_ReturnsFalse_ForMissingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        var probe = new FileStabilityProbe(TimeSpan.FromMilliseconds(20), 2);

        var stable = await probe.WaitUntilStableAsync(path);

        Assert.False(stable);
    }
}
