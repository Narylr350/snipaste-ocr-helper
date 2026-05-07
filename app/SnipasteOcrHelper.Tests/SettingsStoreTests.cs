using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class SettingsStoreTests
{
    [Fact]
    public async Task LoadAsync_ReturnsDefaults_WhenFileDoesNotExist()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "settings.json");
        var store = new SettingsStore(path);

        var settings = await store.LoadAsync();

        Assert.Equal("eng+chi_sim", settings.OcrLanguage);
        Assert.True(settings.MonitoringEnabled);
        Assert.False(settings.StartWithWindows);
        Assert.Equal(OcrImageDeleteMode.Never, settings.ImageDeleteMode);
        Assert.Equal(OcrEngineKind.Tesseract, settings.OcrEngine);
        Assert.Equal(RapidOcrModelPack.ChineseEnglish, settings.RapidOcrModelPack);
        Assert.Equal(string.Empty, settings.WatchDirectory);
        Assert.Equal(DefaultPaths.TessDataDirectory, settings.TessDataDirectory);
    }

    [Fact]
    public async Task LoadAsync_UsesDefaultTessdataDirectory_WhenSavedValueIsEmpty()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "settings.json");
        await File.WriteAllTextAsync(path, """
            {
              "WatchDirectory": "C:\\Screenshots",
              "TessDataDirectory": "",
              "OcrLanguage": "eng+chi_sim",
              "MonitoringEnabled": true,
              "StartWithWindows": false
            }
            """);
        var store = new SettingsStore(path);

        var settings = await store.LoadAsync();

        Assert.Equal(DefaultPaths.TessDataDirectory, settings.TessDataDirectory);
    }

    [Fact]
    public async Task LoadAsync_UsesDefaultTessdataDirectory_WhenSavedValueDoesNotExist()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "settings.json");
        await File.WriteAllTextAsync(path, """
            {
              "WatchDirectory": "C:\\Screenshots",
              "TessDataDirectory": "C:\\MissingTessdata",
              "OcrLanguage": "eng+chi_sim",
              "MonitoringEnabled": true,
              "StartWithWindows": false
            }
            """);
        var store = new SettingsStore(path);

        var settings = await store.LoadAsync();

        Assert.Equal(DefaultPaths.TessDataDirectory, settings.TessDataDirectory);
    }

    [Fact]
    public async Task LoadAsync_UsesDefaultTessdataDirectory_WhenSavedValueExistsOutsideDefaultDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var customTessdata = Path.Combine(dir, "custom-tessdata");
        Directory.CreateDirectory(customTessdata);
        File.WriteAllText(Path.Combine(customTessdata, "eng.traineddata"), "eng");
        var path = Path.Combine(dir, "settings.json");
        await File.WriteAllTextAsync(path, $$"""
            {
              "WatchDirectory": "C:\\Screenshots",
              "TessDataDirectory": "{{customTessdata.Replace("\\", "\\\\")}}",
              "OcrLanguage": "eng+chi_sim",
              "MonitoringEnabled": true,
              "StartWithWindows": false
            }
            """);
        var store = new SettingsStore(path);

        var settings = await store.LoadAsync();

        Assert.Equal(DefaultPaths.TessDataDirectory, settings.TessDataDirectory);
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTripsSettings()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var path = Path.Combine(dir, "settings.json");
        var store = new SettingsStore(path);
        var original = new AppSettings
        {
            WatchDirectory = @"C:\Screenshots",
            TessDataDirectory = DefaultPaths.TessDataDirectory,
            OcrLanguage = "eng+chi_sim",
            MonitoringEnabled = false,
            StartWithWindows = true,
            ImageDeleteMode = OcrImageDeleteMode.Always,
            OcrEngine = OcrEngineKind.RapidOcr,
            RapidOcrModelPack = RapidOcrModelPack.Latin
        };

        await store.SaveAsync(original);
        var loaded = await store.LoadAsync();

        Assert.Equal(original.WatchDirectory, loaded.WatchDirectory);
        Assert.Equal(original.TessDataDirectory, loaded.TessDataDirectory);
        Assert.Equal(original.OcrLanguage, loaded.OcrLanguage);
        Assert.Equal(original.MonitoringEnabled, loaded.MonitoringEnabled);
        Assert.Equal(original.StartWithWindows, loaded.StartWithWindows);
        Assert.Equal(original.ImageDeleteMode, loaded.ImageDeleteMode);
        Assert.Equal(original.OcrEngine, loaded.OcrEngine);
        Assert.Equal(original.RapidOcrModelPack, loaded.RapidOcrModelPack);
    }
}
