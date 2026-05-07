using System.IO;
using System.Windows;
using SnipasteOcrHelper;
using SnipasteOcrHelper.Ocr;
using Forms = System.Windows.Forms;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.Settings;

public partial class SetupWizardWindow : Window
{
    private readonly AppSettings initialSettings;
    private readonly string rapidOcrModelRootDirectory;
    private readonly string tessDataDirectory;
    private readonly Action<string, Exception>? logDownloadFailure;

    public AppSettings? SavedSettings { get; private set; }

    public SetupWizardWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager, TesseractLanguagePackManager tesseractLanguagePackManager, Action<string, Exception>? logDownloadFailure)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        initialSettings = settings;
        rapidOcrModelRootDirectory = rapidOcrModelManager.ModelRootDirectory;
        tessDataDirectory = tesseractLanguagePackManager.TessDataDirectory;
        this.logDownloadFailure = logDownloadFailure;
        ApplyLocalizedText();
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        RapidOcrModelPackComboBox.ItemsSource = RapidOcrModelCatalog.Packs;
        RapidOcrModelPackComboBox.DisplayMemberPath = nameof(RapidOcrModelPackInfo.DisplayName);
        RapidOcrModelPackComboBox.SelectedValuePath = nameof(RapidOcrModelPackInfo.Pack);
        RapidOcrModelPackComboBox.SelectedValue = settings.RapidOcrModelPack;
        ApplyTesseractLanguages(settings.OcrLanguage);
        ApplyOcrEngine(settings.OcrEngine);
        UpdateSelectedEngineResourceStatus();
    }

    private void ApplyLocalizedText()
    {
        Title = Strings.SetupWizardTitle;
        FeatureIntroTextBlock.Text = Strings.SetupWizardFeatureIntro;
        ScreenshotSourceGroupBox.Header = Strings.SettingsSectionScreenshotSource;
        OcrSettingsGroupBox.Header = Strings.SettingsSectionOcrSettings;
        TesseractSettingsGroupBox.Header = Strings.SettingsSectionTesseract;
        RapidOcrSettingsGroupBox.Header = Strings.SettingsSectionRapidOcr;
        ResourceStatusGroupBox.Header = Strings.SettingsSectionSystemMaintenance;
        WatchDirectoryLabel.Text = Strings.SettingsWatchDirectory;
        WatchDirectoryHelpTextBlock.Text = Strings.SetupWizardWatchDirectoryHelp;
        BrowseWatchDirectoryButton.Content = Strings.SettingsBrowse;
        OcrEngineLabel.Text = Strings.SettingsOcrEngine;
        TesseractEngineRadioButton.Content = Strings.SettingsOcrEngineTesseract;
        RapidOcrEngineRadioButton.Content = Strings.SettingsOcrEngineRapidOcr;
        OcrEngineHelpTextBlock.Text = Strings.SetupWizardOcrEngineHelp;
        OcrLanguageLabel.Text = Strings.SettingsTesseractLanguages;
        TesseractLanguagesHelpTextBlock.Text = Strings.SetupWizardTesseractLanguagesHelp;
        RapidOcrModelsLabel.Text = Strings.SettingsRapidOcrModels;
        RapidOcrModelHelpTextBlock.Text = Strings.SetupWizardRapidOcrModelHelp;
        DownloadHelpTextBlock.Text = Strings.SetupWizardDownloadHelp;
        DownloadButton.Content = Strings.SetupWizardDownloadResources;
        FinishButton.Content = Strings.SetupWizardFinish;
        CancelButton.Content = Strings.SettingsCancel;
        ApplyTesseractLanguageText();
    }

    private void BrowseWatchDirectory_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == Forms.DialogResult.OK)
        {
            WatchDirectoryTextBox.Text = dialog.SelectedPath;
        }
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
            UpdateSelectedEngineResourceStatus();
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

    private void OcrEngineRadioButton_Changed(object sender, RoutedEventArgs e)
    {
        if (IsInitialized)
        {
            UpdateSelectedEngineResourceStatus();
        }
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
            UpdateSelectedEngineResourceStatus();
        }
    }

    private void UpdateSelectedEngineResourceStatus()
    {
        if (ReadOcrEngine() == OcrEngineKind.RapidOcr)
        {
            UpdateRapidOcrModelStatus(CreateRapidOcrModelManager().GetStatus());
        }
        else
        {
            UpdateTesseractLanguageStatus(CreateTesseractLanguagePackManager().GetStatus());
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
        StatusTextBlock.Text = string.Format(Strings.SettingsTesseractLanguageStatus, text);
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
        StatusTextBlock.Text = string.Format(Strings.SettingsRapidOcrModelStatus, text);
    }

    private async void DownloadButton_Click(object sender, RoutedEventArgs e)
    {
        DownloadButton.IsEnabled = false;
        try
        {
            if (ReadOcrEngine() == OcrEngineKind.RapidOcr)
            {
                UpdateRapidOcrModelStatus(RapidOcrModelStatus.Downloading);
                await CreateRapidOcrModelManager().DownloadAsync();
                UpdateRapidOcrModelStatus(CreateRapidOcrModelManager().GetStatus());
            }
            else
            {
                UpdateTesseractLanguageStatus(TesseractLanguagePackStatus.Downloading);
                await CreateTesseractLanguagePackManager().DownloadAsync();
                UpdateTesseractLanguageStatus(CreateTesseractLanguagePackManager().GetStatus());
            }
        }
        catch (Exception ex)
        {
            if (ReadOcrEngine() == OcrEngineKind.RapidOcr)
            {
                UpdateRapidOcrModelStatus(RapidOcrModelStatus.DownloadFailed);
                logDownloadFailure?.Invoke("RapidOCR model download failed", ex);
            }
            else
            {
                UpdateTesseractLanguageStatus(TesseractLanguagePackStatus.DownloadFailed);
                logDownloadFailure?.Invoke("Tesseract language pack download failed", ex);
            }

            StatusTextBlock.Text = $"{StatusTextBlock.Text} ({ex.Message})";
        }
        finally
        {
            DownloadButton.IsEnabled = true;
        }
    }

    private void Finish_Click(object sender, RoutedEventArgs e)
    {
        SavedSettings = new AppSettings
        {
            WatchDirectory = WatchDirectoryTextBox.Text.Trim(),
            TessDataDirectory = DefaultPaths.TessDataDirectory,
            OcrLanguage = ReadTesseractLanguage(),
            MonitoringEnabled = true,
            StartWithWindows = initialSettings.StartWithWindows,
            SetupCompleted = true,
            ImageDeleteMode = initialSettings.ImageDeleteMode,
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
