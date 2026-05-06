# settings Module Index

## Current Status

`settings` now has JSON persistence and a WPF settings window for watch directory, tessdata directory, and Start-with-Windows configuration.

## Active Scope

Persist local app settings under LocalAppData, open settings on first run when the watch directory is missing, and let users choose directories and startup behavior from the MVP settings window.

## North Star Contribution

This domain lets users connect the helper to their Snipaste screenshot folder and local Tesseract language data without editing files manually.

## Implemented Features

- `AppSettings` model with watch directory, tessdata directory, OCR language, monitoring flag, and startup flag.
- `SettingsStore` JSON load/save with defaults.
- WPF `SettingsWindow` with folder browse buttons for watch and tessdata directories.
- Fixed OCR language display for `eng+chi_sim` in the MVP.
- Start-with-Windows checkbox persisted into settings and applied by the app host.

## Pending Features

- Manual validation of first-run setup from a clean settings state.
- Provider/language selection UI remains future scope.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: settings persistence tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.
- 2026-05-06: user manual validation reported no problems after configuring watch and tessdata directories.

## Known Issues

- First-run settings UI still needs explicit validation from a clean settings state.
- Tessdata path validity is not deeply validated beyond OCR failure reporting.

## Next Useful Moves

- Validate first-run setup from a clean settings state.
- Decide later whether OCR language should become user-editable.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
