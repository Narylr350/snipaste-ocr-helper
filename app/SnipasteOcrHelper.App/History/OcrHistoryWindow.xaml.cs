using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using SnipasteOcrHelper;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.History;

public partial class OcrHistoryWindow : Window
{
    private readonly OcrHistoryStore history;

    public OcrHistoryWindow(OcrHistoryStore history)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        this.history = history;
        this.history.Changed += History_Changed;
        Closed += (_, _) => this.history.Changed -= History_Changed;
        ApplyLocalizedText();
        RefreshRows();
    }

    private void ApplyLocalizedText()
    {
        Title = Strings.HistoryTitle;
        EmptyMessageTextBlock.Text = Strings.HistoryEmpty;
        TimeColumn.Header = Strings.HistoryTimeColumn;
        FileColumn.Header = Strings.HistoryFileColumn;
        StatusColumn.Header = Strings.HistoryStatusColumn;
        DetailColumn.Header = Strings.HistoryDetailColumn;
        DetailLabel.Text = Strings.HistoryDetailColumn;
    }

    private void History_Changed(object? sender, EventArgs e)
    {
        _ = Dispatcher.BeginInvoke(RefreshRows);
    }

    private void RefreshRows()
    {
        var previous = (HistoryDataGrid.SelectedItem as OcrHistoryRow)?.Entry;
        var rows = history.Snapshot().Select(ToRow).ToArray();
        HistoryDataGrid.ItemsSource = rows;
        EmptyMessageTextBlock.Visibility = rows.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        HistoryDataGrid.SelectedItem = rows.FirstOrDefault(row => row.Entry == previous) ?? rows.FirstOrDefault();
        UpdateSelectedDetail();
    }

    private void HistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateSelectedDetail();
    }

    private void UpdateSelectedDetail()
    {
        DetailTextBox.Text = (HistoryDataGrid.SelectedItem as OcrHistoryRow)?.Detail ?? string.Empty;
    }

    private static OcrHistoryRow ToRow(OcrHistoryEntry entry)
    {
        return new OcrHistoryRow(
            entry,
            entry.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture),
            entry.FileName,
            LocalizeStatus(entry.Status),
            entry.Detail);
    }

    private static string LocalizeStatus(OcrHistoryStatus status)
    {
        return status switch
        {
            OcrHistoryStatus.Success => Strings.HistoryStatusSuccess,
            OcrHistoryStatus.NoText => Strings.HistoryStatusNoText,
            OcrHistoryStatus.Failed => Strings.HistoryStatusFailed,
            _ => status.ToString()
        };
    }

    private sealed record OcrHistoryRow(OcrHistoryEntry Entry, string Time, string FileName, string Status, string Detail);
}
