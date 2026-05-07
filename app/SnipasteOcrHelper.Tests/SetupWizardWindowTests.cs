using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class SetupWizardWindowTests
{
    [Fact]
    public void Window_ExposesFeatureIntroAndRequiredSetupControls()
    {
        var fields = typeof(SetupWizardWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Contains(fields, field => field.Name == "FeatureIntroTextBlock");
        Assert.Contains(fields, field => field.Name == "WatchDirectoryTextBox");
        Assert.Contains(fields, field => field.Name == "TesseractEngineRadioButton");
        Assert.Contains(fields, field => field.Name == "RapidOcrEngineRadioButton");
        Assert.Contains(fields, field => field.Name == "TesseractEnglishCheckBox");
        Assert.Contains(fields, field => field.Name == "RapidOcrModelPackComboBox");
        Assert.Contains(fields, field => field.Name == "DownloadButton");
        Assert.Contains(fields, field => field.Name == "FinishButton");
    }

    [Fact]
    public void Window_ExplainsEachSetupSetting()
    {
        var fields = typeof(SetupWizardWindow).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.Contains(fields, field => field.Name == "WatchDirectoryHelpTextBlock");
        Assert.Contains(fields, field => field.Name == "OcrEngineHelpTextBlock");
        Assert.Contains(fields, field => field.Name == "TesseractLanguagesHelpTextBlock");
        Assert.Contains(fields, field => field.Name == "RapidOcrModelHelpTextBlock");
        Assert.Contains(fields, field => field.Name == "DownloadHelpTextBlock");
    }

    [Fact]
    public void Window_DownloadsResourcesForSelectedEngine()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SetupWizardWindow.xaml.cs"));

        Assert.Contains("DownloadButton_Click", source);
        Assert.Contains("ReadOcrEngine() == OcrEngineKind.RapidOcr", source);
        Assert.Contains("CreateRapidOcrModelManager().DownloadAsync", source);
        Assert.Contains("CreateTesseractLanguagePackManager().DownloadAsync", source);
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
