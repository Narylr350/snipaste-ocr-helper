# Main application

## Role

WPF tray application for the vertical MVP.

## Projects

- `SnipasteOcrHelper.App`: Windows tray app targeting `net8.0-windows`.
- `SnipasteOcrHelper.Tests`: xUnit tests for settings, watcher primitives, queue orchestration, adapters, startup toggle, and logging.

## Build and test

```bash
dotnet test SnipasteOcrHelper.sln
dotnet build SnipasteOcrHelper.sln -c Release
```

## Publish

The MVP publish target is a framework-dependent win-x64 single-file app:

```bash
dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true --source https://api.nuget.org/v3/index.json
```

Output path:

```text
app/SnipasteOcrHelper.App/bin/Release/net8.0-windows/win-x64/publish/SnipasteOcrHelper.App.exe
```

The target machine needs the .NET 8 Windows Desktop runtime and Tesseract tessdata files for `eng+chi_sim`.
