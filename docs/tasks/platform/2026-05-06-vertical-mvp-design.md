# Vertical MVP Design

## Summary

Build the first shippable vertical MVP as a single WPF desktop application. The app runs as a Windows tray utility, watches the Snipaste auto-save directory, waits for new image files to become stable, runs local Tesseract OCR with Chinese-English language support, writes successful OCR text directly to the clipboard, and exposes status plus pause/resume through the tray.

The first delivery includes single-file Windows publishing and an app-controlled startup toggle. It does not include screenshot capture, Snipaste UI replacement, cloud OCR, OCR history, manual retry UI, or complex image editing.

## Approved Scope

- One WPF app under `app`.
- Internal service boundaries for platform, settings, tray, watcher, queue, OCR, and clipboard.
- First-run settings window when no watch directory is configured.
- File watching for Snipaste auto-save images.
- Stable-file detection before OCR.
- Path-based dedupe and serial OCR queue processing.
- Local Tesseract OCR provider with Chinese-English default recognition.
- Direct clipboard overwrite on successful non-empty OCR text.
- Logs plus tray status for failures; no failure popups in the background flow.
- Single-file publish output for Windows.
- User-configurable startup toggle.

## Architecture

Use the recommended `single-app` shape. The first implementation should avoid premature multi-project splitting while still keeping internal services well bounded.

Runtime flow:

1. `AppHost` starts the WPF process, initializes services, creates the tray shell, loads settings, and decides whether to open the settings window.
2. `SettingsStore` persists local settings such as watch directory, Tesseract data path, OCR language, and startup preference.
3. `TrayController` owns tray lifecycle, menu commands, and status display.
4. `ScreenshotWatcher` wraps `FileSystemWatcher`, filters image extensions, supports pause/resume, and switches directories when settings change.
5. `FileStabilityProbe` waits until file size and modified time stop changing before allowing processing.
6. `OcrQueue` deduplicates paths and processes files one at a time so duplicate file events and OCR failures do not disrupt later screenshots.
7. `TesseractOcrProvider` runs local OCR and returns recognized text or a failure result.
8. `ClipboardWriter` writes successful OCR text to the Windows clipboard.
9. Logging records operational failures and recent processing activity.

## Components

### AppHost

Owns application startup and shutdown. It wires dependencies, starts the tray controller, loads settings, starts watching when configuration is valid, and opens the settings window on first launch if no watch directory is configured.

### SettingsStore

Persists a small JSON settings file in the user application data area. Settings include:

- Snipaste auto-save directory
- Tesseract data directory
- OCR language defaulting to Chinese-English
- whether monitoring starts enabled
- whether Windows startup is enabled

### TrayController

Provides tray menu commands:

- Open Settings
- Pause Monitoring / Resume Monitoring
- Open Logs or show log location
- Exit

Tray state should communicate:

- Needs Setup
- Running
- Paused
- Processing
- Last Success
- Error
- No Text

### ScreenshotWatcher

Watches the configured directory for created or changed image files. It filters to common screenshot formats such as PNG, JPG, JPEG, BMP, and WEBP when supported by the OCR path. It does not call Snipaste APIs.

### FileStabilityProbe

Prevents OCR from reading partially written screenshots. A file is considered stable after size and last-write timestamp remain unchanged across a short polling window and the file can be opened for read access.

### OcrQueue

Provides a single-consumer queue. It deduplicates normalized file paths while a file is pending or being processed. Each file result updates tray state and logs. A failed item is completed and does not block the next item.

### TesseractOcrProvider

Runs local OCR using Tesseract. The default language target is Chinese-English mixed recognition. The provider boundary should make it possible to add other OCR providers later without changing watcher or queue logic.

### ClipboardWriter

Writes recognized non-empty text directly to the clipboard. Empty OCR output is treated as a completed no-text result and does not overwrite the existing clipboard.

### StartupManager

Controls the startup toggle for the current Windows user. The MVP can use the standard current-user startup mechanism that fits the WPF deployment shape; it should be documented with the chosen implementation during planning.

## Data Flow

Happy path:

1. User configures the Snipaste auto-save directory.
2. App enters Running state and starts directory monitoring.
3. Snipaste saves a new image file.
4. Watcher receives a filesystem event and filters unsupported file types.
5. Stability probe waits until the file is complete.
6. Queue deduplicates and serially processes the image.
7. Tesseract OCR extracts text.
8. Non-empty text is written directly to the clipboard.
9. Tray shows recent success and logs the processed file.

Failure path:

- Invalid or missing directory: show Needs Setup or Error, open settings when appropriate, do not start watching.
- File never stabilizes: log failure, mark item failed, continue queue.
- Duplicate watcher events: dedupe by normalized path.
- OCR failure: log file path and reason, show Error summary, continue queue.
- Empty OCR result: log No Text, do not overwrite clipboard, continue queue.
- Clipboard failure: log failure, show Error summary, continue queue.

## User Experience

On first launch, if no watch directory exists in settings, the settings window opens automatically. After valid configuration, later launches default to tray-only operation.

The settings window should stay small and focused:

- Watch directory picker
- Tesseract data path or language data requirement
- OCR language display/default for Chinese-English
- Startup toggle
- Save/Cancel actions
- Recent status summary if useful without adding history management

The tray is the primary background surface. It should support pause/resume without closing the application. Pause stops processing new watcher events but does not require deleting settings.

## Release Shape

The first release target is a Windows single-file publish output. This is not a full installer. The app should also provide a startup toggle that works for the current user and matches the published executable path.

A future phase can add an installer such as MSIX or Inno Setup if distribution needs grow.

## Testing Strategy

Automated tests should cover:

- settings read/write defaults and updates
- file extension filtering
- file stability decisions
- queue path dedupe
- queue continues after OCR failure
- empty OCR result does not write clipboard
- successful OCR result invokes clipboard writer
- pause/resume state transitions where practical

Manual validation should cover:

- first launch opens settings when no directory is configured
- configuring a watch directory starts monitoring
- copying or saving a PNG into the watched directory triggers OCR
- successful OCR writes text directly to the clipboard
- rapid consecutive screenshots do not duplicate or block each other
- pause/resume prevents and restores processing
- changing watch directory switches monitoring
- OCR failure updates tray status and logs without blocking later files
- single-file publish output launches successfully
- startup toggle can be enabled and disabled

## Documentation Writeback

When this design is implemented, update affected module indexes under `docs/tasks/` and keep durable verification evidence in `docs/testing/README.md`. If implementation changes architecture or engineering contracts, update `docs/context/architecture.md`, `docs/context/tech-stack.md`, or `docs/engineering/` in the same change.
