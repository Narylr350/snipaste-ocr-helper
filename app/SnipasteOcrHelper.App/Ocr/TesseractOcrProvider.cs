using SnipasteOcrHelper.Core;
using TesseractOCR;
using TesseractOCR.Enums;
using TesseractOCR.Pix;

namespace SnipasteOcrHelper.Ocr;

public sealed class TesseractOcrProvider : IImageOcrProvider
{
    private readonly string tessDataDirectory;
    private readonly string language;

    public TesseractOcrProvider(string tessDataDirectory, string language)
    {
        this.tessDataDirectory = tessDataDirectory;
        this.language = language;
    }

    public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var engine = new Engine(tessDataDirectory, language, EngineMode.Default);
            using var image = TesseractOCR.Pix.Image.LoadFromFile(imagePath);
            using var page = engine.Process(image);
            return Task.FromResult(OcrResult.Success(page.Text.Trim()));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(OcrResult.Failure(ex.Message));
        }
    }
}
