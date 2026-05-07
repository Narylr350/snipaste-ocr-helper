using SnipasteOcrHelper.Ocr;

namespace SnipasteOcrHelper.Tests;

public sealed class TesseractLanguagePackManagerTests
{
    [Fact]
    public void Catalog_ContainsCommonTesseractLanguages()
    {
        var codes = TesseractLanguagePackCatalog.Packs.Select(pack => pack.Code).ToArray();

        Assert.Contains("eng", codes);
        Assert.Contains("chi_sim", codes);
        Assert.Contains("chi_tra", codes);
        Assert.Contains("jpn", codes);
        Assert.Contains("kor", codes);
    }

    [Fact]
    public void GetRequiredFileNames_ParsesSelectedLanguages()
    {
        var fileNames = TesseractLanguagePackManager.GetRequiredFileNames("eng+chi_sim+jpn");

        Assert.Equal(["eng.traineddata", "chi_sim.traineddata", "jpn.traineddata"], fileNames);
    }

    [Fact]
    public void GetStatus_ReturnsMissing_WhenTessdataDirectoryDoesNotExist()
    {
        var manager = new TesseractLanguagePackManager(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")), "eng+chi_sim");

        var status = manager.GetStatus();

        Assert.Equal(TesseractLanguagePackStatus.Missing, status);
    }

    [Fact]
    public void GetStatus_ReturnsInstalled_WhenRequiredFilesExist()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "eng.traineddata"), "eng");
        File.WriteAllText(Path.Combine(directory, "chi_sim.traineddata"), "chi_sim");
        var manager = new TesseractLanguagePackManager(directory, "eng+chi_sim");

        var status = manager.GetStatus();

        Assert.Equal(TesseractLanguagePackStatus.Installed, status);
    }

    [Fact]
    public async Task DownloadAsync_DownloadsMissingSelectedLanguageFiles()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "eng.traineddata"), "eng");
        var downloaded = new List<(Uri Url, string FileName)>();
        var manager = new TesseractLanguagePackManager(directory, "eng+chi_sim+jpn", (url, destinationPath, cancellationToken) =>
        {
            downloaded.Add((url, Path.GetFileName(destinationPath)));
            File.WriteAllText(destinationPath, url.ToString());
            return Task.CompletedTask;
        });

        await manager.DownloadAsync();

        Assert.Equal(["chi_sim.traineddata", "jpn.traineddata"], downloaded.Select(item => item.FileName).ToArray());
        Assert.All(downloaded, item => Assert.Contains("https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/main/", item.Url.ToString()));
        Assert.Equal(TesseractLanguagePackStatus.Installed, manager.GetStatus());
    }
}
