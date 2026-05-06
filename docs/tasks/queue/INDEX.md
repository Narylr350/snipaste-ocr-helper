# queue Module Index

## Current Status

`queue` now has serial OCR orchestration with path deduplication, status publication, clipboard writes for non-empty OCR text, and continuation after OCR failures.

## Active Scope

Accept stable image paths from the watcher, process them one at a time through the configured OCR provider factory, publish app statuses, and write successful non-empty OCR text to the clipboard.

## North Star Contribution

This domain keeps screenshot bursts predictable and prevents one OCR failure from blocking later images.

## Implemented Features

- Path normalization and deduplication for already-seen files.
- Serial queue drain with `Processing`, `LastSuccess`, `NoText`, and `Error` status updates.
- Clipboard write only after successful non-empty OCR.
- Continue-on-failure behavior.
- OCR provider factory so current settings are used when processing starts.

## Pending Features

- Manual validation with rapid consecutive screenshots.
- Future retry/history behavior is outside the current MVP.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: queue tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.

## Known Issues

- Deduplication is in-memory for the app lifetime; restarting the app clears seen paths.
- Queue processing is manually drained by watcher callbacks in the MVP app shell.

## Next Useful Moves

- Validate consecutive Snipaste screenshots are neither skipped nor processed more than intended.
- Consider bounded history or retry only after MVP usage proves it is needed.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
