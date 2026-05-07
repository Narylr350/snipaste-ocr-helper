using System.IO;

namespace SnipasteOcrHelper.Settings;

public static class DefaultPaths
{
    public static string TessDataDirectory
    {
        get
        {
            var appTessdataDirectory = Path.Combine(AppContext.BaseDirectory, "tessdata");
            if (File.Exists(Path.Combine(appTessdataDirectory, "eng.traineddata")))
            {
                return appTessdataDirectory;
            }

            var sourceTessdataDirectory = FindSourceTessdataDirectory();
            return sourceTessdataDirectory ?? appTessdataDirectory;
        }
    }

    public static string RapidOcrModelDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SnipasteOcrHelper",
        "rapidocr-models");

    private static string? FindSourceTessdataDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var tessdataDirectory = Path.Combine(directory.FullName, "app", "SnipasteOcrHelper.App", "Ocr", "tessdata");
            if (File.Exists(Path.Combine(tessdataDirectory, "eng.traineddata")))
            {
                return tessdataDirectory;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
