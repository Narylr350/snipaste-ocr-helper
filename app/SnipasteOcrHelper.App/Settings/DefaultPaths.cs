using System.IO;

namespace SnipasteOcrHelper.Settings;

public static class DefaultPaths
{
    public static string TessDataDirectory => Path.Combine(AppContext.BaseDirectory, "tessdata");
}
