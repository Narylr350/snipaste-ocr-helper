using SnipasteOcrHelper.Ocr;

namespace SnipasteOcrHelper.Tests;

public sealed class RapidOcrProviderTests
{
    [Fact]
    public async Task RecognizeAsync_ReturnsFailure_WhenModelFilesAreMissing()
    {
        var modelDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var provider = new RapidOcrProvider(modelDirectory);

        var result = await provider.RecognizeAsync("missing.png");

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }
}
