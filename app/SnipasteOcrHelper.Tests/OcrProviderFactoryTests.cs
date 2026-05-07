using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class OcrProviderFactoryTests
{
    [Fact]
    public void Create_ReturnsTesseractProvider_WhenEngineIsTesseract()
    {
        var factory = new OcrProviderFactory(
            () => new AppSettings { OcrEngine = OcrEngineKind.Tesseract },
            _ => new FakeRapidOcrProvider());

        var provider = factory.Create();

        Assert.IsType<TesseractOcrProvider>(provider);
    }

    [Fact]
    public void Create_ReturnsRapidOcrProvider_WhenEngineIsRapidOcr()
    {
        var rapidOcr = new FakeRapidOcrProvider();
        var factory = new OcrProviderFactory(
            () => new AppSettings { OcrEngine = OcrEngineKind.RapidOcr },
            _ => rapidOcr);

        var provider = factory.Create();

        Assert.Same(rapidOcr, provider);
    }

    [Fact]
    public void Create_PassesSelectedRapidOcrModelPack_WhenEngineIsRapidOcr()
    {
        RapidOcrModelPack? selectedPack = null;
        var factory = new OcrProviderFactory(
            () => new AppSettings { OcrEngine = OcrEngineKind.RapidOcr, RapidOcrModelPack = RapidOcrModelPack.Latin },
            settings =>
            {
                selectedPack = settings.RapidOcrModelPack;
                return new FakeRapidOcrProvider();
            });

        factory.Create();

        Assert.Equal(RapidOcrModelPack.Latin, selectedPack);
    }

    private sealed class FakeRapidOcrProvider : IImageOcrProvider
    {
        public Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(OcrResult.Success("text"));
        }
    }
}
