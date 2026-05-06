# Architecture

## Shape

`snipaste-ocr-helper` uses `single-app`.

## Application Boundaries

- `app/SnipasteOcrHelper.App` owns the Windows desktop application boundary.
- `app/SnipasteOcrHelper.Tests` owns automated tests for app-internal service boundaries.
- The MVP is a C#/.NET WPF tray application with settings UI and Windows Forms `NotifyIcon` integration.

## Core Workflow Boundaries

- Settings: persist watch directory, tessdata directory, OCR language, monitoring flag, and Start-with-Windows flag.
- Watching: monitor one configured screenshot directory, filter supported images, and wait for stable/readable files.
- Queue: deduplicate paths and process OCR jobs serially.
- OCR: local Tesseract provider behind `IImageOcrProvider`.
- Clipboard: write non-empty OCR text through `IClipboardWriter`.
- Tray: expose Open Settings, Pause/Resume Monitoring, Exit, and coarse status updates.
- Platform: app composition, file logging, and current-user startup registry integration.

## Shared Directories

- `tools`

## Integration Boundaries

Snipaste is integrated only through its auto-save directory. OCR is isolated behind a provider interface; the MVP ships local Tesseract only. Clipboard integration is isolated behind an adapter and overwrites text only after successful non-empty OCR output. Startup integration writes the current executable path to the current-user Run registry key when enabled.

## Architecture Risks

- Automatically overwriting clipboard can replace user-copied content.
- File watching can encounter partial writes, duplicate events, and bursty screenshot saves.
- Tesseract requires correctly installed tessdata files for `eng+chi_sim`.
- NotifyIcon/tray behavior and clipboard writes require manual desktop validation.

## Open Architecture Questions

- Whether local Tesseract accuracy is sufficient before adding cloud OCR.
- Whether OCR failures should become notifications or remain log/status only.
- Whether later versions need OCR history and manual retry.

## Boundary Rules

- Keep business rules inside the app or service that owns them.
- Prefer internal interfaces only at real integration boundaries: OCR, clipboard, startup registry, settings persistence, and watcher callbacks.
- Do not create a new app unless the user-facing or operational boundary is real.
- Keep docs synchronized when structure or boundaries change.
