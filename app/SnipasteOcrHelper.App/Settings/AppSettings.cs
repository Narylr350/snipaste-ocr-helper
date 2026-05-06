namespace SnipasteOcrHelper.Settings;

public sealed class AppSettings
{
    public string WatchDirectory { get; init; } = string.Empty;
    public string TessDataDirectory { get; init; } = DefaultPaths.TessDataDirectory;
    public string OcrLanguage { get; init; } = "eng+chi_sim";
    public bool MonitoringEnabled { get; init; } = true;
    public bool StartWithWindows { get; init; }
}
