using System.Windows;
using SnipasteOcrHelper;
using SnipasteOcrHelper.Ocr;
using Forms = System.Windows.Forms;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppSettings originalSettings;
    private readonly string rapidOcrModelRootDirectory;
    private readonly Action<string, Exception>? logDownloadFailure;

    public AppSettings? SavedSettings { get; private set; }

    public SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager)
        : this(settings, rapidOcrModelManager, null)
    {
    }

    public SettingsWindow(AppSettings settings, RapidOcrModelManager rapidOcrModelManager, Action<string, Exception>? logDownloadFailure)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        ApplyLocalizedText();
        originalSettings = settings;
        rapidOcrModelRootDirectory = rapidOcrModelManager.ModelRootDirectory;
        this.logDownloadFailure = logDownloadFailure;
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        RapidOcrModelPackComboBox.ItemsSource = RapidOcrModelCatalog.Packs;
        RapidOcrModelPackComboBox.DisplayMemberPath = nameof(RapidOcrModelPackInfo.DisplayName);
        RapidOcrModelPackComboBox.SelectedValuePath = nameof(RapidOcrModelPackInfo.Pack);
        RapidOcrModelPackComboBox.SelectedValue = settings.RapidOcrModelPack;
        ApplyOcrEngine(settings.OcrEngine);
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
        WatchDirectoryLabel.Text = Strings.SettingsWatchDirectory;
        BrowseWatchDirectoryButton.Content = Strings.SettingsBrowse;
        OcrLanguageLabel.Text = Strings.SettingsOcrLanguage;
        OcrEngineLabel.Text = Strings.SettingsOcrEngine;
        TesseractEngineRadioButton.Content = Strings.SettingsOcrEngineTesseract;
        RapidOcrEngineRadioButton.Content = Strings.SettingsOcrEngineRapidOcr;
        RapidOcrModelsLabel.Text = Strings.SettingsRapidOcrModels;
        RapidOcrModelDownloadButton.Content = Strings.SettingsRapidOcrModelDownload;
        ImageDeleteModeLabel.Text = Strings.SettingsImageDeleteMode;
        ImageDeleteNeverRadioButton.Content = Strings.SettingsImageDeleteNever;
        ImageDeleteOnSuccessRadioButton.Content = Strings.SettingsImageDeleteOnSuccess;
        ImageDeleteAlwaysRadioButton.Content = Strings.SettingsImageDeleteAlways;
        StartWithWindowsCheckBox.Content = Strings.SettingsStartWithWindows;
        SaveButton.Content = Strings.SettingsSave;
        CancelButton.Content = Strings.SettingsCancel;
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

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        SavedSettings = new AppSettings
        {
            WatchDirectory = WatchDirectoryTextBox.Text.Trim(),
            TessDataDirectory = DefaultPaths.TessDataDirectory,
            OcrLanguage = originalSettings.OcrLanguage,
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
