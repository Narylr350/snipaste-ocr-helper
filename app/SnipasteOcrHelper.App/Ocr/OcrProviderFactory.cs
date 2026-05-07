using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Ocr;

public sealed class OcrProviderFactory
{
    private readonly Func<AppSettings> getSettings;
    private readonly Func<AppSettings, IImageOcrProvider> createRapidOcrProvider;

    public OcrProviderFactory(Func<AppSettings> getSettings, Func<AppSettings, IImageOcrProvider> createRapidOcrProvider)
    {
        this.getSettings = getSettings;
        this.createRapidOcrProvider = createRapidOcrProvider;
    }

    public IImageOcrProvider Create()
    {
        var settings = getSettings();
        return settings.OcrEngine == OcrEngineKind.RapidOcr
            ? createRapidOcrProvider(settings)
            : new TesseractOcrProvider(settings.TessDataDirectory, settings.OcrLanguage);
    }
}
