using System.IO;

namespace SnipasteOcrHelper.Settings;

public static class DefaultPaths
{
    public static string TessDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SnipasteOcrHelper",
        "tessdata");
}
