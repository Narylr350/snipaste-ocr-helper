# Vertical MVP Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the first Windows WPF tray MVP that watches the Snipaste save directory, runs local Tesseract OCR on stable new images, writes non-empty text to the clipboard, shows tray status, supports pause/resume, publishes as a single-file app, and exposes a current-user startup toggle.

**Architecture:** Use one WPF app project under `app/` plus one xUnit test project. Keep app code service-oriented inside focused folders (`Settings`, `Watching`, `Queue`, `Ocr`, `Clipboard`, `Tray`, `Platform`) so the first delivery stays a single app while preserving clear boundaries.

**Tech Stack:** C# `net8.0-windows`, WPF, Windows Forms `NotifyIcon`, xUnit, `dotnet test`, `dotnet publish`, NuGet package `TesseractOCR`.

---

## File Structure

Create this structure:

```text
SnipasteOcrHelper.sln
app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj
app/SnipasteOcrHelper.App/App.xaml
app/SnipasteOcrHelper.App/App.xaml.cs
app/SnipasteOcrHelper.App/Clipboard/ClipboardWriter.cs
app/SnipasteOcrHelper.App/Core/AppStatus.cs
app/SnipasteOcrHelper.App/Core/OcrResult.cs
app/SnipasteOcrHelper.App/Core/Ports.cs
app/SnipasteOcrHelper.App/Ocr/TesseractOcrProvider.cs
app/SnipasteOcrHelper.App/Platform/AppHost.cs
app/SnipasteOcrHelper.App/Platform/AppLogger.cs
app/SnipasteOcrHelper.App/Platform/StartupManager.cs
app/SnipasteOcrHelper.App/Queue/OcrQueue.cs
app/SnipasteOcrHelper.App/Settings/AppSettings.cs
app/SnipasteOcrHelper.App/Settings/SettingsStore.cs
app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml
app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml.cs
app/SnipasteOcrHelper.App/Tray/TrayController.cs
app/SnipasteOcrHelper.App/Watching/FileStabilityProbe.cs
app/SnipasteOcrHelper.App/Watching/ImageFileFilter.cs
app/SnipasteOcrHelper.App/Watching/ScreenshotWatcher.cs
app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj
app/SnipasteOcrHelper.Tests/SettingsStoreTests.cs
app/SnipasteOcrHelper.Tests/ImageFileFilterTests.cs
app/SnipasteOcrHelper.Tests/FileStabilityProbeTests.cs
app/SnipasteOcrHelper.Tests/OcrQueueTests.cs
app/SnipasteOcrHelper.Tests/StartupManagerTests.cs
```

Responsibilities:

- `Core`: small shared records, enums, and testable ports.
- `Settings`: JSON settings persistence and first-run settings window.
- `Watching`: filesystem event filtering and stable-file detection.
- `Queue`: path dedupe and serial processing orchestration.
- `Ocr`: Tesseract adapter only.
- `Clipboard`: Windows clipboard adapter only.
- `Tray`: tray icon lifecycle and user commands.
- `Platform`: app startup, logging, and startup registry integration.

## Task 1: Scaffold solution and test baseline

**Files:**
- Create: `SnipasteOcrHelper.sln`
- Create: `app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj`
- Create: `app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj`
- Create: `app/SnipasteOcrHelper.App/App.xaml`
- Create: `app/SnipasteOcrHelper.App/App.xaml.cs`
- Create: `app/SnipasteOcrHelper.App/Platform/AppHost.cs`

- [ ] **Step 1: Create solution and projects**

Run:

```bash
dotnet new sln -n SnipasteOcrHelper
dotnet new wpf -n SnipasteOcrHelper.App -o app/SnipasteOcrHelper.App -f net8.0-windows
dotnet new xunit -n SnipasteOcrHelper.Tests -o app/SnipasteOcrHelper.Tests -f net8.0-windows
dotnet sln SnipasteOcrHelper.sln add app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj
dotnet add app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj reference app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj
dotnet add app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj package TesseractOCR
```

Expected: solution and two projects are created; the test project references the app project.

- [ ] **Step 2: Configure the WPF project**

Replace `app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="TesseractOCR" />
  </ItemGroup>
</Project>
```

If `dotnet add package` inserted a concrete package version, keep that version instead of removing it.

- [ ] **Step 3: Configure the test project**

Ensure `app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj` contains:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.5.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SnipasteOcrHelper.App\SnipasteOcrHelper.App.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 4: Remove template files that do not belong**

Delete template files if present:

```text
app/SnipasteOcrHelper.App/MainWindow.xaml
app/SnipasteOcrHelper.App/MainWindow.xaml.cs
app/SnipasteOcrHelper.Tests/UnitTest1.cs
```

- [ ] **Step 5: Make the app start without a main window**

Replace `app/SnipasteOcrHelper.App/App.xaml` with:

```xml
<Application x:Class="SnipasteOcrHelper.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             ShutdownMode="OnExplicitShutdown">
  <Application.Resources />
</Application>
```

Replace `app/SnipasteOcrHelper.App/App.xaml.cs` with:

```csharp
using System.Windows;

namespace SnipasteOcrHelper;

public partial class App : Application
{
    private AppHost? host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        host = AppHost.CreateDefault();
        host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        host?.Dispose();
        base.OnExit(e);
    }
}
```

