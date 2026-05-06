using System.Windows;
using Forms = System.Windows.Forms;

namespace SnipasteOcrHelper.Settings;

public partial class SettingsWindow : Window
{
    private readonly AppSettings originalSettings;

    public AppSettings? SavedSettings { get; private set; }

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        originalSettings = settings;
        WatchDirectoryTextBox.Text = settings.WatchDirectory;
        TessDataDirectoryTextBox.Text = settings.TessDataDirectory;
        StartWithWindowsCheckBox.IsChecked = settings.StartWithWindows;
    }

    private void BrowseWatchDirectory_Click(object sender, RoutedEventArgs e)
    {
        BrowseInto(WatchDirectoryTextBox);
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
