# ocr Module Index

## Current Status

`ocr` now has a local Tesseract MVP adapter behind `IImageOcrProvider`, configured by settings for tessdata directory and `eng+chi_sim` language.

## Active Scope

Use local Tesseract OCR for each stable image and return either trimmed recognized text or a failure result that can be logged and surfaced through tray status.

## North Star Contribution

This domain turns stable screenshot image paths into text for clipboard automation while isolating OCR provider details from the queue and app shell.

## Implemented Features

- `IImageOcrProvider` boundary.
- `OcrResult` success/failure result model.
- `TesseractOcrProvider` using `TesseractOCR` package version `5.5.2`.
- Default OCR language setting `eng+chi_sim`.
- Failure wrapping for OCR exceptions without stopping the queue.

## Pending Features

- Manual validation with real tessdata files and screenshots containing Chinese/English text.
- Future provider selection/cloud OCR is outside the current MVP.

## Last Effective Design

- MVP design: `docs/tasks/platform/2026-05-06-vertical-mvp-design.md`
- Implementation plan: `docs/tasks/platform/2026-05-06-vertical-mvp-implementation-plan.md`
- Product context: `docs/context/project-overview.md`
- Architecture context: `docs/context/architecture.md`

## Validation

- 2026-05-06: adapter tests passed as part of `dotnet test SnipasteOcrHelper.sln`.
- 2026-05-06: full solution test run passed: 21 tests, 0 failures.

## Known Issues

- The app requires the user to configure a tessdata directory containing the requested languages.
- OCR accuracy and performance have not yet been manually validated with real screenshots.

## Next Useful Moves

- Validate `eng+chi_sim` recognition with a small known image set.
- Decide later whether provider switching is still needed after local Tesseract MVP feedback.

Before closing work in this module, update `Current Status`, `Implemented Features`, `Validation`, `Known Issues`, and `Next Useful Moves` if any of them changed.
