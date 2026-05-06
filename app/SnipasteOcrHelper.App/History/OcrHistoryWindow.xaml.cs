using System.Globalization;
using System.Windows;
using SnipasteOcrHelper;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.History;

public partial class OcrHistoryWindow : Window
{
    public OcrHistoryWindow(OcrHistoryStore history)
    {
        InitializeComponent();
        Icon = AppIconLoader.LoadImageSource();
        ApplyLocalizedText();

        var rows = history.Snapshot().Select(ToRow).ToArray();
        EmptyMessageTextBlock.Visibility = rows.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        HistoryDataGrid.ItemsSource = rows;
    }

    private void ApplyLocalizedText()
    {
        Title = Strings.HistoryTitle;
        EmptyMessageTextBlock.Text = Strings.HistoryEmpty;
        TimeColumn.Header = Strings.HistoryTimeColumn;
        FileColumn.Header = Strings.HistoryFileColumn;
        StatusColumn.Header = Strings.HistoryStatusColumn;
        DetailColumn.Header = Strings.HistoryDetailColumn;
    }

    private static OcrHistoryRow ToRow(OcrHistoryEntry entry)
    {
        return new OcrHistoryRow(
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

    private sealed record OcrHistoryRow(string Time, string FileName, string Status, string Detail);
}
