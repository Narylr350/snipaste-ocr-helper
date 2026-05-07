using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class SettingsWindowTests
{
    [Fact]
    public void Constructor_AcceptsRapidOcrModelManager()
    {
        Assert.Contains(typeof(SettingsWindow).GetConstructors(), constructor =>
        {
            var parameters = constructor.GetParameters();
            return parameters.Length == 2
                && parameters[0].ParameterType == typeof(AppSettings)
                && parameters[1].ParameterType == typeof(RapidOcrModelManager);
        });
    }

    [Fact]
    public void Constructor_AcceptsRapidOcrDownloadFailureLogger()
    {
        Assert.Contains(typeof(SettingsWindow).GetConstructors(), constructor =>
        {
            var parameters = constructor.GetParameters();
            return parameters.Length == 3
                && parameters[0].ParameterType == typeof(AppSettings)
                && parameters[1].ParameterType == typeof(RapidOcrModelManager)
                && parameters[2].ParameterType == typeof(Action<string, Exception>);
        });
    }

    [Fact]
    public void Constructor_AcceptsTesseractLanguagePackManager()
    {
        Assert.Contains(typeof(SettingsWindow).GetConstructors(), constructor =>
        {
            var parameters = constructor.GetParameters();
            return parameters.Length == 4
                && parameters[0].ParameterType == typeof(AppSettings)
                && parameters[1].ParameterType == typeof(RapidOcrModelManager)
                && parameters[2].ParameterType == typeof(TesseractLanguagePackManager)
                && parameters[3].ParameterType == typeof(Action<string, Exception>);
        });
    }

    [Fact]
    public void Window_DoesNotExposeTessdataDirectoryControls()
    {
        var fields = typeof(SettingsWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.DoesNotContain(fields, field => field.Name.Contains("TessDataDirectory"));
    }

    [Fact]
    public void Window_ExposesRapidOcrModelPackSelection()
    {
        var fields = typeof(SettingsWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Contains(fields, field => field.Name == "RapidOcrModelPackComboBox");
    }

    [Fact]
    public void Window_ExposesTesseractLanguageDownloadControls()
    {
        var fields = typeof(SettingsWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Contains(fields, field => field.Name == "TesseractLanguageStatusTextBlock");
        Assert.Contains(fields, field => field.Name == "TesseractLanguageDownloadButton");
        Assert.Contains(fields, field => field.Name == "TesseractJapaneseCheckBox");
    }

    [Fact]
    public void Window_RefreshesTesseractLanguageStatusWhenSelectionChanges()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));

        Assert.Contains("Checked=\"TesseractLanguageCheckBox_Changed\"", xaml);
        Assert.Contains("Unchecked=\"TesseractLanguageCheckBox_Changed\"", xaml);
    }

    [Fact]
    public void Window_GroupsSettingsIntoClearSections()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));

        Assert.Contains("ScreenshotSourceGroupBox", xaml);
        Assert.Contains("OcrSettingsGroupBox", xaml);
        Assert.Contains("TesseractSettingsGroupBox", xaml);
        Assert.Contains("RapidOcrSettingsGroupBox", xaml);
        Assert.Contains("PostRecognitionGroupBox", xaml);
        Assert.Contains("SystemMaintenanceGroupBox", xaml);
        Assert.Contains("ScrollViewer", xaml);
    }

    [Fact]
    public void Window_ExposesOpenLogFolderButton()
    {
        var fields = typeof(SettingsWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Contains(fields, field => field.Name == "OpenLogFolderButton");
    }

    [Fact]
    public void Window_GroupsEngineSpecificSettingsSeparately()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));

        Assert.Contains("OcrSettingsGroupBox", xaml);
        Assert.Contains("TesseractSettingsGroupBox", xaml);
        Assert.Contains("RapidOcrSettingsGroupBox", xaml);
        Assert.DoesNotContain("ResourceDownloadsGroupBox", xaml);
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
