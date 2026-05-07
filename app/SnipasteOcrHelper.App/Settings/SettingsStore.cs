using System.IO;
using System.Text.Json;

namespace SnipasteOcrHelper.Settings;

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true
    };

    private readonly string path;

    public SettingsStore(string path)
    {
        this.path = path;
    }

    public static SettingsStore CreateDefault()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SnipasteOcrHelper");
        return new SettingsStore(Path.Combine(dir, "settings.json"));
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            return new AppSettings();
        }

        await using var stream = File.OpenRead(path);
        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
            ?? new AppSettings();
        if (!string.IsNullOrWhiteSpace(settings.TessDataDirectory))
        {
            return settings;
        }

        return new AppSettings
        {
            WatchDirectory = settings.WatchDirectory,
            OcrLanguage = settings.OcrLanguage,
            MonitoringEnabled = settings.MonitoringEnabled,
            StartWithWindows = settings.StartWithWindows,
            ImageDeleteMode = settings.ImageDeleteMode
        };
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken);
    }
}
