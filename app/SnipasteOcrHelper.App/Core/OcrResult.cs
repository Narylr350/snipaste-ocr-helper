namespace SnipasteOcrHelper.Core;

public sealed record OcrResult(bool IsSuccess, string Text, string Error)
{
    public static OcrResult Success(string text) => new(true, text, string.Empty);
    public static OcrResult Failure(string error) => new(false, string.Empty, error);
}
