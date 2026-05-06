using System.Text;
using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class TessDataInstallerTests
{
    [Fact]
    public void AppSettings_DefaultsTessdataDirectoryToLocalAppData()
    {
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SnipasteOcrHelper",
            "tessdata");

        var settings = new AppSettings();

        Assert.Equal(expected, settings.TessDataDirectory);
    }

    [Fact]
    public async Task EnsureInstalledAsync_WritesMissingEmbeddedTrainedData()
    {
        var target = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "tessdata");
        var installer = new TessDataInstaller(
            target,
            new[]
            {
                new TessDataResource("eng.traineddata", () => new MemoryStream(Encoding.UTF8.GetBytes("eng-data"))),
                new TessDataResource("chi_sim.traineddata", () => new MemoryStream(Encoding.UTF8.GetBytes("chi-data")))
            });

        await installer.EnsureInstalledAsync();

        Assert.Equal("eng-data", await File.ReadAllTextAsync(Path.Combine(target, "eng.traineddata")));
        Assert.Equal("chi-data", await File.ReadAllTextAsync(Path.Combine(target, "chi_sim.traineddata")));
    }

    [Fact]
    public async Task EnsureInstalledAsync_ReplacesInstalledData_WhenVersionChanges()
    {
        var target = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "tessdata");
        Directory.CreateDirectory(target);
        await File.WriteAllTextAsync(Path.Combine(target, "eng.traineddata"), "old-data");
        await File.WriteAllTextAsync(Path.Combine(target, ".version"), "old-version");
        var installer = new TessDataInstaller(
            target,
            new[] { new TessDataResource("eng.traineddata", () => new MemoryStream(Encoding.UTF8.GetBytes("new-data"))) },
            "new-version");

        await installer.EnsureInstalledAsync();

        Assert.Equal("new-data", await File.ReadAllTextAsync(Path.Combine(target, "eng.traineddata")));
        Assert.Equal("new-version", await File.ReadAllTextAsync(Path.Combine(target, ".version")));
    }

    [Fact]
    public async Task CreateDefault_ExtractsBundledEnglishAndChineseData()
    {
        var target = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "tessdata");
        var installer = TessDataInstaller.CreateDefault(target);

        await installer.EnsureInstalledAsync();

        Assert.True(new FileInfo(Path.Combine(target, "eng.traineddata")).Length > 1000);
        Assert.True(new FileInfo(Path.Combine(target, "chi_sim.traineddata")).Length > 1000);
    }
}
