using System.IO;
using System.Net.Http;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Ocr;

public sealed class RapidOcrModelManager
{
    private static readonly HttpClient HttpClient = CreateHttpClient();
    private readonly Func<Uri, string, CancellationToken, Task> downloadFileAsync;
    private readonly RapidOcrModelPackInfo packInfo;

    public static readonly string[] CommonFileNames =
    [
        "ch_PP-OCRv5_det_mobile.onnx",
        "ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx"
    ];

    public static readonly string[] RequiredFileNames = GetRequiredFileNames(RapidOcrModelPack.ChineseEnglish);

    private static readonly IReadOnlyDictionary<string, Uri> CommonDownloadUrls = new Dictionary<string, Uri>
    {
        ["ch_PP-OCRv5_det_mobile.onnx"] = new("https://www.modelscope.cn/models/RapidAI/RapidOCR/resolve/v3.8.0/onnx/PP-OCRv5/det/ch_PP-OCRv5_det_mobile.onnx"),
        ["ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx"] = new("https://www.modelscope.cn/models/RapidAI/RapidOCR/resolve/v3.8.0/onnx/PP-OCRv5/cls/ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx")
    };

    public RapidOcrModelManager(string modelDirectory)
        : this(modelDirectory, RapidOcrModelPack.ChineseEnglish, DownloadFileAsync)
    {
    }

    public RapidOcrModelManager(string modelDirectory, RapidOcrModelPack pack)
        : this(modelDirectory, pack, DownloadFileAsync)
    {
    }

    public RapidOcrModelManager(string modelDirectory, Func<Uri, string, CancellationToken, Task> downloadFileAsync)
        : this(modelDirectory, RapidOcrModelPack.ChineseEnglish, downloadFileAsync)
    {
    }

    public RapidOcrModelManager(string modelRootDirectory, RapidOcrModelPack pack, Func<Uri, string, CancellationToken, Task> downloadFileAsync)
    {
        ModelRootDirectory = modelRootDirectory;
        packInfo = RapidOcrModelCatalog.Get(pack);
        ModelDirectory = Path.Combine(modelRootDirectory, packInfo.DirectoryName);
        this.downloadFileAsync = downloadFileAsync;
    }

    public string ModelRootDirectory { get; }

    public string ModelDirectory { get; }

    public RapidOcrModelStatus GetStatus()
    {
        return GetRequiredFileNames(packInfo.Pack).All(fileName => File.Exists(Path.Combine(ModelDirectory, fileName)))
            ? RapidOcrModelStatus.Installed
            : RapidOcrModelStatus.Missing;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(ModelDirectory);
        foreach (var fileName in CommonFileNames)
        {
            await downloadFileAsync(CommonDownloadUrls[fileName], Path.Combine(ModelDirectory, fileName), cancellationToken);
        }
        await downloadFileAsync(packInfo.RecognitionUrl, Path.Combine(ModelDirectory, packInfo.RecognitionFileName), cancellationToken);
        await downloadFileAsync(packInfo.DictionaryUrl, Path.Combine(ModelDirectory, packInfo.DictionaryFileName), cancellationToken);
    }

    public static string[] GetRequiredFileNames(RapidOcrModelPack pack)
    {
        var info = RapidOcrModelCatalog.Get(pack);
        return [.. CommonFileNames, info.RecognitionFileName, info.DictionaryFileName];
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