Create `app/SnipasteOcrHelper.App/Platform/AppHost.cs` as a temporary compiling host:

```csharp
namespace SnipasteOcrHelper;

public sealed class AppHost : IDisposable
{
    public static AppHost CreateDefault()
    {
        return new AppHost();
    }

    public void Start()
    {
    }

    public void Dispose()
    {
    }
}
```

Task 7 replaces this temporary host with the real tray application host.

- [ ] **Step 6: Verify scaffold state**

Run:

```bash
dotnet restore SnipasteOcrHelper.sln
dotnet test SnipasteOcrHelper.sln --no-restore
```

Expected: restore succeeds and tests pass from a compiling baseline.

- [ ] **Step 7: Commit scaffold**

Run:

```bash
git add SnipasteOcrHelper.sln app/SnipasteOcrHelper.App app/SnipasteOcrHelper.Tests
git commit -m "$(cat <<'EOF'
Scaffold WPF app and test project

Create the .NET solution, WPF application, and xUnit project so the MVP can be built and tested from one baseline.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 2: Add settings persistence

**Files:**
- Create: `app/SnipasteOcrHelper.App/Settings/AppSettings.cs`
- Create: `app/SnipasteOcrHelper.App/Settings/SettingsStore.cs`
- Test: `app/SnipasteOcrHelper.Tests/SettingsStoreTests.cs`

- [ ] **Step 1: Write failing settings tests**

Create `app/SnipasteOcrHelper.Tests/SettingsStoreTests.cs`:

```csharp
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class SettingsStoreTests
{
    [Fact]
    public async Task LoadAsync_ReturnsDefaults_WhenFileDoesNotExist()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "settings.json");
        var store = new SettingsStore(path);

        var settings = await store.LoadAsync();

        Assert.Equal("eng+chi_sim", settings.OcrLanguage);
        Assert.True(settings.MonitoringEnabled);
        Assert.False(settings.StartWithWindows);
        Assert.Equal(string.Empty, settings.WatchDirectory);
        Assert.Equal(string.Empty, settings.TessDataDirectory);
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTripsSettings()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var path = Path.Combine(dir, "settings.json");
        var store = new SettingsStore(path);
        var original = new AppSettings
        {
            WatchDirectory = @"C:\Screenshots",
            TessDataDirectory = @"C:\tessdata",
            OcrLanguage = "eng+chi_sim",
            MonitoringEnabled = false,
            StartWithWindows = true
        };

        await store.SaveAsync(original);
        var loaded = await store.LoadAsync();

        Assert.Equal(original.WatchDirectory, loaded.WatchDirectory);
        Assert.Equal(original.TessDataDirectory, loaded.TessDataDirectory);
        Assert.Equal(original.OcrLanguage, loaded.OcrLanguage);
        Assert.Equal(original.MonitoringEnabled, loaded.MonitoringEnabled);
        Assert.Equal(original.StartWithWindows, loaded.StartWithWindows);
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter SettingsStoreTests
```

Expected: build fails because `SnipasteOcrHelper.Settings` types do not exist.

- [ ] **Step 3: Implement settings model and store**

Create `app/SnipasteOcrHelper.App/Settings/AppSettings.cs`:

```csharp
namespace SnipasteOcrHelper.Settings;

public sealed class AppSettings
{
    public string WatchDirectory { get; init; } = string.Empty;
    public string TessDataDirectory { get; init; } = string.Empty;
    public string OcrLanguage { get; init; } = "eng+chi_sim";
    public bool MonitoringEnabled { get; init; } = true;
    public bool StartWithWindows { get; init; }
}
```

Create `app/SnipasteOcrHelper.App/Settings/SettingsStore.cs`:

```csharp
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
        return await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken)
            ?? new AppSettings();
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
```

- [ ] **Step 4: Run settings tests**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter SettingsStoreTests
```

Expected: `SettingsStoreTests` pass. If the app still lacks `AppHost`, create the temporary stub from Task 1 Step 6 before running tests.

- [ ] **Step 5: Commit settings persistence**

Run:

```bash
git add app/SnipasteOcrHelper.App/Settings app/SnipasteOcrHelper.Tests/SettingsStoreTests.cs
git commit -m "$(cat <<'EOF'
Add settings persistence

Persist the watch directory, Tesseract data path, OCR language, monitoring state, and startup preference in a small user settings file.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 3: Add image filtering and file stability detection

**Files:**
- Create: `app/SnipasteOcrHelper.App/Watching/ImageFileFilter.cs`
- Create: `app/SnipasteOcrHelper.App/Watching/FileStabilityProbe.cs`
- Test: `app/SnipasteOcrHelper.Tests/ImageFileFilterTests.cs`
- Test: `app/SnipasteOcrHelper.Tests/FileStabilityProbeTests.cs`

- [ ] **Step 1: Write failing image filter tests**

Create `app/SnipasteOcrHelper.Tests/ImageFileFilterTests.cs`:

```csharp
using SnipasteOcrHelper.Watching;

namespace SnipasteOcrHelper.Tests;

public sealed class ImageFileFilterTests
{
    [Theory]
    [InlineData("capture.png")]
    [InlineData("capture.JPG")]
    [InlineData("capture.jpeg")]
    [InlineData("capture.bmp")]
    [InlineData("capture.webp")]
    public void IsSupported_ReturnsTrue_ForImageExtensions(string fileName)
    {
        Assert.True(ImageFileFilter.IsSupported(fileName));
    }

