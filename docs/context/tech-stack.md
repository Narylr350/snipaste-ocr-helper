# Tech Stack

## Selected Stack

- Runtime: C# / .NET 8 Windows desktop application (`net8.0-windows`)
- Desktop UI: WPF
- Tray integration: Windows Forms `NotifyIcon`
- OCR: `TesseractOCR` NuGet package, local tessdata, default language `eng+chi_sim`
- Tests: xUnit with `Microsoft.NET.Test.Sdk`
- Frontend: `none`
- Backend: `none`
- Mobile: `none`

## Current Fit

There is no standalone backend boundary. The `single-app` root is now a Windows tray application with file watching, OCR orchestration, settings, queueing, logging, startup integration, and clipboard writing kept inside the desktop app boundary.

## Product and Workflow Fit

- MVP scope: configure Snipaste auto-save directory and Tesseract tessdata directory, monitor new supported images, wait until files are stable, run local OCR, overwrite clipboard with non-empty recognized text, and provide tray settings/pause/resume/exit controls.
- Core workflow pressure: keep the background path reliable and quiet; OCR failures should log/status and not block later screenshots.
- Integration needs: Snipaste only through filesystem watching; clipboard through WPF dispatcher; Start-with-Windows through current-user registry.

## Validation Strategy

Automated tests cover settings persistence, file filtering/stability, OCR queue behavior, clipboard adapter delegation, Tesseract failure wrapping, startup registry writes, and logging. Manual validation must cover first-run settings, tray menu interaction, real Snipaste file events, real Tesseract OCR, and clipboard writes.

## Build, Test, Publish

```bash
dotnet test SnipasteOcrHelper.sln
dotnet build SnipasteOcrHelper.sln -c Release
dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --source https://api.nuget.org/v3/index.json
```

## Open Technical Questions

- Whether to keep framework-dependent publish or add a self-contained release later.
- Whether OCR language selection should become configurable beyond the fixed MVP `eng+chi_sim` setting.
- Whether cloud OCR/provider switching is needed after local Tesseract validation.

## Notes

- Local SDK observed during implementation was .NET SDK `10.0.101`, but projects target `net8.0-windows`.
- The publish command uses explicit nuget.org source because this machine's default NuGet source can be Visual Studio Offline Packages only.
