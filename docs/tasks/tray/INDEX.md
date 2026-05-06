# tray Module Index

## Current Status

`tray` now has an MVP `NotifyIcon` controller with settings, pause/resume, exit, and status text updates.

## Active Scope

Provide background-app control through the Windows tray: open settings, pause or resume monitoring, exit the app, and display coarse status in the tray tooltip.

## North Star Contribution

This domain makes the OCR helper usable as a background utility rather than a foreground window.

## Implemented Features

- Windows Forms `NotifyIcon` hosted by the WPF app.
- Context menu entries for Open Settings, Pause/Resume Monitoring, and Exit.
- Pause menu label toggles between pause and resume states.
- Tray tooltip status updates from `AppStatusUpdate`.

## Pending Features

- Custom icon and optional notifications remain future polish.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: tray code compiled in Debug and Release builds.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.
- 2026-05-06: user manual validation reported no problems with tray pause/resume.

## Known Issues

- Current tray icon uses `SystemIcons.Application`, not product artwork.

## Next Useful Moves

- Add a product icon only after MVP behavior is validated.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