    [Theory]
    [InlineData("notes.txt")]
    [InlineData("capture.tmp")]
    [InlineData("capture")]
    public void IsSupported_ReturnsFalse_ForUnsupportedExtensions(string fileName)
    {
        Assert.False(ImageFileFilter.IsSupported(fileName));
    }
}
```

- [ ] **Step 2: Write failing stability tests**

Create `app/SnipasteOcrHelper.Tests/FileStabilityProbeTests.cs`:

```csharp
using SnipasteOcrHelper.Watching;

namespace SnipasteOcrHelper.Tests;

public sealed class FileStabilityProbeTests
{
    [Fact]
    public async Task WaitUntilStableAsync_ReturnsTrue_ForReadableUnchangedFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        await File.WriteAllTextAsync(path, "image bytes");
        var probe = new FileStabilityProbe(TimeSpan.FromMilliseconds(20), 2);

        var stable = await probe.WaitUntilStableAsync(path);

        Assert.True(stable);
        File.Delete(path);
    }

    [Fact]
    public async Task WaitUntilStableAsync_ReturnsFalse_ForMissingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        var probe = new FileStabilityProbe(TimeSpan.FromMilliseconds(20), 2);

        var stable = await probe.WaitUntilStableAsync(path);

        Assert.False(stable);
    }
}
```

- [ ] **Step 3: Run tests to verify they fail**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter "ImageFileFilterTests|FileStabilityProbeTests"
```

Expected: build fails because `Watching` types do not exist.

- [ ] **Step 4: Implement image filter**

Create `app/SnipasteOcrHelper.App/Watching/ImageFileFilter.cs`:

```csharp
namespace SnipasteOcrHelper.Watching;

public static class ImageFileFilter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".webp"
    };

    public static bool IsSupported(string path)
    {
        return SupportedExtensions.Contains(Path.GetExtension(path));
    }
}
```

- [ ] **Step 5: Implement stability probe**

Create `app/SnipasteOcrHelper.App/Watching/FileStabilityProbe.cs`:

```csharp
namespace SnipasteOcrHelper.Watching;

public sealed class FileStabilityProbe
{
    private readonly TimeSpan interval;
    private readonly int requiredStableSamples;

    public FileStabilityProbe(TimeSpan? interval = null, int requiredStableSamples = 3)
    {
        this.interval = interval ?? TimeSpan.FromMilliseconds(250);
        this.requiredStableSamples = requiredStableSamples;
    }

    public async Task<bool> WaitUntilStableAsync(string path, CancellationToken cancellationToken = default)
    {
        long? previousLength = null;
        DateTime? previousLastWrite = null;
        var stableSamples = 0;

        while (stableSamples < requiredStableSamples)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(path))
            {
                return false;
            }

            var info = new FileInfo(path);
            if (previousLength == info.Length && previousLastWrite == info.LastWriteTimeUtc && CanOpenForRead(path))
            {
                stableSamples++;
            }
            else
            {
                stableSamples = 0;
                previousLength = info.Length;
                previousLastWrite = info.LastWriteTimeUtc;
            }

            if (stableSamples < requiredStableSamples)
            {
                await Task.Delay(interval, cancellationToken);
            }
        }

        return true;
    }

    private static bool CanOpenForRead(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return stream.Length >= 0;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
```

- [ ] **Step 6: Run watching tests**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter "ImageFileFilterTests|FileStabilityProbeTests"
```

Expected: tests pass.

- [ ] **Step 7: Commit watcher primitives**

Run:

```bash
git add app/SnipasteOcrHelper.App/Watching app/SnipasteOcrHelper.Tests/ImageFileFilterTests.cs app/SnipasteOcrHelper.Tests/FileStabilityProbeTests.cs
git commit -m "$(cat <<'EOF'
Add watcher filtering and file stability checks

Filter supported screenshot files and wait for files to become readable and unchanged before OCR processing.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 4: Add OCR queue orchestration

**Files:**
- Create: `app/SnipasteOcrHelper.App/Core/OcrResult.cs`
- Create: `app/SnipasteOcrHelper.App/Core/AppStatus.cs`
- Create: `app/SnipasteOcrHelper.App/Core/Ports.cs`
- Create: `app/SnipasteOcrHelper.App/Queue/OcrQueue.cs`
- Test: `app/SnipasteOcrHelper.Tests/OcrQueueTests.cs`

- [ ] **Step 1: Write failing queue tests**

Create `app/SnipasteOcrHelper.Tests/OcrQueueTests.cs`:

