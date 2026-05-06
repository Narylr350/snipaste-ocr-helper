# clipboard Module Index

## Current Status

`clipboard` now has an MVP adapter behind `IClipboardWriter` that writes recognized text through the WPF dispatcher.

## Active Scope

Overwrite the clipboard directly when OCR succeeds with non-empty text. Leave the clipboard unchanged for empty OCR text or OCR failures.

## North Star Contribution

This domain delivers the core product value: recognized screenshot text becomes immediately pasteable.

## Implemented Features

- `IClipboardWriter` boundary.
- `ClipboardWriter` using `System.Windows.Clipboard.SetText` on the WPF dispatcher.
- Injectable write delegate for automated tests.
- Queue integration that avoids clipboard writes for empty OCR output.

## Pending Features

- Future UX around clipboard overwrite warnings is outside the current MVP.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: clipboard adapter and queue clipboard tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.
- 2026-05-06: user manual validation reported no problems with OCR text appearing in the clipboard.

## Known Issues

- Direct clipboard overwrite can replace content the user copied between screenshot capture and OCR completion.

## Next Useful Moves

- Revisit overwrite UX only if MVP feedback shows it is too disruptive.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
