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
            Assert.Equal("OCR History", Resources.TrayOcrHistory);
            Assert.Equal("Snipaste OCR Helper Settings", Resources.SettingsTitle);
            Assert.Equal("After recognition", Resources.SettingsImageDeleteMode);
            Assert.Equal("OCR engine", Resources.SettingsOcrEngine);
            Assert.Equal("Tesseract", Resources.SettingsOcrEngineTesseract);
            Assert.Equal("RapidOCR", Resources.SettingsOcrEngineRapidOcr);
            Assert.Equal("RapidOCR models", Resources.SettingsRapidOcrModels);
            Assert.Equal("Chinese + English model: {0}", Resources.SettingsRapidOcrModelStatus);
            Assert.Equal("Download model", Resources.SettingsRapidOcrModelDownload);
            Assert.Equal("Tesseract languages", Resources.SettingsTesseractLanguages);
            Assert.Equal("Language packs: {0}", Resources.SettingsTesseractLanguageStatus);
            Assert.Equal("Download languages", Resources.SettingsTesseractLanguageDownload);
            Assert.Equal("Screenshot source", Resources.SettingsSectionScreenshotSource);
            Assert.Equal("OCR settings", Resources.SettingsSectionOcrSettings);
            Assert.Equal("Tesseract", Resources.SettingsSectionTesseract);
            Assert.Equal("RapidOCR", Resources.SettingsSectionRapidOcr);
            Assert.Equal("Post-recognition", Resources.SettingsSectionPostRecognition);
            Assert.Equal("System / Maintenance", Resources.SettingsSectionSystemMaintenance);
            Assert.Equal("Open log folder", Resources.SettingsOpenLogFolder);
            Assert.Equal("Installed", Resources.SettingsRapidOcrModelInstalled);
            Assert.Equal("Not installed", Resources.SettingsRapidOcrModelMissing);
            Assert.Equal("Downloading", Resources.SettingsRapidOcrModelDownloading);
            Assert.Equal("Download failed", Resources.SettingsRapidOcrModelDownloadFailed);
            Assert.Equal("Do not delete", Resources.SettingsImageDeleteNever);
            Assert.Equal("Delete successful images", Resources.SettingsImageDeleteOnSuccess);
            Assert.Equal("Always delete images", Resources.SettingsImageDeleteAlways);
            Assert.Equal("Recognition History", Resources.HistoryTitle);
            Assert.Equal("Success", Resources.HistoryStatusSuccess);
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
            Assert.Equal("识别记录", Resources.TrayOcrHistory);
            Assert.Equal("Snipaste OCR Helper 设置", Resources.SettingsTitle);
            Assert.Equal("识别后处理图片", Resources.SettingsImageDeleteMode);
            Assert.Equal("OCR 引擎", Resources.SettingsOcrEngine);
            Assert.Equal("Tesseract", Resources.SettingsOcrEngineTesseract);
            Assert.Equal("RapidOCR", Resources.SettingsOcrEngineRapidOcr);
            Assert.Equal("RapidOCR 模型", Resources.SettingsRapidOcrModels);
            Assert.Equal("中英模型：{0}", Resources.SettingsRapidOcrModelStatus);
            Assert.Equal("下载模型", Resources.SettingsRapidOcrModelDownload);
            Assert.Equal("Tesseract 语言", Resources.SettingsTesseractLanguages);
            Assert.Equal("语言包：{0}", Resources.SettingsTesseractLanguageStatus);
            Assert.Equal("下载语言", Resources.SettingsTesseractLanguageDownload);
            Assert.Equal("截图来源", Resources.SettingsSectionScreenshotSource);
            Assert.Equal("OCR 设置", Resources.SettingsSectionOcrSettings);
            Assert.Equal("Tesseract", Resources.SettingsSectionTesseract);
            Assert.Equal("RapidOCR", Resources.SettingsSectionRapidOcr);
            Assert.Equal("识别后处理", Resources.SettingsSectionPostRecognition);
            Assert.Equal("系统 / 维护", Resources.SettingsSectionSystemMaintenance);
            Assert.Equal("打开日志文件夹", Resources.SettingsOpenLogFolder);
            Assert.Equal("已安装", Resources.SettingsRapidOcrModelInstalled);
            Assert.Equal("未安装", Resources.SettingsRapidOcrModelMissing);
            Assert.Equal("下载中", Resources.SettingsRapidOcrModelDownloading);
            Assert.Equal("下载失败", Resources.SettingsRapidOcrModelDownloadFailed);
            Assert.Equal("不删除", Resources.SettingsImageDeleteNever);
            Assert.Equal("识别成功后删除", Resources.SettingsImageDeleteOnSuccess);
            Assert.Equal("总是删除", Resources.SettingsImageDeleteAlways);
            Assert.Equal("识别记录", Resources.HistoryTitle);
            Assert.Equal("成功", Resources.HistoryStatusSuccess);
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
        Assert.DoesNotContain("Content=\"Do not delete\"", xaml);
        Assert.DoesNotContain("Content=\"Delete successful images\"", xaml);
        Assert.DoesNotContain("Content=\"Always delete images\"", xaml);
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
        Assert.Contains("TrayOcrHistory", tray);
        Assert.DoesNotContain("OCR History", tray);
        Assert.DoesNotContain("Pause Monitoring", tray);
        Assert.DoesNotContain("Resume Monitoring", tray);
        Assert.DoesNotContain("\"Exit\"", tray);
        Assert.Contains("OpenHistory", appHost);
        Assert.Contains("historyWindow", appHost);
        Assert.DoesNotContain("new OcrHistoryWindow(history).Show();", appHost);
        Assert.DoesNotContain("Configure watch directory", appHost);
        Assert.DoesNotContain("Watch directory does not exist", appHost);
        Assert.DoesNotContain("Monitoring paused", appHost);
    }

    [Fact]
    public void HistoryWindow_DoesNotKeepHardCodedEnglishUiText()
    {
        var root = FindRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "History", "OcrHistoryWindow.xaml"));
        var source = File.ReadAllText(Path.Combine(root, "app", "SnipasteOcrHelper.App", "History", "OcrHistoryWindow.xaml.cs"));

        Assert.DoesNotContain("Recognition History", xaml);
        Assert.DoesNotContain("Header=\"Time\"", xaml);
        Assert.DoesNotContain("Header=\"File\"", xaml);
        Assert.DoesNotContain("Header=\"Status\"", xaml);
        Assert.DoesNotContain("Header=\"Detail\"", xaml);
        Assert.DoesNotContain("No recognition records yet.", xaml);
        Assert.Contains("DetailTextBox", source);
        Assert.Contains("history.Changed", source);
        Assert.Contains("TextTrimming", xaml);
        Assert.Contains("Value=\"NoWrap\"", xaml);
        Assert.Contains("IsReadOnly=\"True\"", xaml);
        Assert.Contains("AcceptsReturn=\"True\"", xaml);
        Assert.Contains("HistoryStatusSuccess", source);
        Assert.Contains("HistoryStatusNoText", source);
        Assert.Contains("HistoryStatusFailed", source);
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
