using SnipasteOcrHelper.Platform;

namespace SnipasteOcrHelper.Tests;

public sealed class StartupManagerTests
{
    [Fact]
    public void SetEnabled_WritesExecutablePath_WhenEnabled()
    {
        var registry = new FakeStartupRegistry();
        var manager = new StartupManager(registry, "SnipasteOcrHelper", @"C:\App\SnipasteOcrHelper.exe");

        manager.SetEnabled(true);

        Assert.Equal("\"C:\\App\\SnipasteOcrHelper.exe\"", registry.Values["SnipasteOcrHelper"]);
    }

    [Fact]
    public void SetEnabled_RemovesValue_WhenDisabled()
    {
        var registry = new FakeStartupRegistry();
        registry.Values["SnipasteOcrHelper"] = "old";
        var manager = new StartupManager(registry, "SnipasteOcrHelper", @"C:\App\SnipasteOcrHelper.exe");

        manager.SetEnabled(false);

        Assert.False(registry.Values.ContainsKey("SnipasteOcrHelper"));
    }

    private sealed class FakeStartupRegistry : IStartupRegistry
    {
        public Dictionary<string, string> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
        public void SetValue(string name, string value) => Values[name] = value;
        public void DeleteValue(string name) => Values.Remove(name);
    }
}
