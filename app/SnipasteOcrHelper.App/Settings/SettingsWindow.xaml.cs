using System.Windows;
using SnipasteOcrHelper;
using Forms = System.Windows.Forms;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppSettings originalSettings;

    public AppSettings? SavedSettings { get; private set; }

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        ApplyLocalizedText();
        originalSettings = settings;
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        TessDataDirectoryTextBox.Text = settings.TessDataDirectory;
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
        TessDataDirectoryLabel.Text = Strings.SettingsTessDataDirectory;
        BrowseTessDataDirectoryButton.Content = Strings.SettingsBrowse;
        OcrLanguageLabel.Text = Strings.SettingsOcrLanguage;
        StartWithWindowsCheckBox.Content = Strings.SettingsStartWithWindows;
        SaveButton.Content = Strings.SettingsSave;
        CancelButton.Content = Strings.SettingsCancel;
    }

    private void BrowseTessDataDirectory_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(TessDataDirectoryTextBox);
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
            TessDataDirectory = TessDataDirectoryTextBox.Text.Trim(),
            OcrLanguage = originalSettings.OcrLanguage,
            MonitoringEnabled = true,
            StartWithWindows = StartWithWindowsCheckBox.IsChecked == true
        };
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
