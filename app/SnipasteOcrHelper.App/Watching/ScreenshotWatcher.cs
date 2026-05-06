using System.IO;

namespace SnipasteOcrHelper.Watching;

public sealed class ScreenshotWatcher : IDisposable
{
    private readonly Func<string, Task> onImageReady;
    private readonly FileStabilityProbe stabilityProbe;
    private FileSystemWatcher? watcher;
    private bool paused;

    public ScreenshotWatcher(Func<string, Task> onImageReady, FileStabilityProbe stabilityProbe)
    {
        this.onImageReady = onImageReady;
        this.stabilityProbe = stabilityProbe;
    }

    public void Start(string directory)
    {
        Stop();
        watcher = new FileSystemWatcher(directory)
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };
        watcher.Created += OnChanged;
        watcher.Changed += OnChanged;
    }

    public void Pause() => paused = true;

    public void Resume() => paused = false;

    public void Stop()
    {
        if (watcher is null)
        {
            return;
        }

        watcher.EnableRaisingEvents = false;
        watcher.Created -= OnChanged;
        watcher.Changed -= OnChanged;
        watcher.Dispose();
        watcher = null;
    }

    private async void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (paused || !ImageFileFilter.IsSupported(e.FullPath))
        {
            return;
        }

        if (await stabilityProbe.WaitUntilStableAsync(e.FullPath))
        {
            await onImageReady(e.FullPath);
        }
    }

    public void Dispose() => Stop();
}
