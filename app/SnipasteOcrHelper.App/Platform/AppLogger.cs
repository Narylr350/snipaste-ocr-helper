using System.IO;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Platform;

public sealed class AppLogger : IAppLogger
{
    private readonly string logPath;

    public AppLogger(string logPath)
    {
        this.logPath = logPath;
    }

    public static AppLogger CreateDefault()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SnipasteOcrHelper", "logs");
        Directory.CreateDirectory(dir);
        return new AppLogger(Path.Combine(dir, "app.log"));
    }

    public void Info(string message) => Write("INFO", message);

    public void Error(string message, Exception? exception = null)
    {
        Write("ERROR", exception is null ? message : $"{message}: {exception}");
    }

    private void Write(string level, string message)
    {
        var directory = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.AppendAllText(logPath, $"{DateTimeOffset.Now:O} [{level}] {message}{Environment.NewLine}");
    }
}
