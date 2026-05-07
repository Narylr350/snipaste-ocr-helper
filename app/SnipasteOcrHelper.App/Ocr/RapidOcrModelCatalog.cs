using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Ocr;

public sealed record RapidOcrModelPackInfo(RapidOcrModelPack Pack, string DisplayName, string DirectoryName, string RecognitionFileName, string DictionaryFileName, Uri RecognitionUrl, Uri DictionaryUrl);

public static class RapidOcrModelCatalog
{
    private const string BaseUrl = "https://www.modelscope.cn/models/RapidAI/RapidOCR/resolve/v3.8.0";

    public static readonly IReadOnlyList<RapidOcrModelPackInfo> Packs =
    [
        Create(RapidOcrModelPack.ChineseEnglish, "Chinese + English", "ch-en", "ch_PP-OCRv5_rec_mobile.onnx", "ppocrv5_dict.txt", "ch_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.English, "English", "en", "en_PP-OCRv5_rec_mobile.onnx", "ppocrv5_en_dict.txt", "en_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Latin, "Latin languages", "latin", "latin_PP-OCRv5_rec_mobile.onnx", "ppocrv5_latin_dict.txt", "latin_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Korean, "Korean + English", "korean", "korean_PP-OCRv5_rec_mobile.onnx", "ppocrv5_korean_dict.txt", "korean_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Cyrillic, "Cyrillic languages", "cyrillic", "cyrillic_PP-OCRv5_rec_mobile.onnx", "ppocrv5_cyrillic_dict.txt", "cyrillic_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Arabic, "Arabic languages", "arabic", "arabic_PP-OCRv5_rec_mobile.onnx", "ppocrv5_arabic_dict.txt", "arabic_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Devanagari, "Devanagari languages", "devanagari", "devanagari_PP-OCRv5_rec_mobile.onnx", "ppocrv5_devanagari_dict.txt", "devanagari_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Thai, "Thai + English", "thai", "th_PP-OCRv5_rec_mobile.onnx", "ppocrv5_th_dict.txt", "th_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Greek, "Greek + English", "greek", "el_PP-OCRv5_rec_mobile.onnx", "ppocrv5_el_dict.txt", "el_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Tamil, "Tamil + English", "tamil", "ta_PP-OCRv5_rec_mobile.onnx", "ppocrv5_ta_dict.txt", "ta_PP-OCRv5_rec_mobile"),
        Create(RapidOcrModelPack.Telugu, "Telugu + English", "telugu", "te_PP-OCRv5_rec_mobile.onnx", "ppocrv5_te_dict.txt", "te_PP-OCRv5_rec_mobile")
    ];

    public static RapidOcrModelPackInfo Get(RapidOcrModelPack pack)
    {
        return Packs.First(info => info.Pack == pack);
    }

    private static RapidOcrModelPackInfo Create(RapidOcrModelPack pack, string displayName, string directoryName, string recognitionFileName, string dictionaryFileName, string paddleDirectoryName)
    {
        return new RapidOcrModelPackInfo(
            pack,
            displayName,
            directoryName,
            recognitionFileName,
            dictionaryFileName,
            new Uri($"{BaseUrl}/onnx/PP-OCRv5/rec/{recognitionFileName}"),
            new Uri($"{BaseUrl}/paddle/PP-OCRv5/rec/{paddleDirectoryName}/{dictionaryFileName}"));
    }
}
