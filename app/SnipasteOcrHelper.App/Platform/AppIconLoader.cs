using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnipasteOcrHelper;

public static class AppIconLoader
{
    public static Icon LoadIcon()
    {
        var path = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName
            ?? throw new InvalidOperationException("Application executable path is unavailable.");
        return Icon.ExtractAssociatedIcon(path)
            ?? throw new InvalidOperationException("Application icon is unavailable.");
    }

    public static ImageSource LoadImageSource()
    {
        using var icon = LoadIcon();
        return Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }
}
