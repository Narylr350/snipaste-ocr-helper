# Testing Notes

Use this directory for validation notes that should survive across AI sessions.

## Testing Strategy

Automated coverage focuses on service boundaries that can run without desktop interaction: settings persistence, file filtering and stability probing, OCR queue behavior, clipboard adapter delegation, startup registry writes, and logging.

## Standard Checks

Run from the repository root:

```bash
dotnet test SnipasteOcrHelper.sln
dotnet build SnipasteOcrHelper.sln -c Release
dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true --source https://api.nuget.org/v3/index.json
```

## Manual Checks

Use these product workflows as manual validation targets:

- First launch with no watch directory opens the settings window.
- Saving a valid watch directory starts monitoring and updates tray status.
- Snipaste saving a new supported image triggers OCR after the file becomes stable.
- Non-empty OCR output overwrites the clipboard.
- Empty OCR output leaves the clipboard unchanged and reports `NoText`.
- OCR failure logs an error and the queue continues processing later images.
- Tray pause/resume stops and restarts monitoring.
- Start-with-Windows writes/removes the current-user Run registry value.

## Recent Evidence

- 2026-05-06: `dotnet test SnipasteOcrHelper.sln` passed: 26 tests, 0 failures.
- 2026-05-06: `dotnet build SnipasteOcrHelper.sln -c Release` passed: 0 warnings, 0 errors.
- 2026-05-06: `dotnet publish app/SnipasteOcrHelper.App/SnipasteOcrHelper.App.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true --source https://api.nuget.org/v3/index.json` succeeded and produced `SnipasteOcrHelper.App.exe`.
- 2026-05-06: Automated desktop validation ran from development output with generated image `HELLO OCR 123`; OCR wrote `HELLO OCR 123` to clipboard. Tessdata used: `C:\Program Files\Tesseract-OCR\tessdata`.
- 2026-05-06: Automated desktop validation ran from publish output with generated image `PUBLISHED OCR 456`; OCR wrote `PUBLISHED OCR 456` to clipboard after adding `IncludeAllContentForSelfExtract=true` to the publish command. Tessdata used: `C:\Program Files\Tesseract-OCR\tessdata`.
- 2026-05-06: User manual validation reported no problems after launching the app, configuring Snipaste watch/tessdata directories, checking clipboard OCR output, and checking tray pause/resume.
- 2026-05-06: First-run validation from a clean settings state detected the `Snipaste OCR Helper Settings` window; prior settings and Start-with-Windows state were restored afterward.
- 2026-05-06: Published single exe with embedded tessdata extracted `eng.traineddata` and `chi_sim.traineddata` to `%LOCALAPPDATA%\SnipasteOcrHelper\tessdata` from a clean tessdata state, then OCR wrote `EMBEDDED OCR 789` to the clipboard without manual OCR path configuration.

## Known Gaps

- Manual launch should be done intentionally because app startup applies the persisted Start-with-Windows setting to the current-user Run registry key.
- Published exe size is larger because `eng` and `chi_sim` tessdata are embedded.
- The MVP uses local Tesseract only; cloud OCR/provider switching remains future scope.

Keep task-specific detail in `docs/tasks/<module>/`. Keep durable validation rules and cross-module evidence here.
