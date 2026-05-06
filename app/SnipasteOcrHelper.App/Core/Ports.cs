namespace SnipasteOcrHelper.Core;

public interface IImageOcrProvider
{
    Task<OcrResult> RecognizeAsync(string imagePath, CancellationToken cancellationToken = default);
}

public interface IClipboardWriter
{
    Task WriteTextAsync(string text, CancellationToken cancellationToken = default);
}

public interface IAppLogger
{
    void Info(string message);
    void Error(string message, Exception? exception = null);
}