```csharp
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Queue;

namespace SnipasteOcrHelper.Tests;

public sealed class OcrQueueTests
{
    [Fact]
    public async Task EnqueueAsync_ProcessesOnePathOnce_WhenDuplicated()
    {
        var ocr = new FakeOcrProvider(OcrResult.Success("hello"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);
        var path = Path.Combine(Path.GetTempPath(), "capture.png");

        await queue.EnqueueAsync(path);
        await queue.EnqueueAsync(path);
        await queue.DrainAsync();

        Assert.Equal(1, ocr.Processed.Count);
        Assert.Equal("hello", clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.LastSuccess);
    }

    [Fact]
    public async Task EnqueueAsync_DoesNotWriteClipboard_WhenOcrTextIsEmpty()
    {
        var ocr = new FakeOcrProvider(OcrResult.Success("   "));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);

        await queue.EnqueueAsync("empty.png");
        await queue.DrainAsync();

        Assert.Null(clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.NoText);
    }

    [Fact]
    public async Task EnqueueAsync_ContinuesAfterOcrFailure()
    {
        var ocr = new SequenceOcrProvider(
            OcrResult.Failure("bad image"),
            OcrResult.Success("next text"));
        var clipboard = new FakeClipboardWriter();
        var statuses = new List<AppStatusUpdate>();
        var queue = new OcrQueue(ocr, clipboard, statuses.Add);

        await queue.EnqueueAsync("first.png");
        await queue.EnqueueAsync("second.png");
        await queue.DrainAsync();

        Assert.Equal("next text", clipboard.LastText);
        Assert.Contains(statuses, s => s.Status == AppStatus.Error);
        Assert.Contains(statuses, s => s.Status == AppStatus.LastSuccess);
    }

    private sealed class FakeOcrProvider : IImageOcrProvider
    {
        private readonly OcrResult result;
        public List<string> Processed { get; } = [];

        public FakeOcrProvider(OcrResult result)
        {
            this.result = result;
        }

        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            Processed.Add(imagePath);
            return Task.FromResult(result);
        }
    }

    private sealed class SequenceOcrProvider : IImageOcrProvider
    {
        private readonly Queue<OcrResult> results;

        public SequenceOcrProvider(params OcrResult[] results)
        {
            this.results = new Queue<OcrResult>(results);
        }

        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(results.Dequeue());
        }
    }

    private sealed class FakeClipboardWriter : IClipboardWriter
    {
        public string? LastText { get; private set; }

        public Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
        {
            LastText = text;
            return Task.CompletedTask;
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter OcrQueueTests
```

Expected: build fails because queue and core types do not exist.

- [ ] **Step 3: Implement core records and ports**

Create `app/SnipasteOcrHelper.App/Core/OcrResult.cs`:

```csharp
namespace SnipasteOcrHelper.Core;

public sealed record OcrResult(bool IsSuccess, string Text, string Error)
{
    public static OcrResult Success(string text) => new(true, text, string.Empty);
    public static OcrResult Failure(string error) => new(false, string.Empty, error);
}
```

Create `app/SnipasteOcrHelper.App/Core/AppStatus.cs`:

```csharp
namespace SnipasteOcrHelper.Core;

public enum AppStatus
{
    NeedsSetup,
    Running,
    Paused,
    Processing,
    LastSuccess,
    Error,
    NoText
}

public sealed record AppStatusUpdate(AppStatus Status, string Message);
```

Create `app/SnipasteOcrHelper.App/Core/Ports.cs`:

```csharp
namespace SnipasteOcrHelper.Core;

public interface IImageOcrProvider
{
    Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default);
}

public interface IClipboardWriter
{
    Task WriteTextAsync(string text, CancellationToken cancellationToken = default);
}

public interface IAppLogger
{
    void Info(string message);
    void Error(string message, Exception? exception = null);
}
```

- [ ] **Step 4: Implement queue**

Create `app/SnipasteOcrHelper.App/Queue/OcrQueue.cs`:

```csharp
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Queue;

public sealed class OcrQueue
{
    private readonly IImageOcrProvider ocrProvider;
    private readonly IClipboardWriter clipboardWriter;
    private readonly Action<AppStatusUpdate> publishStatus;
    private readonly Queue<string> pending = new();
    private readonly HashSet<string> knownPaths = new(StringComparer.OrdinalIgnoreCase);
    private bool processing;

    public OcrQueue(
        IImageOcrProvider ocrProvider,
        IClipboardWriter clipboardWriter,
        Action<AppStatusUpdate> publishStatus)
    {
        this.ocrProvider = ocrProvider;
        this.clipboardWriter = clipboardWriter;
        this.publishStatus = publishStatus;
    }

    public Task EnqueueAsync(string path, CancellationToken cancellationToken = default)
    {
        var normalized = Path.GetFullPath(path);
        if (knownPaths.Add(normalized))
        {
            pending.Enqueue(normalized);
        }

        return Task.CompletedTask;
    }

    public async Task DrainAsync(CancellationToken cancellationToken = default)
    {
        if (processing)
        {
            return;
        }

        processing = true;
        try
        {
            while (pending.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var path = pending.Dequeue();
                publishStatus(new AppStatusUpdate(AppStatus.Processing, Path.GetFileName(path)));

                try
                {
                    var result = await ocrProvider.RecognizeAsync(path, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        publishStatus(new AppStatusUpdate(AppStatus.Error, result.Error));
                    }
                    else if (string.IsNullOrWhiteSpace(result.Text))
                    {
                        publishStatus(new AppStatusUpdate(AppStatus.NoText, Path.GetFileName(path)));
                    }
                    else
                    {
                        await clipboardWriter.WriteTextAsync(result.Text, cancellationToken);
                        publishStatus(new AppStatusUpdate(AppStatus.LastSuccess, Path.GetFileName(path)));
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    publishStatus(new AppStatusUpdate(AppStatus.Error, ex.Message));
                }
                finally
                {
                    knownPaths.Remove(path);
                }
            }
        }
        finally
        {
            processing = false;
        }
    }
}
```

