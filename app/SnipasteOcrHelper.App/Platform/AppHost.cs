using System.IO;
using SnipasteOcrHelper.Clipboard;
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Platform;
using SnipasteOcrHelper.Queue;
using SnipasteOcrHelper.Settings;
using SnipasteOcrHelper.Tray;
using SnipasteOcrHelper.Watching;

namespace SnipasteOcrHelper;

public sealed class AppHost : IDisposable
{
    private readonly SettingsStore settingsStore;
    private readonly StartupManager startupManager;
    private readonly AppLogger logger;
    private readonly ScreenshotWatcher watcher;
    private readonly OcrQueue queue;
    private readonly TrayController tray;
    private AppSettings settings = new();
    private bool paused;

    private AppHost(SettingsStore settingsStore, StartupManager startupManager, AppLogger logger)
    {
        this.settingsStore = settingsStore;
        this.startupManager = startupManager;
        this.logger = logger;
        var clipboard = new ClipboardWriter();
        queue = new OcrQueue(
            () => new TesseractOcrProvider(settings.TessDataDirectory, settings.OcrLanguage),
            clipboard,
            UpdateStatus);
        watcher = new ScreenshotWatcher(async path =>
        {
            await queue.EnqueueAsync(path);
            await queue.DrainAsync();
        }, new FileStabilityProbe());
        tray = new TrayController(OpenSettings, TogglePaused, () => System.Windows.Application.Current.Shutdown());
    }

    public static AppHost CreateDefault()
    {
        return new AppHost(SettingsStore.CreateDefault(), StartupManager.CreateDefault(), AppLogger.CreateDefault());
    }

    public async void Start()
    {
        settings = await settingsStore.LoadAsync();
        ApplyStartupSetting(settings.StartWithWindows);
        if (string.IsNullOrWhiteSpace(settings.WatchDirectory) || !Directory.Exists(settings.WatchDirectory))
        {
            UpdateStatus(new AppStatusUpdate(AppStatus.NeedsSetup, "Configure watch directory"));
            OpenSettings();
            return;
        }

        StartWatcher();
    }

    private void OpenSettings()
    {
        var window = new SettingsWindow(settings);
        if (window.ShowDialog() == true && window.SavedSettings is not null)
        {
            settings = window.SavedSettings;
            _ = settingsStore.SaveAsync(settings);
            ApplyStartupSetting(settings.StartWithWindows);
            if (Directory.Exists(settings.WatchDirectory))
            {
                StartWatcher();
            }
            else
            {
                UpdateStatus(new AppStatusUpdate(AppStatus.Error, "Watch directory does not exist"));
            }
        }
    }

    private void StartWatcher()
    {
        watcher.Start(settings.WatchDirectory);
        UpdateStatus(new AppStatusUpdate(AppStatus.Running, settings.WatchDirectory));
    }

    private void TogglePaused()
    {
        paused = !paused;
        if (paused)
        {
            watcher.Pause();
            UpdateStatus(new AppStatusUpdate(AppStatus.Paused, "Monitoring paused"));
        }
        else
        {
            watcher.Resume();
            UpdateStatus(new AppStatusUpdate(AppStatus.Running, settings.WatchDirectory));
        }
    }

    private void UpdateStatus(AppStatusUpdate update)
    {
        tray.UpdateStatus(update);
        if (update.Status == AppStatus.Error)
        {
            logger.Error(update.Message);
        }
        else
        {
            logger.Info($"{update.Status}: {update.Message}");
        }
    }

    private void ApplyStartupSetting(bool enabled)
    {
        try
        {
            startupManager.SetEnabled(enabled);
        }
        catch (Exception ex)
        {
            logger.Error("Failed to update startup setting", ex);
        }
    }

    public void Dispose()
    {
        watcher.Dispose();
        tray.Dispose();
    }
}
