using System.IO;
using System.Windows;
using SnipasteOcrHelper;
using SnipasteOcrHelper.Ocr;
using Forms = System.Windows.Forms;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.Settings;

public partial class SettingsWindow : Window
{
    private readonly string rapidOcrModelRootDirectory;
    private readonly string tessDataDirectory;
    private readonly Action<string, Exception>? logDownloadFailure;
    private readonly Action<string> openFolder;

    public AppSettings? SavedSettings { get; private set; }

    public SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager)
        : this(settings, rapidOcrModelManager, new TesseractLanguagePackManager(DefaultPaths.TessDataDirectory, settings.OcrLanguage), null)
    {
    }

    public SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager, Action<string, Exception>? logDownloadFailure)
        : this(settings, rapidOcrModelManager, new TesseractLanguagePackManager(DefaultPaths.TessDataDirectory, settings.OcrLanguage), logDownloadFailure)
    {
    }

    public SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager, TesseractLanguagePackManager tesseractLanguagePackManager, Action<string, Exception>? logDownloadFailure)
        : this(settings, rapidOcrModelManager, tesseractLanguagePackManager, logDownloadFailure, OpenFolder)
    {
    }

    private SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager, TesseractLanguagePackManager tesseractLanguagePackManager, Action<string, Exception>? logDownloadFailure, Action<string> openFolder)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        ApplyLocalizedText();
        rapidOcrModelRootDirectory = rapidOcrModelManager.ModelRootDirectory;
        tessDataDirectory = tesseractLanguagePackManager.TessDataDirectory;
        this.logDownloadFailure = logDownloadFailure;
        this.openFolder = openFolder;
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        RapidOcrModelPackComboBox.ItemsSource = RapidOcrModelCatalog.Packs;
        RapidOcrModelPackComboBox.DisplayMemberPath = nameof(RapidOcrModelPackInfo.DisplayName);
        RapidOcrModelPackComboBox.SelectedValuePath = nameof(RapidOcrModelPackInfo.Pack);
        RapidOcrModelPackComboBox.SelectedValue = settings.RapidOcrModelPack;
        ApplyTesseractLanguages(settings.OcrLanguage);
        ApplyOcrEngine(settings.OcrEngine);
        UpdateTesseractLanguageStatus(CreateTesseractLanguagePackManager().GetStatus());
        UpdateRapidOcrModelStatus(CreateRapidOcrModelManager().GetStatus());
        ApplyImageDeleteMode(settings.ImageDeleteMode);
        StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
    }

    private void BrowseWatchDirectory_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(WatchDirectoryTextBox);
    }

    private void ApplyLocalizedText()
    {
        Title = Strings.SettingsTitle;
        DescriptionTextBlock.Text = Strings.SettingsDescription;
        ScreenshotSourceGroupBox.Header = Strings.SettingsSectionScreenshotSource;
        OcrSettingsGroupBox.Header = Strings.SettingsSectionOcrSettings;
        TesseractSettingsGroupBox.Header = Strings.SettingsSectionTesseract;
        RapidOcrSettingsGroupBox.Header = Strings.SettingsSectionRapidOcr;
        PostRecognitionGroupBox.Header = Strings.SettingsSectionPostRecognition;
        SystemMaintenanceGroupBox.Header = Strings.SettingsSectionSystemMaintenance;
        WatchDirectoryLabel.Text = Strings.SettingsWatchDirectory;
        BrowseWatchDirectoryButton.Content = Strings.SettingsBrowse;
        OcrLanguageLabel.Text = Strings.SettingsOcrLanguage;
        TesseractLanguagesLabel.Text = Strings.SettingsTesseractLanguages;
        TesseractLanguageDownloadButton.Content = Strings.SettingsTesseractLanguageDownload;
        OcrEngineLabel.Text = Strings.SettingsOcrEngine;
        TesseractEngineRadioButton.Content = Strings.SettingsOcrEngineTesseract;
        RapidOcrEngineRadioButton.Content = Strings.SettingsOcrEngineRapidOcr;
        RapidOcrModelsLabel.Text = Strings.SettingsRapidOcrModels;
        RapidOcrModelStatusLabel.Text = string.Empty;
        RapidOcrModelDownloadButton.Content = Strings.SettingsRapidOcrModelDownload;
        ApplyTesseractLanguageText();
        ImageDeleteModeLabel.Text = Strings.SettingsImageDeleteMode;
        ImageDeleteNeverRadioButton.Content = Strings.SettingsImageDeleteNever;
        ImageDeleteOnSuccessRadioButton.Content = Strings.SettingsImageDeleteOnSuccess;
        ImageDeleteAlwaysRadioButton.Content = Strings.SettingsImageDeleteAlways;
        StartWithWindowsCheckBox.Content = Strings.SettingsStartWithWindows;
        OpenLogFolderButton.Content = Strings.SettingsOpenLogFolder;
        SaveButton.Content = Strings.SettingsSave;
        CancelButton.Content = Strings.SettingsCancel;
    }

    private void ApplyTesseractLanguageText()
    {
        TesseractEnglishCheckBox.Content = FormatTesseractLanguage("eng");
        TesseractSimplifiedChineseCheckBox.Content = FormatTesseractLanguage("chi_sim");
        TesseractTraditionalChineseCheckBox.Content = FormatTesseractLanguage("chi_tra");
        TesseractJapaneseCheckBox.Content = FormatTesseractLanguage("jpn");
        TesseractKoreanCheckBox.Content = FormatTesseractLanguage("kor");
    }

    private static string FormatTesseractLanguage(string code)
    {
        var pack = TesseractLanguagePackCatalog.Packs.First(pack => pack.Code == code);
        return $"{pack.DisplayName} ({pack.Code})";
    }

    private void ApplyTesseractLanguages(string language)
    {
        var selected = language.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
        TesseractEnglishCheckBox.IsChecked = selected.Contains("eng");
        TesseractSimplifiedChineseCheckBox.IsChecked = selected.Contains("chi_sim");
        TesseractTraditionalChineseCheckBox.IsChecked = selected.Contains("chi_tra");
        TesseractJapaneseCheckBox.IsChecked = selected.Contains("jpn");
        TesseractKoreanCheckBox.IsChecked = selected.Contains("kor");
    }

    private void TesseractLanguageCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (IsInitialized)
        {
            UpdateTesseractLanguageStatus(CreateTesseractLanguagePackManager().GetStatus());
        }
    }

    private string ReadTesseractLanguage()
    {
        var codes = new List<string>();
        AddSelectedLanguage(codes, TesseractEnglishCheckBox, "eng");
        AddSelectedLanguage(codes, TesseractSimplifiedChineseCheckBox, "chi_sim");
        AddSelectedLanguage(codes, TesseractTraditionalChineseCheckBox, "chi_tra");
        AddSelectedLanguage(codes, TesseractJapaneseCheckBox, "jpn");
        AddSelectedLanguage(codes, TesseractKoreanCheckBox, "kor");
        return codes.Count == 0 ? "eng" : string.Join('+', codes);
    }

    private static void AddSelectedLanguage(List<string> codes, System.Windows.Controls.CheckBox checkBox, string code)
    {
        if (checkBox.IsChecked == true)
        {
            codes.Add(code);
        }
    }

    private TesseractLanguagePackManager CreateTesseractLanguagePackManager()
    {
        return new TesseractLanguagePackManager(tessDataDirectory, ReadTesseractLanguage());
    }

    private void ApplyOcrEngine(OcrEngineKind engine)
    {
        TesseractEngineRadioButton.IsChecked = engine == OcrEngineKind.Tesseract;
        RapidOcrEngineRadioButton.IsChecked = engine == OcrEngineKind.RapidOcr;
    }

    private OcrEngineKind ReadOcrEngine()
    {
        return RapidOcrEngineRadioButton.IsChecked == true ? OcrEngineKind.RapidOcr : OcrEngineKind.Tesseract;
    }

    private RapidOcrModelPack ReadRapidOcrModelPack()
    {
        return RapidOcrModelPackComboBox.SelectedValue is RapidOcrModelPack pack ? pack : RapidOcrModelPack.ChineseEnglish;
    }

    private RapidOcrModelManager CreateRapidOcrModelManager()
    {
        return new RapidOcrModelManager(rapidOcrModelRootDirectory, ReadRapidOcrModelPack());
    }

    private void RapidOcrModelPackComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (IsInitialized)
        {
            UpdateRapidOcrModelStatus(CreateRapidOcrModelManager().GetStatus());
        }
    }

    private void UpdateTesseractLanguageStatus(TesseractLanguagePackStatus status)
    {
        var text = status switch
        {
            TesseractLanguagePackStatus.Installed => Strings.SettingsRapidOcrModelInstalled,
            TesseractLanguagePackStatus.Downloading => Strings.SettingsRapidOcrModelDownloading,
            TesseractLanguagePackStatus.DownloadFailed => Strings.SettingsRapidOcrModelDownloadFailed,
            _ => Strings.SettingsRapidOcrModelMissing
        };
        TesseractLanguageStatusTextBlock.Text = string.Format(Strings.SettingsTesseractLanguageStatus, text);
    }

    private async void TesseractLanguageDownload_Click(object sender, RoutedEventArgs e)
    {
        TesseractLanguageDownloadButton.IsEnabled = false;
        UpdateTesseractLanguageStatus(TesseractLanguagePackStatus.Downloading);
        try
        {
            var manager = CreateTesseractLanguagePackManager();
            await manager.DownloadAsync();
            UpdateTesseractLanguageStatus(manager.GetStatus());
        }
        catch (Exception ex)
        {
            UpdateTesseractLanguageStatus(TesseractLanguagePackStatus.DownloadFailed);
            logDownloadFailure?.Invoke("Tesseract language pack download failed", ex);
            TesseractLanguageStatusTextBlock.Text = $"{TesseractLanguageStatusTextBlock.Text} ({ex.Message})";
        }
        finally
        {
            TesseractLanguageDownloadButton.IsEnabled = true;
        }
    }

    private void UpdateRapidOcrModelStatus(RapidOcrModelStatus status)
    {
        var text = status switch
        {
            RapidOcrModelStatus.Installed => Strings.SettingsRapidOcrModelInstalled,
            RapidOcrModelStatus.Downloading => Strings.SettingsRapidOcrModelDownloading,
            RapidOcrModelStatus.DownloadFailed => Strings.SettingsRapidOcrModelDownloadFailed,
            _ => Strings.SettingsRapidOcrModelMissing
        };
        RapidOcrModelStatusTextBlock.Text = string.Format(Strings.SettingsRapidOcrModelStatus, text);
    }

    private async void RapidOcrModelDownload_Click(object sender, RoutedEventArgs e)
    {
        RapidOcrModelDownloadButton.IsEnabled = false;
        UpdateRapidOcrModelStatus(RapidOcrModelStatus.Downloading);
        try
        {
            var modelManager = CreateRapidOcrModelManager();
            await modelManager.DownloadAsync();
            UpdateRapidOcrModelStatus(modelManager.GetStatus());
        }
        catch (Exception ex)
        {
            UpdateRapidOcrModelStatus(RapidOcrModelStatus.DownloadFailed);
            logDownloadFailure?.Invoke("RapidOCR model download failed", ex);
            RapidOcrModelStatusTextBlock.Text = $"{RapidOcrModelStatusTextBlock.Text} ({ex.Message})";
        }
        finally
        {
            RapidOcrModelDownloadButton.IsEnabled = true;
        }
    }

    private void ApplyImageDeleteMode(OcrImageDeleteMode mode)
    {
        ImageDeleteNeverRadioButton.IsChecked = mode == OcrImageDeleteMode.Never;
        ImageDeleteOnSuccessRadioButton.IsChecked = mode == OcrImageDeleteMode.OnSuccess;
        ImageDeleteAlwaysRadioButton.IsChecked = mode == OcrImageDeleteMode.Always;
    }

    private OcrImageDeleteMode ReadImageDeleteMode()
    {
        if (ImageDeleteAlwaysRadioButton.IsChecked == true)
        {
            return OcrImageDeleteMode.Always;
        }

        return ImageDeleteOnSuccessRadioButton.IsChecked == true ? OcrImageDeleteMode.OnSuccess : OcrImageDeleteMode.Never;
    }

    private static void BrowseInto(System.Windows.Controls.TextBox target)
    {
        using var dialog = new Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == Forms.DialogResult.OK)
        {
            target.Text = dialog.SelectedPath;
        }
    }

    private void OpenLogFolder_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(DefaultPaths.LogDirectory);
        openFolder(DefaultPaths.LogDirectory);
    }

    private static void OpenFolder(string path)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        SavedSettings = new AppSettings
        {
            WatchDirectory = WatchDirectoryTextBox.Text.Trim(),
            TessDataDirectory = DefaultPaths.TessDataDirectory,
            OcrLanguage = ReadTesseractLanguage(),
            MonitoringEnabled = true,
            StartWithWindows = StartWithWindowsCheckBox.IsChecked == true,
            ImageDeleteMode = ReadImageDeleteMode(),
            OcrEngine = ReadOcrEngine(),
            RapidOcrModelPack = ReadRapidOcrModelPack()
        };
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
