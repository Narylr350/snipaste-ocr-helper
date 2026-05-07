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
}
