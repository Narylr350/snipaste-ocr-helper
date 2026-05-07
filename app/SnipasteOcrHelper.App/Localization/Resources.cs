using System.Globalization;
using System.Resources;

namespace SnipasteOcrHelper.Localization;

public static class Resources
{
    private static readonly ResourceManager ResourceManager = new("SnipasteOcrHelper.Localization.Resources", typeof(Resources).Assembly);

    public static string AppName => GetString(nameof(AppName));
    public static string TrayOpenSettings => GetString(nameof(TrayOpenSettings));
    public static string TrayOcrHistory => GetString(nameof(TrayOcrHistory));
    public static string TrayPauseMonitoring => GetString(nameof(TrayPauseMonitoring));
    public static string TrayResumeMonitoring => GetString(nameof(TrayResumeMonitoring));
    public static string TrayExit => GetString(nameof(TrayExit));
    public static string TrayTooltip => GetString(nameof(TrayTooltip));
    public static string SettingsTitle => GetString(nameof(SettingsTitle));
    public static string SettingsDescription => GetString(nameof(SettingsDescription));
    public static string SettingsWatchDirectory => GetString(nameof(SettingsWatchDirectory));
    public static string SettingsTessDataDirectory => GetString(nameof(SettingsTessDataDirectory));
    public static string SettingsOcrLanguage => GetString(nameof(SettingsOcrLanguage));
    public static string SettingsStartWithWindows => GetString(nameof(SettingsStartWithWindows));
    public static string SettingsImageDeleteMode => GetString(nameof(SettingsImageDeleteMode));
    public static string SettingsImageDeleteNever => GetString(nameof(SettingsImageDeleteNever));
    public static string SettingsImageDeleteOnSuccess => GetString(nameof(SettingsImageDeleteOnSuccess));
    public static string SettingsImageDeleteAlways => GetString(nameof(SettingsImageDeleteAlways));
    public static string SettingsBrowse => GetString(nameof(SettingsBrowse));
    public static string SettingsSave => GetString(nameof(SettingsSave));
    public static string SettingsCancel => GetString(nameof(SettingsCancel));
    public static string HistoryTitle => GetString(nameof(HistoryTitle));
    public static string HistoryEmpty => GetString(nameof(HistoryEmpty));
    public static string HistoryTimeColumn => GetString(nameof(HistoryTimeColumn));
    public static string HistoryFileColumn => GetString(nameof(HistoryFileColumn));
    public static string HistoryStatusColumn => GetString(nameof(HistoryStatusColumn));
    public static string HistoryDetailColumn => GetString(nameof(HistoryDetailColumn));
    public static string HistoryStatusSuccess => GetString(nameof(HistoryStatusSuccess));
    public static string HistoryStatusNoText => GetString(nameof(HistoryStatusNoText));
    public static string HistoryStatusFailed => GetString(nameof(HistoryStatusFailed));
    public static string StatusNeedsSetup => GetString(nameof(StatusNeedsSetup));
    public static string StatusRunning => GetString(nameof(StatusRunning));
    public static string StatusPaused => GetString(nameof(StatusPaused));
    public static string StatusProcessing => GetString(nameof(StatusProcessing));
    public static string StatusLastSuccess => GetString(nameof(StatusLastSuccess));
    public static string StatusError => GetString(nameof(StatusError));
    public static string StatusNoText => GetString(nameof(StatusNoText));
    public static string StatusConfigureWatchDirectory => GetString(nameof(StatusConfigureWatchDirectory));
    public static string StatusWatchDirectoryMissing => GetString(nameof(StatusWatchDirectoryMissing));
    public static string StatusMonitoringPaused => GetString(nameof(StatusMonitoringPaused));

    private static string GetString(string name)
    {
        return ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
    }
}
