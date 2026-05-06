# ocr Module Index

## Current Status

`ocr` now has a local Tesseract MVP adapter behind `IImageOcrProvider`, embedded `eng`/`chi_sim` tessdata extraction, and optional settings override for tessdata directory.

## Active Scope

Use local Tesseract OCR for each stable image, defaulting to embedded tessdata extracted under LocalAppData, and return either trimmed recognized text or a failure result that can be logged and surfaced through tray status.

## North Star Contribution

This domain turns stable screenshot image paths into text for clipboard automation while isolating OCR provider details from the queue and app shell.

## Implemented Features

- `IImageOcrProvider` boundary.
- `OcrResult` success/failure result model.
- `TesseractOcrProvider` using `TesseractOCR` package version `5.5.2`.
- Default OCR language setting `eng+chi_sim`.
- Embedded `eng.traineddata` and `chi_sim.traineddata` extraction to `%LOCALAPPDATA%\SnipasteOcrHelper\tessdata`.
- Version marker for replacing previously extracted tessdata after bundled data changes.
- Failure wrapping for OCR exceptions without stopping the queue.

## Pending Features

- Future provider selection/cloud OCR is outside the current MVP.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: adapter tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 26 tests, 0 failures.
- 2026-05-06: published single exe extracted embedded tessdata to LocalAppData and OCR wrote generated image text to the clipboard without manual OCR path configuration.

## Known Issues

- Embedded tessdata increases published exe size.
- OCR accuracy and performance have not yet been manually validated with a broad real-world screenshot set.

## Next Useful Moves

- Decide later whether provider switching is still needed after local Tesseract MVP feedback.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
