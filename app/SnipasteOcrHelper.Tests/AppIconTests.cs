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
    public void AppIconLoaderUsesExecutableIcon()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Platform", "AppIconLoader.cs"));

        Assert.Contains("ExtractAssociatedIcon", source);
    }

    [Fact]
    public void SettingsWindowUsesExecutableIcon()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml"));
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Settings", "SettingsWindow.xaml.cs"));

        Assert.DoesNotContain("Icon=\"/Assets/AppIcon.ico\"", xaml);
        Assert.Contains("AppIconLoader.LoadImageSource()", source);
    }

    [Fact]
    public void TrayControllerUsesExecutableIcon()
    {
        var root = FindRepositoryRoot();
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "Tray", "TrayController.cs"));

        Assert.Contains("AppIconLoader.LoadIcon()", source);
        Assert.DoesNotContain("GetResourceStream", source);
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
