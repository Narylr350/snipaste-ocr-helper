using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class TessDataPackagingTests
{
    [Fact]
    public void AppSettings_DefaultsTessdataDirectoryToAppDataTessdataDirectory()
    {
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SnipasteOcrHelper",
            "tessdata");

        var settings = new AppSettings();

        Assert.Equal(expected, settings.TessDataDirectory);
    }

    [Fact]
    public void AppProject_DoesNotEmbedTessdata()
    {
        var root = FindRepositoryRoot();
        var project = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "SnipasteOcrHelper.App.csproj"));

        Assert.DoesNotContain("EmbeddedResource Include=\"Ocr\\tessdata", project);
    }

    [Fact]
    public void AppHost_DoesNotExtractBundledTessdataOnStartup()
    {
        var root = FindRepositoryRoot();
        var appHost = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Platform", "AppHost.cs"));

        Assert.DoesNotContain("TessDataInstaller", appHost);
    }

    [Fact]
    public void InnoSetupScript_InstallsAppWithoutBundledTessdata()
    {
        var root = FindRepositoryRoot();
        var script = File.ReadAllText(Path.Combine(root, "installer", "SnipasteOcrHelper.iss"));

        Assert.Contains("DefaultDirName={localappdata}\\Programs\\Snipaste OCR Helper", script);
        Assert.Contains("Source: \"..\\app\\SnipasteOcrHelper.App\\bin\\Release\\net8.0-windows\\win-x64\\publish\\*\"", script);
        Assert.Contains("DestDir: \"{app}\"", script);
        Assert.DoesNotContain("Ocr\\tessdata\\*.traineddata", script);
        Assert.DoesNotContain("DestDir: \"{app}\\tessdata\"", script);
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
