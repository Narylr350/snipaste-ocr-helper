# platform Module Index

## Current Status

`platform` now has a working WPF tray-app vertical MVP in `app/SnipasteOcrHelper.App`, with app startup wiring, settings-first launch, logging, startup toggle integration, and a win-x64 single-file publish command.

## Active Scope

The active MVP is a single Windows desktop app: configure Snipaste's screenshot folder, monitor supported image files, wait for stable writes, process images through local Tesseract OCR, overwrite the clipboard on non-empty results, and expose status/settings/pause/resume/exit from the tray.

## North Star Contribution

This domain owns the app shell that connects settings, watcher, queue, OCR, clipboard, tray status, logging, and publishing into one usable background tool.

## Implemented Features

- WPF app shell with explicit shutdown and no primary window.
- `AppHost` composition root for settings, startup, logging, watcher, queue, OCR, clipboard, and tray services.
- First-run settings window when the watch directory is missing or invalid.
- Current-user Start-with-Windows toggle through the Run registry key.
- File logger under LocalAppData.
- Framework-dependent win-x64 single-file publish command.

## Pending Features

- Manual validation with real Snipaste screenshots, tessdata files, tray interaction, and clipboard writes.
- Product polish after MVP validation: icon, notifications, installer/autostart UX, and optional OCR history/manual retry.
- Cloud OCR/provider switching remains future scope.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`
- Product intent: `docs/product/idea.md`

## Validation

- 2026-05-06: `dotnet test SnipasteOcrHelper.sln` passed: 21 tests, 0 failures.
- 2026-05-06: `dotnet build SnipasteOcrHelper.sln -c Release` passed: 0 warnings, 0 errors.
- 2026-05-06: win-x64 single-file publish succeeded with explicit nuget.org source.

## Known Issues

- End-to-end tray, OCR, and clipboard behavior still needs manual validation on Windows.
- The published app requires .NET 8 Windows Desktop runtime and local Tesseract tessdata for `eng+chi_sim`.
- Automatically overwriting clipboard can replace user-copied content.

## Next Useful Moves

- Run Task 9 manual validation against a real watch directory and tessdata installation.
- Decide whether MVP closure needs a custom tray icon or notifications before release.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
