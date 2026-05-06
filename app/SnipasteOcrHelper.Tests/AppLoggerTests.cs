using SnipasteOcrHelper.Platform;

namespace SnipasteOcrHelper.Tests;

public sealed class AppLoggerTests
{
    [Fact]
    public void Info_WritesInfoLineToLogFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "app.log");
        var logger = new AppLogger(path);

        logger.Info("started");

        var content = File.ReadAllText(path);
        Assert.Contains("[INFO] started", content);
    }

    [Fact]
    public void Error_WritesExceptionDetailsToLogFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "app.log");
        var logger = new AppLogger(path);

        logger.Error("failed", new InvalidOperationException("bad state"));

        var content = File.ReadAllText(path);
        Assert.Contains("[ERROR] failed", content);
        Assert.Contains("bad state", content);
    }
}
