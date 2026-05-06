# watcher Module Index

## Current Status

`watcher` now has MVP implementation for supported image filtering, file stability probing, and a pauseable `FileSystemWatcher` adapter.

## Active Scope

Watch one configured Snipaste screenshot directory, ignore unsupported extensions, wait until candidate images are stable/readable, and enqueue stable images for OCR unless monitoring is paused.

## North Star Contribution

This domain protects the OCR pipeline from duplicate filesystem noise and partially-written screenshot files.

## Implemented Features

- Supported image extension filter for `.png`, `.jpg`, `.jpeg`, `.bmp`, and `.webp`.
- File stability probe based on length, last-write timestamp, and read access.
- `ScreenshotWatcher` subscribing to created/changed events without subdirectory recursion.
- Pause/resume support used by the tray menu.

## Pending Features

- Manual validation with Snipaste auto-save events.
- Additional tuning if real-world Snipaste writes produce duplicate or delayed events.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: watcher unit tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.

## Known Issues

- `FileSystemWatcher` may still emit duplicate events; queue deduplication handles already-seen paths for the current app lifetime.
- Real Snipaste write timing still needs manual validation.

## Next Useful Moves

- Validate large screenshot writes do not trigger OCR before the file is complete.
- If duplicates appear in manual validation, tune stability timing or event coalescing.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
