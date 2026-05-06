using System.Globalization;
using System.Resources;

namespace SnipasteOcrHelper.Localization;

public static class Resources
{
    private static readonly ResourceManager ResourceManager = new("SnipasteOcrHelper.Localization.Resources", typeof(Resources).Assembly);

    public static string AppName => GetString(nameof(AppName));
    public static string TrayOpenSettings => GetString(nameof(TrayOpenSettings));
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
    public static string SettingsBrowse => GetString(nameof(SettingsBrowse));
    public static string SettingsSave => GetString(nameof(SettingsSave));
    public static string SettingsCancel => GetString(nameof(SettingsCancel));
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
