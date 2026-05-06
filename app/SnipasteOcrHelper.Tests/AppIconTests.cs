namespace SnipasteOcrHelper.Tests;

public sealed class AppIconTests
{
    [Fact]
    public void ProjectUsesAppIcon()
    {
        var root = FindRepositoryRoot();
        var project = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "SnipasteOcrHelper.App.csproj"));

        Assert.Contains("<ApplicationIcon>Assets\\AppIcon.ico</ApplicationIcon>", project);
        Assert.True(File.Exists(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Assets", "AppIcon.ico")));
    }

    [Fact]
    public void SettingsWindowUsesAppIcon()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));

        Assert.Contains("Icon=\"/Assets/AppIcon.ico\"", xaml);
    }

    [Fact]
    public void TrayControllerUsesAppIcon()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Tray", "TrayController.cs"));

        Assert.Contains("Assets/AppIcon.ico", source);
        Assert.DoesNotContain("SystemIcons.Application", source);
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
