namespace SnipasteOcrHelper.Tests;

public sealed class AppHostSetupWizardTests
{
    [Fact]
    public void AppHost_StartUsesSetupWizardBeforeSettingsWindowForIncompleteSetup()
    {
        var root = FindRepositoryRoot();
        var appHost = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Platform", "AppHost.cs"));

        Assert.Contains("SetupCompleted", appHost);
        Assert.Contains("NeedsInitialSetup", appHost);
        Assert.Contains("OpenSetupWizard", appHost);
        Assert.Contains("TesseractLanguagePackStatus.Installed", appHost);
        Assert.Contains("RapidOcrModelStatus.Installed", appHost);
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