- [ ] **Step 5: Run queue tests**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter OcrQueueTests
```

Expected: tests pass.

- [ ] **Step 6: Commit queue orchestration**

Run:

```bash
git add app/SnipasteOcrHelper.App/Core app/SnipasteOcrHelper.App/Queue app/SnipasteOcrHelper.Tests/OcrQueueTests.cs
git commit -m "$(cat <<'EOF'
Add OCR queue orchestration

Deduplicate file paths, process OCR work serially, write successful text to clipboard, and keep later items moving after failures.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 5: Add Tesseract OCR and clipboard adapters

**Files:**
- Create: `app/SnipasteOcrHelper.App/Ocr/TesseractOcrProvider.cs`
- Create: `app/SnipasteOcrHelper.App/Clipboard/ClipboardWriter.cs`

- [ ] **Step 1: Add Tesseract adapter**

Create `app/SnipasteOcrHelper.App/Ocr/TesseractOcrProvider.cs`:

```csharp
using SnipasteOcrHelper.Core;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

namespace SnipasteOcrHelper.Ocr;

public sealed class TesseractOcrProvider : IImageOcrProvider
{
    private readonly string tessDataDirectory;
    private readonly string language;

    public TesseractOcrProvider(string tessDataDirectory, string language)
    {
        this.tessDataDirectory = tessDataDirectory;
        this.language = language;
    }

    public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var engine = new Engine(tessDataDirectory, language, EngineMode.Default);
            using var image = Image.LoadFromFile(imagePath);
            using var page = engine.Process(image);
            return Task.FromResult(OcrResult.Success(page.Text.Trim()));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(OcrResult.Failure(ex.Message));
        }
    }
}
```

If the installed `TesseractOCR` package exposes language constants instead of string language construction for multi-language recognition, use the package-supported string overload or the direct language constructor documented by the installed package. Keep the provider signature as `TesseractOcrProvider(string tessDataDirectory, string language)` so settings remain stable.

- [ ] **Step 2: Add clipboard adapter**

Create `app/SnipasteOcrHelper.App/Clipboard/ClipboardWriter.cs`:

```csharp
using System.Windows;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Clipboard;

public sealed class ClipboardWriter : IClipboardWriter
{
    public Task WriteTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Application.Current.Dispatcher.InvokeAsync(() => System.Windows.Clipboard.SetText(text)).Task;
    }
}
```

- [ ] **Step 3: Build adapters**

Run:

```bash
dotnet build SnipasteOcrHelper.sln
```

Expected: build succeeds. If `TesseractOCR` API names differ, inspect package IntelliSense or generated errors and adjust only `TesseractOcrProvider.cs` while preserving the `IImageOcrProvider` contract.

- [ ] **Step 4: Commit adapters**

Run:

```bash
git add app/SnipasteOcrHelper.App/Ocr app/SnipasteOcrHelper.App/Clipboard
git commit -m "$(cat <<'EOF'
Add OCR and clipboard adapters

Connect the app boundary to local Tesseract OCR and Windows clipboard writing through small testable ports.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 6: Add startup manager

**Files:**
- Create: `app/SnipasteOcrHelper.App/Platform/StartupManager.cs`
- Test: `app/SnipasteOcrHelper.Tests/StartupManagerTests.cs`

- [ ] **Step 1: Write failing startup tests**

Create `app/SnipasteOcrHelper.Tests/StartupManagerTests.cs`:

```csharp
using SnipasteOcrHelper.Platform;

namespace SnipasteOcrHelper.Tests;

public sealed class StartupManagerTests
{
    [Fact]
    public void SetEnabled_WritesExecutablePath_WhenEnabled()
    {
        var registry = new FakeStartupRegistry();
        var manager = new StartupManager(registry, "SnipasteOcrHelper", @"C:\App\SnipasteOcrHelper.exe");

        manager.SetEnabled(true);

        Assert.Equal("\"C:\\App\\SnipasteOcrHelper.exe\"", registry.Values["SnipasteOcrHelper"]);
    }

    [Fact]
    public void SetEnabled_RemovesValue_WhenDisabled()
    {
        var registry = new FakeStartupRegistry();
        registry.Values["SnipasteOcrHelper"] = "old";
        var manager = new StartupManager(registry, "SnipasteOcrHelper", @"C:\App\SnipasteOcrHelper.exe");

        manager.SetEnabled(false);

        Assert.False(registry.Values.ContainsKey("SnipasteOcrHelper"));
    }

