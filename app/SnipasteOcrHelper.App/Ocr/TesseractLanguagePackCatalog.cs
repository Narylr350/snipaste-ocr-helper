namespace SnipasteOcrHelper.Ocr;

public sealed record TesseractLanguagePack(string Code, string DisplayName);

public static class TesseractLanguagePackCatalog
{
    private const string BaseUrl = "https://raw.githubusercontent.com/tesseract-ocr/tessdata_fast/main";

    public static readonly IReadOnlyList<TesseractLanguagePack> Packs =
    [
        new("eng", "English"),
        new("chi_sim", "Simplified Chinese"),
        new("chi_tra", "Traditional Chinese"),
        new("jpn", "Japanese"),
        new("kor", "Korean")
    ];

    public static Uri GetDownloadUrl(string code)
    {
        var pack = Packs.FirstOrDefault(pack => pack.Code == code)
            ?? throw new ArgumentException($"Unsupported Tesseract language code: {code}", nameof(code));
        return new Uri($"{BaseUrl}/{pack.Code}.traineddata");
    }
}
