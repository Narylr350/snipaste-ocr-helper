using Microsoft.Win32;

namespace SnipasteOcrHelper.Platform;

public interface IStartupRegistry
{
    void SetValue(string name, string value);
    void DeleteValue(string name);
}

public sealed class CurrentUserRunRegistry : IStartupRegistry
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public void SetValue(string name, string value)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKey, writable: true);
        key.SetValue(name, value);
    }

    public void DeleteValue(string name)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        key?.DeleteValue(name, throwOnMissingValue: false);
    }
}

public sealed class StartupManager
{
    private readonly IStartupRegistry registry;
    private readonly string appName;
    private readonly string executablePath;

    public StartupManager(IStartupRegistry registry, string appName, string executablePath)
    {
        this.registry = registry;
        this.appName = appName;
        this.executablePath = executablePath;
    }

    public static StartupManager CreateDefault()
    {
        return new StartupManager(
            new CurrentUserRunRegistry(),
            "SnipasteOcrHelper",
            Environment.ProcessPath ?? System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            registry.SetValue(appName, $"\"{executablePath}\"");
        }
        else
        {
            registry.DeleteValue(appName);
        }
    }
}
