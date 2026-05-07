using System.IO;
using System.Net.Http;

namespace SnipasteOcrHelper.Ocr;

public sealed class TesseractLanguagePackManager
{
    private static readonly HttpClient HttpClient = CreateHttpClient();
    private readonly Func<Uri, string, CancellationToken, Task> downloadFileAsync;

    public TesseractLanguagePackManager(string tessDataDirectory, string language)
        : this(tessDataDirectory, language, DownloadFileAsync)
    {
    }

    public TesseractLanguagePackManager(string tessDataDirectory, string language, Func<Uri, string, CancellationToken, Task> downloadFileAsync)
    {
        TessDataDirectory = tessDataDirectory;
        Language = language;
        this.downloadFileAsync = downloadFileAsync;
    }

    public string TessDataDirectory { get; }

    public string Language { get; }

    public TesseractLanguagePackStatus GetStatus()
    {
        return GetRequiredFileNames(Language).All(fileName => File.Exists(Path.Combine(TessDataDirectory, fileName)))
            ? TesseractLanguagePackStatus.Installed
            : TesseractLanguagePackStatus.Missing;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(TessDataDirectory);
        foreach (var code in GetLanguageCodes(Language))
        {
            var destinationPath = Path.Combine(TessDataDirectory, $"{code}.traineddata");
            if (!File.Exists(destinationPath))
            {
                await downloadFileAsync(TesseractLanguagePackCatalog.GetDownloadUrl(code), destinationPath, cancellationToken);
            }
        }
    }

    public static string[] GetRequiredFileNames(string language)
    {
        return [.. GetLanguageCodes(language).Select(code => $"{code}.traineddata")];
    }

    private static IEnumerable<string> GetLanguageCodes(string language)
    {
        return language.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct();
    }

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SnipasteOcrHelper/0.1");
        return client;
    }

    private static async Task DownloadFileAsync(Uri url, string destinationPath, CancellationToken cancellationToken)
    {
        var tempPath = destinationPath + ".download";
        try
        {
            using var response = await HttpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            {
                await using var input = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var output = File.Create(tempPath);
                await input.CopyToAsync(output, cancellationToken);
            }
            File.Move(tempPath, destinationPath, true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
