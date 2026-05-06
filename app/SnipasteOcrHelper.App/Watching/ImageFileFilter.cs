using System.IO;

namespace SnipasteOcrHelper.Watching;

public static class ImageFileFilter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".webp"
    };

    public static bool IsSupported(string path)
    {
        return SupportedExtensions.Contains(Path.GetExtension(path));
    }
}
