using System.IO;
using RapidOcrNet;
using SkiaSharp;
using SnipasteOcrHelper.Core;
using SnipasteOcrHelper.Settings;
using AppOcrResult = SnipasteOcrHelper.Core.OcrResult;

namespace SnipasteOcrHelper.Ocr;

public sealed class RapidOcrProvider : IImageOcrProvider, IDisposable
{
    private readonly string modelDirectory;
    private readonly RapidOcrModelPackInfo packInfo;
    private readonly Lazy<RapidOcr> engine;

    public RapidOcrProvider(string modelRootDirectory)
        : this(modelRootDirectory, RapidOcrModelPack.ChineseEnglish)
    {
    }

    public RapidOcrProvider(string modelRootDirectory, RapidOcrModelPack pack)
    {
        packInfo = RapidOcrModelCatalog.Get(pack);
        modelDirectory = Path.Combine(modelRootDirectory, packInfo.DirectoryName);
        engine = new Lazy<RapidOcr>(CreateEngine);
    }

    public Task<AppOcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (RapidOcrModelManager.GetRequiredFileNames(packInfo.Pack).Any(fileName => !File.Exists(Path.Combine(modelDirectory, fileName))))
            {
                return Task.FromResult(AppOcrResult.Failure("RapidOCR model files are not installed."));
            }

            using var image = SKBitmap.Decode(imagePath);
            if (image is null)
            {
                return Task.FromResult(AppOcrResult.Failure("Unable to load image."));
            }

            var result = engine.Value.Detect(image, RapidOcrOptions.Default);
            return Task.FromResult(AppOcrResult.Success(result.StrRes.Trim()));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(AppOcrResult.Failure(ex.Message));
        }
    }

    public void Dispose()
    {
        if (engine.IsValueCreated)
        {
            engine.Value.Dispose();
        }
    }

    private RapidOcr CreateEngine()
    {
        var rapidOcr = new RapidOcr();
        var detPath = Path.Combine(modelDirectory, "ch_PP-OCRv5_det_mobile.onnx");
        var clsPath = Path.Combine(modelDirectory, "ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx");
        var recPath = Path.Combine(modelDirectory, packInfo.RecognitionFileName);
        var keysPath = Path.Combine(modelDirectory, packInfo.DictionaryFileName);
        rapidOcr.InitModels(detPath, clsPath, recPath, keysPath);
        return rapidOcr;
    }
}
