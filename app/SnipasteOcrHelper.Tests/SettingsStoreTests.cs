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
        Assert.Equal(string.Empty, settings.WatchDirectory);
        Assert.Equal(string.Empty, settings.TessDataDirectory);
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
            TessDataDirectory = @"C:\tessdata",
            OcrLanguage = "eng+chi_sim",
            MonitoringEnabled = false,
            StartWithWindows = true
        };

        await store.SaveAsync(original);
        var loaded = await store.LoadAsync();

        Assert.Equal(original.WatchDirectory, loaded.WatchDirectory);
        Assert.Equal(original.TessDataDirectory, loaded.TessDataDirectory);
        Assert.Equal(original.OcrLanguage, loaded.OcrLanguage);
        Assert.Equal(original.MonitoringEnabled, loaded.MonitoringEnabled);
        Assert.Equal(original.StartWithWindows, loaded.StartWithWindows);
    }
}