    private sealed class FakeStartupRegistry : IStartupRegistry
    {
        public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
        public void SetValue(string name, string value) => Values[name] = value;
        public void DeleteValue(string name) => Values.Remove(name);
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter StartupManagerTests
```

Expected: build fails because `StartupManager` does not exist.

- [ ] **Step 3: Implement startup manager**

Create `app/SnipasteOcrHelper.App/Platform/StartupManager.cs`:

```csharp
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
```

- [ ] **Step 4: Run startup tests**

Run:

```bash
dotnet test app/SnipasteOcrHelper.Tests/SnipasteOcrHelper.Tests.csproj --filter StartupManagerTests
```

Expected: tests pass.

- [ ] **Step 5: Commit startup manager**

Run:

```bash
git add app/SnipasteOcrHelper.App/Platform/StartupManager.cs app/SnipasteOcrHelper.Tests/StartupManagerTests.cs
git commit -m "$(cat <<'EOF'
Add current-user startup toggle support

Manage Windows current-user startup registration through a small registry adapter that can be tested without touching the registry.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 7: Add watcher, tray shell, settings window, and app host

**Files:**
- Create: `app/SnipasteOcrHelper.App/Watching/ScreenshotWatcher.cs`
- Create: `app/SnipasteOcrHelper.App/Tray/TrayController.cs`
- Create: `app/SnipasteOcrHelper.App/Platform/AppLogger.cs`
- Create: `app/SnipasteOcrHelper.App/Platform/AppHost.cs`
- Create: `app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml`
- Create: `app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml.cs`
- Modify: `app/SnipasteOcrHelper.App/App.xaml.cs`

- [ ] **Step 1: Add logger**

Create `app/SnipasteOcrHelper.App/Platform/AppLogger.cs`:

```csharp
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
        File.AppendAllText(logPath, $"{DateTimeOffset.Now:O} [{level}] {message}{Environment.NewLine}");
    }
}
```

- [ ] **Step 2: Add screenshot watcher**

Create `app/SnipasteOcrHelper.App/Watching/ScreenshotWatcher.cs`:

```csharp
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
```

- [ ] **Step 3: Add settings window XAML**

Create `app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml`:

```xml
<Window x:Class="SnipasteOcrHelper.Settings.SettingsWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Snipaste OCR Helper Settings"
        Width="560"
        Height="300"
        WindowStartupLocation="CenterScreen"
        ResizeMode="NoResize">
  <Grid Margin="16">
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto" />
      <RowDefinition Height="Auto" />
      <RowDefinition Height="Auto" />
      <RowDefinition Height="Auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="150" />
      <ColumnDefinition Width="*" />
      <ColumnDefinition Width="Auto" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Row="0" Grid.ColumnSpan="3" Text="Configure the folder where Snipaste saves screenshots." Margin="0,0,0,14" />

    <TextBlock Grid.Row="1" Grid.Column="0" Text="Watch directory" VerticalAlignment="Center" />
    <TextBox Grid.Row="1" Grid.Column="1" x:Name="WatchDirectoryTextBox" Margin="0,0,8,8" />
    <Button Grid.Row="1" Grid.Column="2" Content="Browse" Width="80" Margin="0,0,0,8" Click="BrowseWatchDirectory_Click" />

    <TextBlock Grid.Row="2" Grid.Column="0" Text="Tessdata directory" VerticalAlignment="Center" />
    <TextBox Grid.Row="2" Grid.Column="1" x:Name="TessDataDirectoryTextBox" Margin="0,0,8,8" />
    <Button Grid.Row="2" Grid.Column="2" Content="Browse" Width="80" Margin="0,0,0,8" Click="BrowseTessDataDirectory_Click" />

    <TextBlock Grid.Row="3" Grid.Column="0" Text="OCR language" VerticalAlignment="Center" />
    <TextBlock Grid.Row="3" Grid.Column="1" Grid.ColumnSpan="2" Text="eng+chi_sim" VerticalAlignment="Center" Margin="0,0,0,8" />

    <CheckBox Grid.Row="4" Grid.Column="1" Grid.ColumnSpan="2" x:Name="StartWithWindowsCheckBox" Content="Start with Windows" VerticalAlignment="Top" />

    <StackPanel Grid.Row="5" Grid.ColumnSpan="3" Orientation="Horizontal" HorizontalAlignment="Right">
      <Button Content="Save" Width="90" Margin="0,0,8,0" Click="Save_Click" />
      <Button Content="Cancel" Width="90" Click="Cancel_Click" />
    </StackPanel>
  </Grid>
</Window>
```

- [ ] **Step 4: Add settings window code-behind**

Create `app/SnipasteOcrHelper.App/Settings/SettingsWindow.xaml.cs`:

```csharp
using System.Windows;
using Forms = System.Windows.Forms;

namespace SnipasteOcrHelper.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppSettings originalSettings;

    public AppSettings? SavedSettings { get; private set; }

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        originalSettings = settings;
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        TessDataDirectoryTextBox.Text = settings.TessDataDirectory;
        StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
    }

    private void BrowseWatchDirectory_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(WatchDirectoryTextBox);
    }

    private void BrowseTessDataDirectory_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(TessDataDirectoryTextBox);
    }

    private static void BrowseInto(System.Windows.Controls.TextBox target)
    {
        using var dialog = new Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == Forms.DialogResult.OK)
        {
            target.Text = dialog.SelectedPath;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        SavedSettings = new AppSettings
        {
            WatchDirectory = WatchDirectoryTextBox.Text.Trim(),
            TessDataDirectory = TessDataDirectoryTextBox.Text.Trim(),
            OcrLanguage = originalSettings.OcrLanguage,
            MonitoringEnabled = true,
            StartWithWindows = StartWithWindowsCheckBox.IsChecked == true
        };
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
```

- [ ] **Step 5: Add tray controller**

Create `app/SnipasteOcrHelper.App/Tray/TrayController.cs`:

```csharp
using Forms = System.Windows.Forms;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Tray;

public sealed class TrayController : IDisposable
{
    private readonly Forms.NotifyIcon notifyIcon;
    private readonly Action openSettings;
    private readonly Action togglePaused;
    private readonly Action exit;
    private readonly Forms.ToolStripMenuItem pauseItem;
    private bool paused;

    public TrayController(Action openSettings, Action togglePaused, Action exit)
    {
        this.openSettings = openSettings;
        this.togglePaused = togglePaused;
        this.exit = exit;
        pauseItem = new Forms.ToolStripMenuItem("Pause Monitoring", null, (_, _) => TogglePaused());
        notifyIcon = new Forms.NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "Snipaste OCR Helper",
            ContextMenuStrip = new Forms.ContextMenuStrip()
        };
        notifyIcon.ContextMenuStrip.Items.Add("Open Settings", null, (_, _) => this.openSettings());
        notifyIcon.ContextMenuStrip.Items.Add(pauseItem);
        notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (_, _) => this.exit());
    }

    public void UpdateStatus(AppStatusUpdate update)
    {
        notifyIcon.Text = $"Snipaste OCR Helper - {update.Status}";
    }

    private void TogglePaused()
    {
        paused = !paused;
        pauseItem.Text = paused ? "Resume Monitoring" : "Pause Monitoring";
        togglePaused();
    }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
    }
}
```

- [ ] **Step 6: Add app host**

Create `app/SnipasteOcrHelper.App/Platform/AppHost.cs`:

```csharp
using System.Windows;
using SnipasteOcrHelper.Clipboard;
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Ocr;
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
        var ocr = new TesseractOcrProvider(string.Empty, "eng+chi_sim");
        queue = new OcrQueue(ocr, clipboard, UpdateStatus);
        watcher = new ScreenshotWatcher(async path =>
        {
            await queue.EnqueueAsync(path);
            await queue.DrainAsync();
        }, new FileStabilityProbe());
        tray = new TrayController(OpenSettings, TogglePaused, () => Application.Current.Shutdown());
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
```

- [ ] **Step 7: Correct OCR provider construction after settings load**

Replace the fixed `TesseractOcrProvider(string.Empty, "eng+chi_sim")` wiring in `AppHost` if needed by adding a provider factory to `OcrQueue` or by recreating the queue after settings load. The simplest concrete change is to modify `OcrQueue` constructor to accept `Func<IImageOcrProvider>` and call the factory inside `DrainAsync`. Use this code if settings-specific OCR wiring is needed:

```csharp
// In OcrQueue field declarations
private readonly Func<IImageOcrProvider> ocrProviderFactory;

// In constructor parameters
Func<IImageOcrProvider> ocrProviderFactory,
IClipboardWriter clipboardWriter,
Action<AppStatusUpdate> publishStatus

// In constructor body
this.ocrProviderFactory = ocrProviderFactory;

// In DrainAsync before RecognizeAsync
var result = await ocrProviderFactory().RecognizeAsync(path, cancellationToken);
```

Then construct `OcrQueue` in `AppHost` as:

```csharp
queue = new OcrQueue(
    () => new TesseractOcrProvider(settings.TessDataDirectory, settings.OcrLanguage),
    clipboard,
    UpdateStatus);
```

Update `OcrQueueTests` fake construction to pass `() => ocr` instead of `ocr`.

- [ ] **Step 8: Build app shell**

Run:

```bash
dotnet build SnipasteOcrHelper.sln
```

Expected: build succeeds.

- [ ] **Step 9: Commit app shell**

Run:

```bash
git add app/SnipasteOcrHelper.App app/SnipasteOcrHelper.Tests/OcrQueueTests.cs
git commit -m "$(cat <<'EOF'
Wire WPF tray MVP shell

Start the tray application, settings window, watcher, OCR queue, logging, and pause/resume lifecycle inside the WPF host.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 8: Add publish command and documentation writeback

**Files:**
- Modify: `docs/testing/README.md`
- Modify: `docs/tasks/platform/INDEX.md`
- Modify: `docs/tasks/watcher/INDEX.md`
- Modify: `docs/tasks/ocr/INDEX.md`
- Modify: `docs/tasks/queue/INDEX.md`
- Modify: `docs/tasks/clipboard/INDEX.md`
- Modify: `docs/tasks/tray/INDEX.md`
- Modify: `docs/tasks/settings/INDEX.md`
- Modify: `app/README.md`

- [ ] **Step 1: Verify automated tests**

Run:

```bash
dotnet test SnipasteOcrHelper.sln
```

Expected: all tests pass.

- [ ] **Step 2: Verify app build**

Run:

```bash
dotnet build SnipasteOcrHelper.sln -c Release
```

Expected: build succeeds.

- [ ] **Step 3: Verify single-file publish**

Run:

```bash
dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Expected: publish succeeds and produces `app/SnipasteOcrHelper.App/bin/Release/net8.0-windows/win-x64/publish/SnipasteOcrHelper.App.exe`.

- [ ] **Step 4: Update `app/README.md`**

Replace `app/README.md` with:

```markdown
# Main application

## Role

Primary WPF tray application for Snipaste OCR Helper.

## Build

```bash
dotnet build ../../SnipasteOcrHelper.sln
```

## Test

```bash
dotnet test ../../SnipasteOcrHelper.sln
```

## Publish

```bash
dotnet publish SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Runtime notes

- Configure the Snipaste auto-save directory on first launch.
- Configure the Tesseract tessdata directory with `eng` and `chi_sim` language data.
- OCR success writes non-empty recognized text directly to the clipboard.
- Failures are recorded in the app log and summarized by tray state.
```

- [ ] **Step 5: Update testing evidence**

Append this dated entry to `docs/testing/README.md` under `Recent Evidence` after actual verification commands pass:

```markdown
- 2026-05-06: Verified `dotnet test SnipasteOcrHelper.sln`, `dotnet build SnipasteOcrHelper.sln -c Release`, and single-file publish for `win-x64` after implementing the WPF tray MVP.
```

- [ ] **Step 6: Update affected module indexes**

In each affected `docs/tasks/<module>/INDEX.md`, update:

- `Current Status`: state that the vertical MVP implementation exists.
- `Implemented Features`: list the module-specific shipped behavior.
- `Validation`: include the exact commands from Steps 1-3 after they pass.
- `Next Useful Moves`: keep cloud OCR, richer history, installer, and retry UI as follow-up work.

Use these module-specific implemented features:

```markdown
platform: WPF app host, tray startup, settings-first launch, logging, publish command.
watcher: directory watcher, image extension filtering, stable-file wait, pause/resume.
ocr: Tesseract OCR provider with `eng+chi_sim` default language setting.
queue: path dedupe, serial processing, empty-result handling, failure continuation.
clipboard: successful non-empty OCR text writes directly to Windows clipboard.
tray: tray menu, status updates, pause/resume, settings, exit.
settings: JSON settings, watch directory, tessdata directory, language default, startup toggle.
```

- [ ] **Step 7: Commit documentation writeback**

Run:

```bash
git add app/README.md docs/testing/README.md docs/tasks
git commit -m "$(cat <<'EOF'
Document MVP build and verification

Record the build, test, publish, and module-state details for the implemented WPF tray MVP.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Task 9: Manual validation checkpoint

**Files:**
- Modify: `docs/testing/README.md`

- [ ] **Step 1: Launch the app from development output**

Run:

```bash
dotnet run --project app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj
```

Expected: app starts; if no watch directory is configured, the settings window opens.

- [ ] **Step 2: Configure runtime settings**

In the settings window:

1. Set Watch directory to a temporary folder.
2. Set Tessdata directory to a folder containing `eng.traineddata` and `chi_sim.traineddata`.
3. Leave OCR language as `eng+chi_sim`.
4. Keep startup disabled for this validation unless the user explicitly wants to test registry startup now.
5. Save.

Expected: app remains in tray and status becomes Running.

- [ ] **Step 3: Validate OCR processing**

Copy a PNG image with visible English or Chinese text into the watch directory.

Expected:

- tray status changes through Processing to Last Success, or No Text for images without recognized text;
- clipboard contains recognized text for non-empty OCR output;
- log file records the result.

- [ ] **Step 4: Validate pause/resume**

Use the tray menu to pause monitoring, copy another image into the watch directory, then resume monitoring and copy a third image.

Expected:

- paused state does not process new files;
- resumed state processes files again.

- [ ] **Step 5: Validate publish output**

Run the published executable from:

```text
app/SnipasteOcrHelper.App/bin/Release/net8.0-windows/win-x64/publish/SnipasteOcrHelper.App.exe
```

Expected: the published app launches and reaches the same settings/tray flow.

- [ ] **Step 6: Record manual evidence**

Append the actual results to `docs/testing/README.md` under `Recent Evidence`. If the validation uses `C:\tessdata`, write this exact line:

```markdown
- 2026-05-06: Manual validation ran from development output and publish output. First-run settings, watch-directory processing, clipboard write, pause/resume, and tray status were checked. Tessdata used: `C:\tessdata`.
```

If the validation uses a different tessdata directory, write the same sentence with the actual directory path used in that run.

- [ ] **Step 7: Commit manual evidence**

Run:

```bash
git add docs/testing/README.md
git commit -m "$(cat <<'EOF'
Record manual MVP validation

Capture the runtime validation evidence for first-run settings, OCR processing, clipboard writing, pause/resume, tray status, and publish output.

Co-Authored-By: Claude Opus 4.7 <noreply@anthropic.com>
EOF
)"
```

## Self-Review

Spec coverage:

- Single WPF app under `app`: Tasks 1 and 7.
- Internal service boundaries: Tasks 2-7.
- First-run settings window: Task 7.
- File watching and stable-file detection: Tasks 3 and 7.
- Path dedupe and serial queue: Task 4.
- Tesseract Chinese-English OCR: Tasks 1 and 5.
- Direct clipboard overwrite for non-empty text: Tasks 4 and 5.
- Logs plus tray status, no failure popups: Tasks 4 and 7.
- Single-file publish: Task 8.
- Startup toggle: Tasks 6 and 7.
- Automated and manual validation: Tasks 2-9.
- Documentation writeback: Task 8 and Task 9.

Placeholder scan:

- The plan avoids open-ended implementation instructions and unresolved markers.
- Manual validation evidence uses a concrete `C:\tessdata` example and instructs the executor to record the actual directory used if it differs.

Type consistency:

- `AppSettings`, `SettingsStore`, `ImageFileFilter`, `FileStabilityProbe`, `OcrResult`, `AppStatus`, `AppStatusUpdate`, `IImageOcrProvider`, `IClipboardWriter`, `OcrQueue`, `TesseractOcrProvider`, `ClipboardWriter`, `StartupManager`, `ScreenshotWatcher`, `TrayController`, and `AppHost` names are consistent across tasks.
- Task 7 Step 7 explicitly updates `OcrQueue` tests if the provider factory is introduced.
