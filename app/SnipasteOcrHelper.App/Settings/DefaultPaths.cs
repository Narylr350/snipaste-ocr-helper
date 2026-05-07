using System.IO;

namespace SnipasteOcrHelper.Settings;

public static class DefaultPaths
{
    public static string TessDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SnipasteOcrHelper",
        "tessdata");

    public static string RapidOcrModelDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SnipasteOcrHelper",
        "rapidocr-models");

    public static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SnipasteOcrHelper",
        "logs");
}
