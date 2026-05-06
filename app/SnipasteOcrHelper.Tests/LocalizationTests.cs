using System.Globalization;
using SnipasteOcrHelper.Localization;

namespace SnipasteOcrHelper.Tests;

public sealed class LocalizationTests
{
    [Fact]
    public void Resources_ReturnEnglishText_ForNeutralCulture()
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            Assert.Equal("Open Settings", Resources.TrayOpenSettings);
            Assert.Equal("Snipaste OCR Helper Settings", Resources.SettingsTitle);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void Resources_ReturnChineseText_ForSimplifiedChineseCulture()
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");

            Assert.Equal("打开设置", Resources.TrayOpenSettings);
            Assert.Equal("Snipaste OCR Helper 设置", Resources.SettingsTitle);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Fact]
    public void SettingsWindow_DoesNotKeepHardCodedEnglishUiText()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));

        Assert.DoesNotContain("Snipaste OCR Helper Settings", xaml);
        Assert.DoesNotContain("Configure the folder where Snipaste saves screenshots.", xaml);
        Assert.DoesNotContain("Text=\"Watch directory\"", xaml);
        Assert.DoesNotContain("Text=\"Tessdata directory\"", xaml);
        Assert.DoesNotContain("Text=\"OCR language\"", xaml);
        Assert.DoesNotContain("Content=\"Start with Windows\"", xaml);
        Assert.DoesNotContain("Content=\"Browse\"", xaml);
        Assert.DoesNotContain("Content=\"Save\"", xaml);
        Assert.DoesNotContain("Content=\"Cancel\"", xaml);
    }

    [Fact]
    public void TrayAndAppHost_DoNotKeepHardCodedEnglishUiText()
    {
        var root = FindRepositoryRoot();
        var tray = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Tray", "TrayController.cs"));
        var appHost = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Platform", "AppHost.cs"));

        Assert.DoesNotContain("Open Settings", tray);
        Assert.DoesNotContain("Pause Monitoring", tray);
        Assert.DoesNotContain("Resume Monitoring", tray);
        Assert.DoesNotContain("\"Exit\"", tray);
        Assert.DoesNotContain("Configure watch directory", appHost);
        Assert.DoesNotContain("Watch directory does not exist", appHost);
        Assert.DoesNotContain("Monitoring paused", appHost);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "SnipasteOcrHelper.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not find repository root.");
    }
}
