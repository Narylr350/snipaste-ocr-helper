using System.IO;
using SnipasteOcrHelper.Clipboard;
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.History;
using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Platform;
using SnipasteOcrHelper.Queue;
using SnipasteOcrHelper.Settings;
using SnipasteOcrHelper.Tray;
using SnipasteOcrHelper.Watching;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper;

public sealed class AppHost : IDisposable
{
    private readonly SettingsStore settingsStore;
    private readonly StartupManager startupManager;
    private readonly AppLogger logger;
    private readonly ScreenshotWatcher watcher;
    private readonly OcrHistoryStore history = new();
    private readonly OcrQueue queue;
    private readonly TrayController tray;
    private AppSettings settings = new();
    private OcrHistoryWindow? historyWindow;
    private bool paused;

    private AppHost(SettingsStore settingsStore, StartupManager startupManager, AppLogger logger)
    {
        this.settingsStore = settingsStore;
        this.startupManager = startupManager;
        this.logger = logger;
        var clipboard = new ClipboardWriter();
        var providerFactory = new OcrProviderFactory(
            () => settings,
            currentSettings => new RapidOcrProvider(DefaultPaths.RapidOcrModelDirectory, currentSettings.RapidOcrModelPack));
        queue = new OcrQueue(
            providerFactory.Create,
            clipboard,
            UpdateStatus,
            history,
            () => settings.ImageDeleteMode);
        watcher = new ScreenshotWatcher(async path =>
        {
            await queue.EnqueueAsync(path);
            await queue.DrainAsync();
        }, new FileStabilityProbe());
        tray = new TrayController(OpenSettings, OpenHistory, TogglePaused, () => System.Windows.Application.Current.Shutdown());
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
            UpdateStatus(new AppStatusUpdate(AppStatus.NeedsSetup, Strings.StatusConfigureWatchDirectory));
            OpenSettings();
            return;
        }

        StartWatcher();
    }

    private void OpenSettings()
    {
        var window = new SettingsWindow(
            settings,
            new RapidOcrModelManager(DefaultPaths.RapidOcrModelDirectory, settings.RapidOcrModelPack),
            logger.Error);
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
                UpdateStatus(new AppStatusUpdate(AppStatus.Error, Strings.StatusWatchDirectoryMissing));
            }
        }
    }

    private void OpenHistory()
    {
        if (historyWindow is null)
        {
            historyWindow = new OcrHistoryWindow(history);
            historyWindow.Closed += (_, _) => historyWindow = null;
        }

        historyWindow.Show();
        historyWindow.Activate();
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
            UpdateStatus(new AppStatusUpdate(AppStatus.Paused, Strings.StatusMonitoringPaused));
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
